import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { TodoStore } from '../../data';
import { TodoFormComponent } from '../../forms/todo-form/todo-form.component';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { NzIconModule } from 'ng-zorro-antd/icon';
@Component({
  selector: 'app-todos',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, NzButtonModule, NzIconModule, NzModalModule],
  templateUrl: './todos.component.html',
  styleUrl: './todos.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TodosComponent implements OnInit {
  private fb = inject(FormBuilder);
  private store = inject(TodoStore);
  private modal = inject(NzModalService);

  // Signals from the store
  todos = this.store.todoEntities;
  isLoading = this.store.todoIsLoading;
  error = this.store.error;
  todosCount = this.store.todosCount;

  constructor() {}
  openCreateModal() {
    // Logic to open a modal for creating a new todo
    console.log('Open create todo modal');
    this.modal.create({
      nzTitle: 'Create Todo',
      nzContent: TodoFormComponent,
      nzData: {
        todoId: 'new-todo-' + Date.now(), // Generate a unique ID for new todo
        isEditMode: false,
      },
      nzFooter: null,
      nzWidth: 600,
    });
  }

  openEditModal(todoId: string) {
    // Logic to open a modal for editing an existing todo
    console.log('Open edit todo modal for ID:', todoId);
    const modalref = this.modal.create({
      nzTitle: 'Edit Todo',
      nzContent: TodoFormComponent,

      nzFooter: null,
      nzWidth: 600,
    });
    if (modalref.componentInstance) {
      modalref.componentInstance.todoId.set(todoId);
      console.log('Modal instance created', modalref.componentInstance);
      // Note: This won't work directly with signal inputs because they're readonly
      // We need to use nzData approach instead
    }
    modalref.afterClose.subscribe(() => {
      this.loadTodos();
    });
  }
  ngOnInit(): void {
    this.loadTodos();
  }

  loadTodos(): void {
    this.store.loadTodos();
  }

  clearError(): void {
    this.store.clearError();
  }

  toggleComplete(todoId: string): void {
    const todo = this.store.todoEntityMap()[todoId];
    if (todo) {
      this.store.updateTodo(todoId, {
        IsCompleted: !todo.IsCompleted,
        CompletedAt: !todo.IsCompleted ? new Date().toISOString() : null,
        UpdatedAt: new Date().toISOString(),
      });
    }
  }
}
