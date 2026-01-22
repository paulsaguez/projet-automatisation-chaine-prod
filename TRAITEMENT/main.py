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

# TODO: Ajouter vos endpoints de traitement ici
