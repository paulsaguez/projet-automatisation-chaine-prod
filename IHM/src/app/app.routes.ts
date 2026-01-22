import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { SearchDataComponent } from './search-data/search-data.component';
import { AddDataComponent } from './add-data/add-data.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'search', component: SearchDataComponent },
  { path: 'add', component: AddDataComponent },
];
