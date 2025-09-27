import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzTypographyModule } from 'ng-zorro-antd/typography';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { FamilyHubDataStore } from '../../data/store/familyHubDataStore';
import { User } from '../../data/models/usersModel';
import { UsersUtilService } from '../../data/services/users-util.service';

@Component({
  selector: 'app-user-select-view',
  standalone: true,
  imports: [
    CommonModule,
    NzAvatarModule,
    NzGridModule,
    NzTypographyModule,
    NzCardModule,
    NzSpinModule,
  ],
  templateUrl: './user-select-view.component.html',
  styleUrl: './user-select-view.component.scss',
})
export class UserSelectViewComponent {
  private store = inject(FamilyHubDataStore);
  private router = inject(Router);
  private userUtil = inject(UsersUtilService);

  constructor() {
    this.store.loadUsers();
  }

  get getUserInitials() {
    return this.userUtil.getUserInitials;
  }
  get getAvatarColor() {
    return this.userUtil.getAvatarColor;
  }

  users = this.store.userEntities;
  isLoading = this.store.userIsLoading;

  onUserSelect(user: User): void {
    this.store.setSelectedUser(user);
    this.router.navigate(['/dashboard']);
  }
}
