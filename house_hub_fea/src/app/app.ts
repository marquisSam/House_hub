import { Component, inject, signal, OnInit } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { NzTooltipModule } from 'ng-zorro-antd/tooltip';
import { FamilyHubDataStore } from './data';
import { UserSelectViewComponent } from './components/user-select-view/user-select-view.component';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { UsersUtilService } from './data/services/users-util.service';
import { User } from './data/models/usersModel';
import { NzPopoverModule } from 'ng-zorro-antd/popover';
@Component({
  selector: 'app-root',
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    NzButtonModule,
    NzIconModule,
    NzAvatarModule,
    NzMenuModule,
    NzTooltipModule,
    NzPopoverModule,
    UserSelectViewComponent,
  ],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App implements OnInit {
  store = inject(FamilyHubDataStore);
  userUtil = inject(UsersUtilService);

  async ngOnInit(): Promise<void> {
    await this.store.initializeApp();
  }

  getCurrentUserInitials(): string {
    const user = this.currentUser();
    return user ? this.userUtil.getUserInitials(user) : '';
  }

  getAvatarColor(user: User): string {
    return this.userUtil.getAvatarColor(user);
  }

  currentUser = this.store.selectedUser;

  isCollapsed = signal(true);
  protected readonly title = signal('house_hub_fea');

  toggleCollapsed() {
    this.isCollapsed.update((value) => !value);
  }
  disconnectUser(navigateToUserSelect: boolean = true) {
    this.store.setSelectedUser(null);
    if (navigateToUserSelect) {
      // Navigate to user selection view
      window.location.href = '/'; // Simple navigation to root
    }
  }
}
