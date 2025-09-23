# Todo Store Documentation

## Overview

Le `FamilyHubDataStore` est un signal store utilisant NgRx Signals pour gérer l'état des todos dans l'application.

## Architecture

- **Service Layer**: `TodoService` gère les appels HTTP vers l'API
- **Store Layer**: `FamilyHubDataStore` gère l'état local et les opérations asynchrones
- **Component Layer**: Les composants utilisent directement le store via injection

## Usage

### Dans un composant

```typescript
import { Component, inject } from '@angular/core';
import { FamilyHubDataStore } from '../../app/data';

@Component({
  providers: [FamilyHubDataStore], // Provide le store au niveau du composant
})
export class MyComponent {
  private store = inject(FamilyHubDataStore);

  // Accès aux signals
  todos = this.store.todos;
  isLoading = this.store.isLoading;
  error = this.store.error;

  // Computed signals
  todosCount = this.store.todosCount;
  completedTodos = this.store.completedTodos;
  pendingTodos = this.store.pendingTodos;

  ngOnInit() {
    // Charger les todos
    this.store.loadTodos();
  }

  createTodo(todoData: any) {
    // Créer un nouveau todo
    this.store.createTodo(todoData);
  }

  removeTodo(id: string) {
    // Supprimer un todo
    this.store.removeTodo(id);
  }
}
```

### Dans le template

```html
<!-- Affichage des todos -->
@for (todo of todos(); track todo.Id) {
<div>{{ todo.Title }}</div>
}

<!-- Gestion du loading -->
@if (isLoading()) {
<div>Loading...</div>
}

<!-- Gestion des erreurs -->
@if (error()) {
<div class="error">{{ error() }}</div>
}
```

## Store Methods

### Async Methods (RxMethods)

- `loadTodos()`: Charge tous les todos depuis l'API
- `createTodo(todoData)`: Crée un nouveau todo

### Sync Methods

- `addTodo(todo)`: Ajoute un todo au state local
- `removeTodo(id)`: Supprime un todo du state local
- `updateTodo(id, updates)`: Met à jour un todo local
- `clearError()`: Efface l'erreur courante

## Store State

```typescript
interface TodosState {
  todos: Todo[];
  isLoading: boolean;
  error: string | null;
}
```

## Store Computed

- `todosCount()`: Nombre total de todos
- `hasError()`: Booléen indiquant s'il y a une erreur
- `completedTodos()`: Liste des todos complétés
- `pendingTodos()`: Liste des todos en attente

## Benefits

1. **Réactivité**: Utilise les signals Angular pour une réactivité optimale
2. **Gestion d'état centralisée**: Un seul endroit pour gérer l'état des todos
3. **Gestion d'erreurs**: Gestion automatique des erreurs d'API
4. **Type Safety**: Typé avec TypeScript pour une meilleure DX
5. **Computed Values**: Valeurs calculées automatiquement mises à jour
