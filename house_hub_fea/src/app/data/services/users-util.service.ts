import { Injectable } from '@angular/core';
import { User } from '../models/usersModel';

@Injectable({
  providedIn: 'root',
})
export class UsersUtilService {
  getUserInitials(user: User): string {
    const firstInitial = user.FirstName?.charAt(0)?.toUpperCase() || '';
    const lastInitial = user.LastName?.charAt(0)?.toUpperCase() || '';
    return `${firstInitial}${lastInitial}`;
  }

  getAvatarColor(user: User): string {
    // Generate a color based on user ID
    const colors = [
      '#f56565',
      '#ed8936',
      '#ecc94b',
      '#48bb78',
      '#38b2ac',
      '#4299e1',
      '#667eea',
      '#9f7aea',
      '#ed64a6',
      '#fc8181',
    ];
    const index = parseInt(user.Id, 36) % colors.length;
    return colors[index];
  }
  getUserFullName(user: User): string {
    return `${user.FirstName} ${user.LastName || ''}`.trim();
  }
}
