import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, OnInit, effect } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';
import { TodoStore } from '../../app/data';

@Component({
  selector: 'app-todos',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, NgxSpinnerModule],
  templateUrl: './todos.component.html',
  styleUrl: './todos.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [TodoStore],
})
export class TodosComponent implements OnInit {
  private fb = inject(FormBuilder);
  private store = inject(TodoStore);
  private spinner = inject(NgxSpinnerService);

  // Signals from the store
  todos = this.store.todoEntities;
  isLoading = this.store.todoIsLoading;
  error = this.store.error;
  todosCount = this.store.todosCount;

  constructor() {
    // Effect to show/hide spinner based on loading state
    effect(() => {
      if (this.isLoading()) {
        this.spinner.show('todoSpinner', {
          type: 'ball-scale-multiple',
          size: 'medium',
          bdColor: 'rgba(0, 0, 0, 0.8)',
          color: '#3b82f6',
          template:
            '<div class="custom-spinner"><p style="color: white; margin-top: 20px;">Loading todos...</p></div>',
        });
      } else {
        this.spinner.hide('todoSpinner');
      }
    });
  }

  form: FormGroup = this.fb.group({
    Title: ['', Validators.required],
    Description: ['', Validators.required],
    DueDate: ['', Validators.required],
    Priority: [1, [Validators.required, Validators.min(1), Validators.max(5)]],
    Category: ['', Validators.required],
  });

  ngOnInit(): void {
    this.loadTodos();
  }

  loadTodos(): void {
    this.store.loadTodos();
  }

  onSubmit(): void {
    if (this.form.valid) {
      const todoData = this.form.value;
      this.store.createTodo(todoData);
      this.form.reset();
      this.form.patchValue({ Priority: 1 }); // Reset priority to default
    }
  }

  clearError(): void {
    this.store.clearError();
  }
}
