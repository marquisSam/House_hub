import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { Todo } from '../models/todosModel';
import { User } from '../models/usersModel';

// Backend response type for Todo with Users navigation property
interface TodoBackendResponse {
  Id: string;
  Title: string;
  Description: string;
  IsCompleted: boolean;
  CreatedAt: string;
  CompletedAt: string | null;
  DueDate: string | null;
  Priority: number;
  Category: string | null;
  UpdatedAt: string;
  Users?: User[];
}

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
      AssignedUserIds: todoData.AssignedUsers || [],
    };

    return this.http.post<TodoBackendResponse>(`${this.baseUrl}/odata/Todos`, createRequest).pipe(
      map((response: TodoBackendResponse) => {
        // Map Users navigation property to AssignedUsers for frontend
        return {
          ...response,
          AssignedUsers: response.Users ? response.Users.map((user: User) => user.Id) : [],
        } as Todo;
      })
    );
  }

  getTodos(): Observable<{ value: Todo[] }> {
    return this.http.get<{ value: TodoBackendResponse[] }>(`${this.baseUrl}/odata/Todos`).pipe(
      map((response) => ({
        value: response.value.map((todo) => ({
          ...todo,
          AssignedUsers: todo.Users ? todo.Users.map((user: User) => user.Id) : [],
        })) as Todo[],
      }))
    );
  }

  updateTodo(id: string, todoData: Partial<Todo>): Observable<Todo> {
    const url = `${this.baseUrl}/odata/Todos(${id})`;

    // Prepare update request with proper field mapping
    const updateRequest: Record<string, unknown> = {
      ...todoData,
      AssignedUserIds: todoData.AssignedUsers,
    };
    // Remove the frontend field to avoid confusion
    delete updateRequest['AssignedUsers'];

    return this.http
      .patch<{ value?: TodoBackendResponse } | TodoBackendResponse>(url, updateRequest)
      .pipe(
        map((response: { value?: TodoBackendResponse } | TodoBackendResponse) => {
          // Handle OData response format and clean metadata
          let cleanTodo: TodoBackendResponse;

          if ('value' in response && response.value) {
            // OData collection format
            cleanTodo = response.value;
          } else if ('Id' in response) {
            // Direct todo object
            cleanTodo = response as TodoBackendResponse;
          } else {
            throw new Error('Invalid server response format');
          }

          // Return clean Todo object with proper mapping
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
            AssignedUsers: cleanTodo.Users ? cleanTodo.Users.map((user: User) => user.Id) : [],
          } as Todo;
        })
      );
  }

  deleteTodo(id: string): Observable<string> {
    return this.http.delete<void>(`${this.baseUrl}/odata/Todos(${id})`).pipe(map(() => id));
  }
}
