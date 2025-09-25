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
import { NzButtonModule, NzButtonType } from 'ng-zorro-antd/button';
import { NzSizeLDSType } from 'ng-zorro-antd/core/types';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NZ_MODAL_DATA } from 'ng-zorro-antd/modal';
import { Todo, TodoStore } from '../../data';
import {
  ActionBtnBarComponent,
  familyHubButtonConfig,
} from '../../components/action-btn-bar/action-btn-bar.component';
export enum ModalState {
  edit,
  create,
  view,
}

@Component({
  selector: 'app-todo-form',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    NzButtonModule,
    NzIconModule,
    NzInputModule,
    ActionBtnBarComponent,
  ],
  templateUrl: './todo-form.component.html',
  styleUrl: './todo-form.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TodoFormComponent {
  private fb = inject(FormBuilder);
  todoStore = inject(TodoStore);
  private modalData = inject(NZ_MODAL_DATA, { optional: true });

  constructor() {
    // Initialize signals from modal data if available
    if (this.modalData) {
      this.todoId.set(this.modalData.todoId || '');
      this.isEditMode.set(this.modalData.isEditMode || false);
    }

    effect(() => {
      if (this.todoEntity()) {
        this.form.patchValue(this.todoEntity()!);
      }
    });
    effect(() => {
      if (this.todoEntity()) {
        this.isEditMode() ? this.form.enable() : this.form.disable();
      }
    });
  }

  get modalState() {
    return ModalState;
  }
  todoId = signal<string>('');
  isEditMode = signal<boolean>(false);
  todoEntity = computed<Todo | null>(() => {
    const todoEntityMap = this.todoStore.todoEntityMap();
    return todoEntityMap[this.todoId()] || null;
  });

  modalMode = computed<ModalState>(() => {
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

  buttonConfig = computed<familyHubButtonConfig[]>(() => {
    const currentMode = this.modalMode();

    return [
      {
        text: 'Annuler',
        type: 'default' as const,
        size: 'large' as const,
        action: () => this.cancelEdit(),
        visible: currentMode === ModalState.edit,
        icon: undefined,
      },
      {
        text: 'Sauvegarder',
        icon: 'save',
        type: 'primary' as const,
        size: 'large' as const,
        action: () => this.onPatch(),
        visible: currentMode === ModalState.edit,
      },
      {
        text: 'Éditer ce todo',
        icon: 'form',
        type: 'primary' as const,
        size: 'large' as const,
        action: () => this.toggleEditMode(),
        visible: currentMode === ModalState.view,
      },
      {
        text: 'Créer un nouveau todo',
        icon: 'form',
        type: 'primary' as const,
        size: 'large' as const,
        action: () => this.onCreate(),
        visible: currentMode === ModalState.create,
      },
    ].filter((btn) => btn.visible);
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
  onPatch(): void {
    if (this.form.valid && this.todoEntity()) {
      const formValue = this.form.value;
      // Update existing todo
      this.todoStore.updateTodo({
        id: this.todoEntity()!.Id,
        updates: {
          ...formValue,
          UpdatedAt: new Date().toISOString(),
        },
      });
      this.isEditMode.set(false);
    }
  }
  onCreate(): void {
    if (this.form.valid) {
      const formValue = this.form.value;

      this.todoStore.addTodo({
        ...formValue,
        IsCompleted: false,
        CreatedAt: new Date().toISOString(),
        UpdatedAt: new Date().toISOString(),
      });
    }
  }
}
