import { Routes } from '@angular/router';
import { TodosComponent } from './modules/todos/todos.component';
import { DashboardComponent } from './modules/dashboard/dashboard.component';
import { HomeLayoutComponent } from './home-layout/home-layout.component';
import { PrimengTestComponent } from './components/primeng-test/primeng-test.component';

export const routes: Routes = [
  {
    path: '',
    component: HomeLayoutComponent,
    children: [
      { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: DashboardComponent },
      { path: 'todos', component: TodosComponent },
      { path: 'primeng-test', component: PrimengTestComponent },
    ],
  },
  { path: '**', redirectTo: '' }, // Wildcard route for 404 page
];
