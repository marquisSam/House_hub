import { CommonModule } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
  inject,
  signal,
} from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Todo, TodoStore } from '../../data';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
export enum ModalState {
  edit,
  create,
  view,
}

@Component({
  selector: 'app-todo-form',
  imports: [CommonModule, ReactiveFormsModule, NzButtonModule, NzIconModule, NzInputModule],
  templateUrl: './todo-form.component.html',
  styleUrl: './todo-form.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TodoFormComponent {
  private fb = inject(FormBuilder);
  todoStore = inject(TodoStore);
  constructor() {
    effect(() => {
      if (this.todoEntity()) {
        this.form.patchValue(this.todoEntity()!);
      }
    });
    effect(() => {
      this.isEditMode() ? this.form.enable() : this.form.disable();
    });
  }

  todoId = signal<string>('');
  isEditMode = signal<boolean>(false);
  todoEntity = computed<Todo | null>(() => {
    const todoEntityMap = this.todoStore.todoEntityMap();
    return todoEntityMap[this.todoId()] || null;
  });

  modalState = computed<ModalState>(() => {
    if (this.todoEntity()) {
      if (this.isEditMode()) {
        return ModalState.edit;
      } else {
        return ModalState.view;
      }
    }
    return ModalState.create;
  });

  // ==================== FORM SETUP ====================

  form: FormGroup = this.fb.group({
    Title: ['', Validators.required],
    Description: ['', Validators.required],
    DueDate: [''],
    Priority: [1, [Validators.required, Validators.min(1), Validators.max(5)]],
    Category: [''],
  });

  toggleEditMode() {
    this.isEditMode.update((value) => !value);
  }
  cancelEdit() {
    this.isEditMode.set(false);
    if (this.todoEntity()) {
      this.form.patchValue(this.todoEntity()!);
    } else {
      this.form.reset();
    }
  }
  onSubmit(): void {}
}
