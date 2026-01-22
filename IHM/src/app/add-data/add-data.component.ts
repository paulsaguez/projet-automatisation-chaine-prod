import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-add-data',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './add-data.component.html',
  styleUrl: './add-data.component.scss',
})
export class AddDataComponent {
  messages: string[] = [];

  onFileSelected(event: any) {
    const file: File = event.target.files[0];
    if (file) {
      this.messages.push(`Fichier sélectionné: ${file.name}`);
      // Upload logic here later
    }
  }
}
