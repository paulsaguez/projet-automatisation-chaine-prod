from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
import os

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

@app.get("/")
async def root():
    return {"message": "Service de traitement opérationnel"}

@app.get("/health")
async def health_check():
    return {"status": "healthy"}

import pandas as pd
import requests
import io
from fastapi import UploadFile, File, HTTPException
import json

# URL de l'API C# (définie dans docker-compose)
API_URL = os.getenv("API_URL", "http://api:5000")

import shutil
import pathlib
from fastapi import Form

TEMP_DIR = pathlib.Path("/tmp/uploads")
TEMP_DIR.mkdir(parents=True, exist_ok=True)

@app.post("/upload_chunk")
async def upload_chunk(
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
            
            # Traitement du fichier reconstitué (Logique existante)
            try:
                # Lecture CSV
                df = pd.read_csv(final_file_path)
                
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
                    for key, value in report.items():
                        if value.lower() == "nan":
                            report[key] = None
                    reports.append(report)

                # Envoi API C#
                api_endpoint = f"{API_URL}/api/migration"
                response = requests.post(api_endpoint, json=reports)
                
                if response.status_code not in [200, 201]:
                     raise HTTPException(status_code=500, detail=f"Erreur API C#: {response.text}")

                return {"message": "Fichier reconstitué et traité avec succès", "filename": file.filename, "rows_processed": len(reports)}

            finally:
                # Nettoyage
                shutil.rmtree(upload_dir)

        return {"message": f"Chunk {chunk_index} reçu"}

    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

