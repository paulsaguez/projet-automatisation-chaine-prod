import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-add-data',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './add-data.component.html',
  styleUrl: './add-data.component.scss',
})
export class AddDataComponent {
  messages: string[] = [];
  selectedFile: File | null = null;
  isUploading: boolean = false;

  constructor(private http: HttpClient) {}

  onFileSelected(event: any) {
    const file: File = event.target.files[0];
    if (file) {
      this.selectedFile = file;
      this.messages.push(`Fichier sélectionné: ${file.name}`);
    }
  }

  async onUpload() {
    if (this.selectedFile && !this.isUploading) {
      this.isUploading = true;
      const CHUNK_SIZE = 1 * 1024 * 1024;
      const totalChunks = Math.ceil(this.selectedFile.size / CHUNK_SIZE);
      const uploadId = crypto.randomUUID();

      this.messages.push(`Début de l'upload...`);

      for (let i = 0; i < totalChunks; i++) {
        const start = i * CHUNK_SIZE;
        const end = Math.min(start + CHUNK_SIZE, this.selectedFile.size);
        const chunk = this.selectedFile.slice(start, end);

        const formData = new FormData();
        formData.append('file', chunk, this.selectedFile.name);
        formData.append('upload_id', uploadId);
        formData.append('chunk_index', i.toString());
        formData.append('total_chunks', totalChunks.toString());

        try {
          await new Promise((resolve, reject) => {
            this.http.post('/traitement/upload_chunk', formData).subscribe({
              next: (response: any) => {
                if (i === totalChunks - 1) {
                  this.messages.push(
                    `✅ Upload terminé : ${response.filename}`,
                  );
                } else if (i % 5 === 0) {
                  this.messages.push(
                    `... Envoi partie ${i + 1}/${totalChunks}`,
                  );
                }
                resolve(response);
              },
              error: (err) => reject(err),
            });
          });
        } catch (error: any) {
          this.messages.push(
            `❌ Erreur: ${error.error?.detail || error.statusText}`,
          );
          this.isUploading = false;
          return;
        }
      }
      this.selectedFile = null;
      this.isUploading = false;
    }
  }
}
