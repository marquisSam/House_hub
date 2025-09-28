import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class TodoUtilService {
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
}
