import os
from fastapi import HTTPException, UploadFile
from app.core.config import settings


async def save_upload(file: UploadFile) -> dict:
    if not file.filename:
        raise HTTPException(status_code=400, detail="Aucun fichier fourni")

    safe_name = os.path.basename(file.filename)
    destination = settings.upload_dir / safe_name

    try:
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
