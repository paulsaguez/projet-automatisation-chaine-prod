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

  onUpload() {
    if (this.selectedFile && !this.isUploading) {
      this.isUploading = true;
      this.messages.push(`Upload en cours: ${this.selectedFile.name}...`);

      const formData = new FormData();
      formData.append('file', this.selectedFile);

      this.http.post('/api/traitement/upload', formData).subscribe({
        next: (response: any) => {
          this.messages.push(`✅ Fichier uploadé avec succès: ${response.filename}`);
          this.selectedFile = null;
          this.isUploading = false;
        },
        error: (error) => {
          this.messages.push(`❌ Erreur lors de l'upload: ${error.error?.message || error.statusText}`);
          this.isUploading = false;
        }
      });
    }
  }
}
