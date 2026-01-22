import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HttpClient, HttpParams } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-search-data',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './search-data.component.html',
  styleUrl: './search-data.component.scss',
})
export class SearchDataComponent {
  // Filters
  filters: any = {
    title: '',
    status: '',
    source: '',
    destination: '',
    type: '',
    migrationAction: '',
    subJobId: '',
    sourceId: '',
    destinationId: '',
    errorCode: '',
    migrationStartTime: '',
  };

  results: any[] = [];
  isSearching: boolean = false;
  hasSearched: boolean = false;

  constructor(private http: HttpClient) {}

  search() {
    this.isSearching = true;
    this.hasSearched = true;

    let params = new HttpParams();
    Object.keys(this.filters).forEach((key) => {
      if (this.filters[key]) {
        params = params.set(key, this.filters[key]);
      }
    });

    this.http.get<any[]>('/api/data/migration/search', { params }).subscribe({
      next: (data) => {
        this.results = data;
        this.isSearching = false;
      },
      error: (error) => {
        console.error('Search error', error);
        this.isSearching = false;
      },
    });
  }

  clearFilters() {
    this.filters = {
      title: '',
      status: '',
      source: '',
      destination: '',
      type: '',
      migrationAction: '',
      subJobId: '',
      sourceId: '',
      destinationId: '',
      errorCode: '',
      migrationStartTime: '',
    };
    this.results = [];
    this.hasSearched = false;
  }
}
