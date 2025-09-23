import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Todo } from '../models/todosModel';

@Injectable({
  providedIn: 'root',
})
export class TodoService {
  private http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:5001';

  constructor() {}

  createTodo(todoData: any): Observable<Todo> {
    console.log('Creating todo:', todoData);
    return this.http.post<Todo>(`${this.baseUrl}/api/todos`, todoData);
  }

  getTodos(): Observable<{ value: Todo[] }> {
    return this.http.get<{ value: Todo[] }>(`${this.baseUrl}/odata/Todos`);
  }

  updateTodo(id: string, todoData: Partial<Todo>): Observable<Todo> {
    return this.http.put<Todo>(`${this.baseUrl}/api/todos/${id}`, todoData);
  }

  deleteTodo(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/api/todos/${id}`);
  }
}
