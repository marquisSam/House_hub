import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { FamilyHubDataStore } from '../../data/store/familyHubDataStore';

export const userSelectedGuard: CanActivateFn = (_route, _state) => {
  const store = inject(FamilyHubDataStore);
  const router = inject(Router);

  // Check if there's a selected user in the store
  const selectedUser = store.selectedUser();

  if (selectedUser) {
    return true;
  }

  // If no user is selected, redirect to user selection page
  router.navigate(['/users']);
  return false;
};
