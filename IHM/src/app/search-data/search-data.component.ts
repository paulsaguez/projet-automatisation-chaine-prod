import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-search-data',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './search-data.component.html',
  styleUrl: './search-data.component.scss',
})
export class SearchDataComponent {
  // Placeholder for search logic
}
