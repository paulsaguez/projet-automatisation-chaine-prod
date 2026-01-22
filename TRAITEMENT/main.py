from fastapi import FastAPI, File, HTTPException, UploadFile
from fastapi.middleware.cors import CORSMiddleware
from pathlib import Path
import os

app = FastAPI(
    title="Service Traitement",
    description="API de traitement de données",
    version="1.0.0"
)

UPLOAD_DIR = Path(os.getenv("UPLOAD_DIR", "/app/uploads"))
UPLOAD_DIR.mkdir(parents=True, exist_ok=True)

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

@app.post("/upload")
async def upload_file(file: UploadFile = File(...)):
    if not file.filename:
        raise HTTPException(status_code=400, detail="Aucun fichier fourni")

    safe_name = os.path.basename(file.filename)
    destination = UPLOAD_DIR / safe_name

    try:
        # Stream the file to disk to avoid loading large files in memory.
        with destination.open("wb") as buffer:
            while True:
                chunk = await file.read(1024 * 1024)
                if not chunk:
                    break
                buffer.write(chunk)
    except Exception as exc:  # pragma: no cover - defensive logging path
        raise HTTPException(status_code=500, detail=f"Erreur lors de l'enregistrement du fichier: {exc}")
    finally:
        await file.close()

    return {"filename": safe_name, "path": str(destination)}