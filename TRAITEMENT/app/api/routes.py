from fastapi import APIRouter, File, UploadFile
from app.services.upload_service import save_upload

router = APIRouter()


@router.get("/")
async def root():
    return {"message": "Service de traitement opérationnel"}


@router.get("/health")
async def health_check():
    return {"status": "healthy"}


@router.post("/upload")
async def upload_file(file: UploadFile = File(...)):
    return await save_upload(file)
