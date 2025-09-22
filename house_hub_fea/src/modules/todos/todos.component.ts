import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-todos',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './todos.component.html',
  styleUrl: './todos.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TodosComponent implements OnInit {
  private http = inject(HttpClient);
  private fb = inject(FormBuilder);

  todos = signal<any[]>([]);

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
    this.http.get('http://localhost:5001/odata/Todos').subscribe({
      next: (data: any) => {
        console.log('Todos loaded:', data);
        this.todos.set(data.value || data);
      },
      error: (error: any) => {
        console.error('Error loading todos:', error);
      },
    });
  }

  onSubmit(): void {
    if (this.form.valid) {
      const todoData = this.form.value;

      this.http.post('http://localhost:5001/api/todos', todoData).subscribe({
        next: (response) => {
          console.log('Todo created:', response);
          this.form.reset();
          this.form.patchValue({ Priority: 1 }); // Reset priority to default
          this.loadTodos(); // Reload the todo list
        },
        error: (error) => {
          console.error('Error creating todo:', error);
        },
      });
    }
  }
}
