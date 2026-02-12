from fastapi import FastAPI, BackgroundTasks, UploadFile, File, Form, HTTPException
from fastapi.middleware.cors import CORSMiddleware
import os
import shutil
import pathlib
import pandas as pd
import requests
import hashlib

app = FastAPI(
    title="Service Traitement",
    description="API de traitement de données",
    version="1.0.0"
)

# Configuration CORS
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

API_URL = os.getenv("API_URL", "http://api:5000")
TEMP_DIR = pathlib.Path("/tmp/uploads")
TEMP_DIR.mkdir(parents=True, exist_ok=True)

@app.get("/")
async def root():
    return {"message": "Service de traitement opérationnel"}

@app.get("/health")
async def health_check():
    return {"status": "healthy"}

def process_file_background(file_path: pathlib.Path, filename: str):
    try:
        df = pd.read_csv(file_path)
        reports = []
        
        for _, row in df.iterrows():
            report = {
                "migration_start_time": str(row.get("Migration start time", "")),
                "sub_job_id": str(row.get("Sub job ID", "")),
                "title": str(row.get("Title", "")),
                "type": str(row.get("Type", "")),
                "source_id": str(row.get("Source ID", "")),
                "source": str(row.get("Source", "")),
                "destination_id": str(row.get("Destination ID", "")),
                "destination": str(row.get("Destination", "")),
                "size": str(row.get("Size", "")),
                "status": str(row.get("Status", "")),
                "migration_action": str(row.get("Migration action", "")),
                "comment": str(row.get("Comment", "")),
                "error_code": str(row.get("Error code", ""))
            }
            
            # Clean NaN values
            for key, value in report.items():
                if value.lower() == "nan":
                    report[key] = None

            # Generate Hash
            concat_str = f"{report['migration_start_time']}|{report['source_id']}|{report['destination_id']}|{report['title']}|{report['status']}"
            report['hash'] = hashlib.sha256(concat_str.encode('utf-8')).hexdigest()
            reports.append(report)

        all_hashes = [r['hash'] for r in reports]
        check_endpoint = f"{API_URL}/api/migration/check"
        existing_hashes = set()
        
        try:
            check_res = requests.post(check_endpoint, json=all_hashes)
            if check_res.status_code == 200:
                existing_hashes = set(check_res.json())
        except:
            pass 

        reports_to_send = [r for r in reports if r['hash'] not in existing_hashes]
        
        if reports_to_send:
            api_endpoint = f"{API_URL}/api/migration"
            requests.post(api_endpoint, json=reports_to_send)

    except Exception as e:
        print(f"Error processing {filename}: {e}")
    finally:
        shutil.rmtree(file_path.parent)

@app.post("/upload_chunk")
async def upload_chunk(
    background_tasks: BackgroundTasks,
    file: UploadFile = File(...),
    upload_id: str = Form(...),
    chunk_index: int = Form(...),
    total_chunks: int = Form(...)
):
    try:
        # Dossier temporaire pour cet upload
        upload_dir = TEMP_DIR / upload_id
        upload_dir.mkdir(parents=True, exist_ok=True)

        # Sauvegarde du chunk
        chunk_path = upload_dir / f"chunk_{chunk_index}"
        with open(chunk_path, "wb") as buffer:
            shutil.copyfileobj(file.file, buffer)

        # Si c'est le dernier chunk, on assemble tout
        if chunk_index == total_chunks - 1:
            final_file_path = upload_dir / file.filename
            with open(final_file_path, "wb") as final_file:
                for i in range(total_chunks):
                    part_path = upload_dir / f"chunk_{i}"
                    if not part_path.exists():
                        raise HTTPException(status_code=400, detail=f"Manque le morceau {i}")
                    
                    with open(part_path, "rb") as part_file:
                        shutil.copyfileobj(part_file, final_file)
            
            # Lancement du traitement en arrière-plan
            background_tasks.add_task(process_file_background, final_file_path, file.filename)

            return {"message": "Fichier reconstitué. Traitement démarré en arrière-plan.", "filename": file.filename}

        return {"message": f"Chunk {chunk_index} reçu"}

    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

