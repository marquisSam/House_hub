import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { inject, computed } from '@angular/core';
import { Todo } from '../models/todosModel';
import {
  patchState,
  signalStore,
  type,
  withComputed,
  withMethods,
  withState,
} from '@ngrx/signals';
import { TodoService } from '../services/todo.service';
import { pipe, switchMap, tap, catchError, EMPTY, finalize, delay } from 'rxjs';
import {
  addEntities,
  entityConfig,
  removeEntity,
  setEntities,
  updateEntity,
  withEntities,
} from '@ngrx/signals/entities';

interface TodoState {
  todoIsLoading: boolean;
  error: string | null;
}

const todoConfig = entityConfig({
  entity: type<Todo>(),
  collection: 'todo',
  selectId: (todo) => todo.Id,
});

const TodoStore = signalStore(
  withEntities(todoConfig),
  withState<TodoState>({
    todoIsLoading: false,
    error: null,
  }),
  withMethods((store, todoService = inject(TodoService)) => ({
    loadTodos: rxMethod<void>(
      pipe(
        tap(() => patchState(store, { todoIsLoading: true, error: null })),
        delay(1000), // Add 1 second delay
        switchMap(() =>
          todoService.getTodos().pipe(
            tap(({ value }: { value: Todo[] }) => {
              patchState(store, setEntities(value, todoConfig));
            }),

            catchError((error) => {
              console.error('Error loading todos:', error);
              patchState(store, {
                error: error.message || 'Failed to load todos',
              });
              return EMPTY;
            }),
            finalize(() => patchState(store, { todoIsLoading: false }))
          )
        )
      )
    ),

    createTodo: rxMethod<any>(
      pipe(
        tap(() => patchState(store, { todoIsLoading: true, error: null })),
        switchMap((todoData) =>
          todoService.createTodo(todoData).pipe(
            tap((newTodo: Todo) => {
              patchState(store, addEntities([newTodo], todoConfig));
            }),
            catchError((error) => {
              console.error('Error creating todo:', error);
              patchState(store, {
                error: error.message || 'Failed to create todo',
              });
              return EMPTY;
            }),
            finalize(() => patchState(store, { todoIsLoading: false }))
          )
        )
      )
    ),

    addTodo(todo: Todo): void {
      patchState(store, addEntities([todo], todoConfig));
    },

    removeTodo(id: string): void {
      patchState(store, removeEntity(id, todoConfig));
    },

    updateTodo(id: string, updates: Partial<Todo>): void {
      patchState(store, updateEntity({ id, changes: updates }, todoConfig));
    },

    clearError(): void {
      patchState(store, { error: null });
    },
  })),
  withComputed(({ todoEntities, error }) => ({
    todosCount: computed(() => todoEntities().length),
    hasError: computed(() => !!error()),
    completedTodos: computed(() =>
      todoEntities().filter((todo: Todo) => todo.IsCompleted)
    ),
    pendingTodos: computed(() =>
      todoEntities().filter((todo: Todo) => !todo.IsCompleted)
    ),
  }))
);

export { TodoStore };
