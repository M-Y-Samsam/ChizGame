import { isPlatformBrowser } from '@angular/common';
import { inject, PLATFORM_ID } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CanActivateFn, Router } from '@angular/router';

export const authGuard: CanActivateFn = (route, state) => {
  const platformId = inject(PLATFORM_ID);
  const router = inject(Router);
  const snackbar = inject(MatSnackBar);

  if (isPlatformBrowser(platformId)) {
    const loggedInUserSt: String | null = localStorage.getItem('loggedInUser');

    if (loggedInUserSt) return true; // open the component

    snackbar.open('please login first', 'close', {
      verticalPosition: 'top',
      horizontalPosition: 'center',
      duration: 7000
    });

    router.navigateByUrl('/account/login');
  }

  return false; // block the component
};
