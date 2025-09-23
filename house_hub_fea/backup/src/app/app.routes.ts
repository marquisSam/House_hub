import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { TodosComponent } from '../modules/todos/todos.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'todos', component: TodosComponent },
  { path: '**', redirectTo: '' }, // Wildcard route for 404 page
];
