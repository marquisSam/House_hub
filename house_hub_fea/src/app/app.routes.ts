import { Routes } from '@angular/router';
import { TodosComponent } from './modules/todos/todos.component';
import { DashboardComponent } from './modules/dashboard/dashboard.component';
import { UsersComponent } from './modules/users/users.component';

export const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: 'users', component: UsersComponent },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'todos', component: TodosComponent },
  { path: '**', redirectTo: '/dashboard' }, // Wildcard route for 404 page
];
