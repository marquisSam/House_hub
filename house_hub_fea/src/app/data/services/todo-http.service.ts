import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable, tap, catchError, map } from 'rxjs';
import { Todo } from '../models/todosModel';

@Injectable({
  providedIn: 'root',
})
export class TodoService {
  private http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:5001';

  constructor() {}

  createTodo(todoData: Partial<Todo>): Observable<Todo> {
    // Ensure the data matches the expected CreateTodosRequest format
    const createRequest = {
      Title: todoData.Title,
      Description: todoData.Description || null,
      DueDate: todoData.DueDate || null,
      Priority: todoData.Priority || 3,
      Category: todoData.Category || null,
    };

    return this.http.post<Todo>(`${this.baseUrl}/odata/Todos`, createRequest);
  }

  getTodos(): Observable<{ value: Todo[] }> {
    return this.http.get<{ value: Todo[] }>(`${this.baseUrl}/odata/Todos`);
  }

  updateTodo(id: string, todoData: Partial<Todo>): Observable<Todo> {
    const url = `${this.baseUrl}/odata/Todos(${id})`;

    return this.http.patch<any>(url, todoData).pipe(
      map((response: any) => {
        // Handle OData response format and clean metadata
        let cleanTodo: Todo;

        if (response && response.value) {
          // OData collection format
          cleanTodo = response.value;
        } else if (response && response.Id) {
          // Direct todo object
          cleanTodo = response;
        } else {
          throw new Error('Invalid server response format');
        }

        // Return clean Todo object without OData metadata
        return {
          Id: cleanTodo.Id,
          Title: cleanTodo.Title,
          Description: cleanTodo.Description,
          IsCompleted: cleanTodo.IsCompleted,
          CreatedAt: cleanTodo.CreatedAt,
          CompletedAt: cleanTodo.CompletedAt,
          DueDate: cleanTodo.DueDate,
          Priority: cleanTodo.Priority,
          Category: cleanTodo.Category,
          UpdatedAt: cleanTodo.UpdatedAt,
        } as Todo;
      })
    );
  }

  deleteTodo(id: string): Observable<string> {
    return this.http.delete<void>(`${this.baseUrl}/odata/Todos(${id})`).pipe(map(() => id));
  }
}
