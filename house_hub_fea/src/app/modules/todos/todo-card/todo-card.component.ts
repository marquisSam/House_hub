import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, input, output, effect } from '@angular/core';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzTypographyModule } from 'ng-zorro-antd/typography';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';
import { Todo } from '../../../data';

@Component({
  selector: 'app-todo-card',
  imports: [
    CommonModule,
    NzCardModule,
    NzButtonModule,
    NzIconModule,
    NzTagModule,
    NzTypographyModule,
    NzToolTipModule,
  ],
  templateUrl: './todo-card.component.html',
  styleUrl: './todo-card.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TodoCardComponent {
  todo = input.required<Todo>();

  // Output events
  editTodo = output<string>();
  toggleComplete = output<string>();
  deleteTodo = output<string>();

  constructor() {
    // Debug effect to track todo changes
    effect(() => {
      console.log('TodoCard: Todo changed:', this.todo());
    });
  }
  onEdit() {
    this.editTodo.emit(this.todo().Id);
  }

  onToggleComplete() {
    this.toggleComplete.emit(this.todo().Id);
  }

  onDelete() {
    this.deleteTodo.emit(this.todo().Id);
  }

  getPriorityColor(priority: number): string {
    switch (priority) {
      case 1:
        return 'red'; // High
      case 2:
        return 'orange'; // Medium-High
      case 3:
        return 'blue'; // Medium
      case 4:
        return 'cyan'; // Medium-Low
      case 5:
        return 'green'; // Low
      default:
        return 'default';
    }
  }

  getPriorityText(priority: number): string {
    switch (priority) {
      case 1:
        return 'Élevée';
      case 2:
        return 'Moyenne-Élevée';
      case 3:
        return 'Moyenne';
      case 4:
        return 'Moyenne-Faible';
      case 5:
        return 'Faible';
      default:
        return 'Non définie';
    }
  }

  formatDate(dateString: string | null): string {
    if (!dateString) return 'Non définie';
    return new Date(dateString).toLocaleDateString('fr-FR');
  }
}
