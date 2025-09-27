import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { FamilyHubDataStore } from '../../data';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzEmptyModule } from 'ng-zorro-antd/empty';
import { NzAlertModule } from 'ng-zorro-antd/alert';
import { NzMessageService } from 'ng-zorro-antd/message';
import { User, CreateUserRequest } from '../../data/models/usersModel';
import { UserCardComponent } from './user-card/user-card.component';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    NzButtonModule,
    NzIconModule,
    NzModalModule,
    NzInputModule,
    NzSpinModule,
    NzEmptyModule,
    NzAlertModule,
    UserCardComponent,
  ],
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UsersComponent implements OnInit {
  private fb = inject(FormBuilder);
  private modal = inject(NzModalService);
  private message = inject(NzMessageService);

  // Inject the data store
  store = inject(FamilyHubDataStore);

  // Search form
  searchForm = this.fb.group({
    searchTerm: [''],
  });

  // Quick create form
  quickCreateForm = this.fb.group({
    firstName: [''],
    lastName: [''],
    email: [''],
  });

  ngOnInit() {
    // Load users when component initializes
    // this.loadUsers();
  }

  loadUsers() {
    this.store.loadUsers();
  }

  loadActiveUsers() {
    this.store.loadActiveUsers();
  }

  searchUsers() {
    const searchTerm = this.searchForm.get('searchTerm')?.value;
    if (searchTerm && searchTerm.trim()) {
      this.store.searchUsersByName(searchTerm.trim());
    } else {
      this.loadUsers();
    }
  }

  clearSearch() {
    this.searchForm.patchValue({ searchTerm: '' });
    this.loadUsers();
  }

  createUser() {
    const formValue = this.quickCreateForm.value;

    if (!formValue.firstName?.trim()) {
      this.message.error('First name is required');
      return;
    }

    const userData: CreateUserRequest = {
      firstName: formValue.firstName.trim(),
      lastName: formValue.lastName?.trim() || null,
      email: formValue.email?.trim() || null,
    };

    this.store.addUser(userData);
    this.quickCreateForm.reset();
    this.message.success('User created successfully!');
  }

  openCreateUserModal() {
    this.modal.create({
      nzTitle: 'Create New User',
      nzContent: this.getCreateUserModalContent(),
      nzFooter: null,
      nzWidth: 500,
    });
  }

  private getCreateUserModalContent() {
    // This could be expanded to use a dedicated form component
    return `
      <div class="user-create-modal">
        <p>Use the quick create form in the main interface or create a dedicated user form component for advanced options.</p>
      </div>
    `;
  }

  onUserUpdate(user: User) {
    // Handle user update from child component
    this.message.success('User updated successfully!');
  }

  onUserDelete(userId: string) {
    this.modal.confirm({
      nzTitle: 'Delete User',
      nzContent: 'Are you sure you want to delete this user? This action cannot be undone.',
      nzOkText: 'Delete',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzOnOk: () => {
        this.store.removeUser(userId);
        this.message.success('User deleted successfully!');
      },
    });
  }

  trackByUserId(index: number, user: User): string {
    return user.Id;
  }
}
