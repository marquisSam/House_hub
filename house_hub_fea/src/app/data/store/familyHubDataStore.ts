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
import { TodoService } from '../services/todo-http.service';
import { User, CreateUserRequest, UpdateUserRequest } from '../models/usersModel';
import { UsersService } from '../services/users-http.service';

interface TodoState {
  selectedUser: User | null;
  appIsLoading: boolean;
  todoIsLoading: boolean;
  todoCreationPending: boolean;
  todoUpdatingPending: boolean;
  todoDeletionPending: boolean;
  userIsLoading: boolean;
  userCreationPending: boolean;
  userUpdatingPending: boolean;
  userDeletionPending: boolean;
  error: string | null;
}

interface TodoUpdate {
  id: string;
  updates: Partial<Todo>;
}

interface UserUpdate {
  id: string;
  updates: UpdateUserRequest;
}

const todoConfig = entityConfig({
  entity: type<Todo>(),
  collection: 'todo',
  selectId: (todo) => todo.Id,
});
const userConfig = entityConfig({
  entity: type<User>(),
  collection: 'user',
  selectId: (user) => user.Id,
});

const FamilyHubDataStore = signalStore(
  withEntities(todoConfig),
  withEntities(userConfig),
  withState<TodoState>({
    selectedUser: null,
    appIsLoading: true,
    todoIsLoading: false,
    todoCreationPending: false,
    todoUpdatingPending: false,
    todoDeletionPending: false,
    userIsLoading: false,
    userCreationPending: false,
    userUpdatingPending: false,
    userDeletionPending: false,
    error: null,
  }),
  withMethods((store, todoService = inject(TodoService), usersService = inject(UsersService)) => {
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
      // Todo methods
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
            console.log('Todo created:', newTodo.Title);
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

      // User methods
      loadUsers: createRxMethod<void>('userIsLoading', () =>
        usersService.getUsers().pipe(
          tap(({ value }: { value: User[] }) => {
            patchState(store, setEntities(value, userConfig));
          })
        )
      ),

      addUser: createRxMethod<CreateUserRequest>('userCreationPending', (userData) =>
        usersService.createUser(userData).pipe(
          tap((newUser: User) => {
            patchState(store, addEntities([newUser], userConfig));
          })
        )
      ),

      removeUser: createRxMethod<string>('userDeletionPending', (userId) =>
        usersService.deleteUser(userId).pipe(
          tap(() => {
            patchState(store, removeEntity(userId, userConfig));
          })
        )
      ),

      updateUser: createRxMethod<UserUpdate>('userUpdatingPending', ({ id, updates }) =>
        usersService.updateUser(id, updates).pipe(
          tap((updatedUser: User) => {
            patchState(store, updateEntity({ id, changes: updatedUser }, userConfig));
          })
        )
      ),

      // Search and filter methods
      searchUsersByName: createRxMethod<string>('userIsLoading', (searchTerm) =>
        usersService.searchUsersByName(searchTerm).pipe(
          tap(({ value }: { value: User[] }) => {
            patchState(store, setEntities(value, userConfig));
          })
        )
      ),

      loadActiveUsers: createRxMethod<void>('userIsLoading', () =>
        usersService.getActiveUsers().pipe(
          tap(({ value }: { value: User[] }) => {
            patchState(store, setEntities(value, userConfig));
          })
        )
      ),

      clearError(): void {
        patchState(store, { error: null });
      },

      setSelectedUser(user: User | null): void {
        // Sauver en session
        if (user) {
          sessionStorage.setItem('selectedUser', JSON.stringify(user));
        } else {
          sessionStorage.removeItem('selectedUser');
        }

        patchState(store, { selectedUser: user });
      },

      // Méthode pour charger depuis la session
      loadSelectedUserFromSession(): void {
        try {
          const stored = sessionStorage.getItem('selectedUser');
          if (stored) {
            const user = JSON.parse(stored) as User;
            patchState(store, { selectedUser: user });
          }
        } catch (error) {
          console.warn('Failed to load selected user from session:', error);
          sessionStorage.removeItem('selectedUser');
        }
      },

      setAppLoading(loading: boolean): void {
        patchState(store, { appIsLoading: loading });
      },

      // Méthode d'initialisation pour l'app
      async initializeApp(): Promise<void> {
        // Démarrer le loading de l'app
        patchState(store, { appIsLoading: true });

        try {
          // Charger les utilisateurs actifs et attendre la fin de l'opération
          await new Promise<void>((resolve) => {
            this.loadActiveUsers();

            // Attendre que le loading soit terminé
            const checkLoading = () => {
              if (!store.userIsLoading()) {
                resolve();
              } else {
                setTimeout(checkLoading, 100);
              }
            };

            // Démarrer la vérification après un petit délai pour laisser le loading commencer
            setTimeout(checkLoading, 100);
          });

          console.log('Active users loaded, checking session storage...');

          const stored = sessionStorage.getItem('selectedUser');
          if (stored) {
            const sessionUser = JSON.parse(stored) as User;

            // Maintenant que les utilisateurs actifs sont chargés, vérifier si l'utilisateur existe
            const allUsers = store.userEntities();
            const activeUsers = allUsers.filter((user: User) => user.IsActive);
            const userExists = activeUsers.some((user: User) => user.Id === sessionUser.Id);

            if (userExists) {
              // L'utilisateur existe toujours dans les utilisateurs actifs
              patchState(store, { selectedUser: sessionUser });
              console.log('User loaded from session:', sessionUser.FirstName, sessionUser.LastName);
            } else {
              // L'utilisateur n'existe plus dans les utilisateurs actifs, le supprimer du sessionStorage
              console.log('User from session no longer active, removing from sessionStorage');
              sessionStorage.removeItem('selectedUser');
              patchState(store, { selectedUser: null });
            }
          } else {
            console.log('No user in session, active users loaded');
          }
        } catch (error) {
          console.warn('Failed to initialize app from session:', error);
          sessionStorage.removeItem('selectedUser');
          patchState(store, { selectedUser: null });
        } finally {
          // Arrêter le loading de l'app à la fin
          patchState(store, { appIsLoading: false });
        }
      },
    };
  }),
  withComputed(
    ({
      todoEntities,
      userEntities,
      error,
      appIsLoading,
      todoIsLoading,
      todoCreationPending,
      todoUpdatingPending,
      todoDeletionPending,
      userIsLoading,
      userCreationPending,
      userUpdatingPending,
      userDeletionPending,
    }) => ({
      // Todo computed properties
      todosCount: computed(() => todoEntities().length),
      completedTodos: computed(() => todoEntities().filter((todo: Todo) => todo.IsCompleted)),
      pendingTodos: computed(() => todoEntities().filter((todo: Todo) => !todo.IsCompleted)),

      // User computed properties
      usersCount: computed(() => userEntities().length),
      activeUsers: computed(() => userEntities().filter((user: User) => user.IsActive)),
      inactiveUsers: computed(() => userEntities().filter((user: User) => !user.IsActive)),
      usersWithEmails: computed(() =>
        userEntities().filter((user: User) => user.Email && user.Email.trim() !== '')
      ),

      // General computed properties
      hasError: computed(() => !!error()),
      isAnyTodoOperationPending: computed(
        () =>
          todoIsLoading() || todoCreationPending() || todoUpdatingPending() || todoDeletionPending()
      ),
      isAnyUserOperationPending: computed(
        () =>
          userIsLoading() || userCreationPending() || userUpdatingPending() || userDeletionPending()
      ),
      isAnyOperationPending: computed(
        () =>
          todoIsLoading() ||
          todoCreationPending() ||
          todoUpdatingPending() ||
          todoDeletionPending() ||
          userIsLoading() ||
          userCreationPending() ||
          userUpdatingPending() ||
          userDeletionPending()
      ),
    })
  )
);

export { FamilyHubDataStore };
