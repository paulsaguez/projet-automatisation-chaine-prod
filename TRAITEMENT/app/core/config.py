from pathlib import Path
import os
from pydantic import BaseModel


class Settings(BaseModel):
    upload_dir: Path

    @classmethod
    def load(cls) -> "Settings":
        upload_dir = Path(os.getenv("UPLOAD_DIR", "/app/uploads"))
        upload_dir.mkdir(parents=True, exist_ok=True)
        return cls(upload_dir=upload_dir)

    class Config:
        frozen = True


settings = Settings.load()
