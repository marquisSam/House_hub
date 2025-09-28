import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, input, output } from '@angular/core';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';
import { NzTypographyModule } from 'ng-zorro-antd/typography';
import { Todo, User } from '../../../data';
import { FamilyHubDataStore } from '../../../data/store/familyHubDataStore';
import { UsersUtilService } from '../../../data/services/users-util.service';
import { TodoUtilService } from '../../../data/services/todo-util.service';

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
    NzAvatarModule,
  ],
  templateUrl: './todo-card.component.html',
  styleUrl: './todo-card.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TodoCardComponent {
  private store = inject(FamilyHubDataStore);
  private userUtils = inject(UsersUtilService);
  private todoUtils = inject(TodoUtilService);

  todo = input.required<Todo>();

  // Get assigned users from the store based on AssignedUsers IDs
  assignedUsers = computed(() => {
    const assignedUserIds = this.todo().AssignedUsers || [];
    const allUsers = this.store.userEntities();
    return assignedUserIds
      .map((userId) => allUsers.find((user) => user.Id === userId))
      .filter((user): user is User => user !== undefined);
  });

  // Output events
  editTodo = output<string>();
  toggleComplete = output<string>();
  deleteTodo = output<string>();

  onEdit() {
    this.editTodo.emit(this.todo().Id);
  }

  onToggleComplete() {
    this.toggleComplete.emit(this.todo().Id);
  }

  onDelete() {
    this.deleteTodo.emit(this.todo().Id);
  }

  getPriorityColor = (priority: number): string => {
    return this.todoUtils.getPriorityColor(priority);
  };

  getPriorityText = (priority: number): string => {
    return this.todoUtils.getPriorityText(priority);
  };

  formatDate = (dateString: string | null): string => {
    if (!dateString) return 'Non définie';
    return new Date(dateString).toLocaleDateString('fr-FR');
  };

  getUserInitials = (user: User): string => {
    return this.userUtils.getUserInitials(user);
  };

  getUserFullName = (user: User): string => {
    return this.userUtils.getUserFullName(user);
  };
}
