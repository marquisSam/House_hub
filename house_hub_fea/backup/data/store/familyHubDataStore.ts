import { Todo } from '../models/todosModel';
import { patchState, signalStore, type, withComputed, withMethods } from '@ngrx/signals';
import { entityConfig, withEntities } from '@ngrx/signals/entities';

const todoConfig = entityConfig({
  entity: type<Todo>(),
  collection: '_todo',
});

const TodosStore = signalStore(
  withEntities(todoConfig),
  withComputed(({ _todoEntities }) => ({
    // 👇 exposing entity array publicly
    todos: _todoEntities,
  }))
);
