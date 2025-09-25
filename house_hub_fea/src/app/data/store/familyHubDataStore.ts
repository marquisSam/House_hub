import { computed, inject } from '@angular/core';
import { patchState, signalStore, type, withComputed, withMethods, withState } from '@ngrx/signals';
import {
  addEntities,
  entityConfig,
  removeEntity,
  setEntities,
  updateEntity,
  withEntities,
} from '@ngrx/signals/entities';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { catchError, delay, EMPTY, Observable, pipe, switchMap, tap } from 'rxjs';
import { Todo } from '../models/todosModel';
import { TodoService } from '../services/todo.service';

interface TodoState {
  todoIsLoading: boolean;
  todoCreationPending: boolean;
  todoUpdatingPending: boolean;
  todoDeletionPending: boolean;
  error: string | null;
}

interface TodoUpdate {
  id: string;
  updates: Partial<Todo>;
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
    todoCreationPending: false,
    todoUpdatingPending: false,
    todoDeletionPending: false,
    error: null,
  }),
  withMethods((store, todoService = inject(TodoService)) => {
    // Helper function to handle common error and loading patterns
    const createRxMethod = <T>(
      loadingKey: keyof TodoState,
      operation: (data: T) => Observable<any>
    ) => {
      return rxMethod<T>(
        pipe(
          tap(() => patchState(store, { [loadingKey]: true, error: null } as Partial<TodoState>)),
          delay(500),
          switchMap(operation),
          tap(() => patchState(store, { [loadingKey]: false } as Partial<TodoState>)),
          catchError((error) => {
            patchState(store, { [loadingKey]: false } as Partial<TodoState>);
            patchState(store, { error: error.message || 'Operation failed' });
            return EMPTY;
          })
        )
      );
    };

    return {
      loadTodos: createRxMethod<void>('todoIsLoading', () =>
        todoService.getTodos().pipe(
          tap(({ value }: { value: Todo[] }) => {
            patchState(store, setEntities(value, todoConfig));
          })
        )
      ),

      addTodo: createRxMethod<Partial<Todo>>('todoCreationPending', (todoData) =>
        todoService.createTodo(todoData).pipe(
          tap((newTodo: Todo) => {
            console;
            patchState(store, addEntities([newTodo], todoConfig));
          })
        )
      ),

      removeTodo: createRxMethod<string>('todoDeletionPending', (todoId) =>
        todoService.deleteTodo(todoId).pipe(
          tap(() => {
            patchState(store, removeEntity(todoId, todoConfig));
          })
        )
      ),

      updateTodo: createRxMethod<TodoUpdate>('todoUpdatingPending', ({ id, updates }) =>
        todoService.updateTodo(id, updates).pipe(
          tap((updatedTodo: Todo) => {
            patchState(store, updateEntity({ id, changes: updatedTodo }, todoConfig));
          })
        )
      ),

      clearError(): void {
        patchState(store, { error: null });
      },
    };
  }),
  withComputed(
    ({
      todoEntities,
      error,
      todoIsLoading,
      todoCreationPending,
      todoUpdatingPending,
      todoDeletionPending,
    }) => ({
      todosCount: computed(() => todoEntities().length),
      hasError: computed(() => !!error()),
      isAnyOperationPending: computed(
        () =>
          todoIsLoading() || todoCreationPending() || todoUpdatingPending() || todoDeletionPending()
      ),
      completedTodos: computed(() => todoEntities().filter((todo: Todo) => todo.IsCompleted)),
      pendingTodos: computed(() => todoEntities().filter((todo: Todo) => !todo.IsCompleted)),
    })
  )
);

export { TodoStore };
