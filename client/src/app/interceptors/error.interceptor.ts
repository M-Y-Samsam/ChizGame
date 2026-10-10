import { isPlatformBrowser } from '@angular/common';
import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject, PLATFORM_ID } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { NavigationExtras, Router, RouterLink } from '@angular/router';
import { catchError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const platformId = inject(PLATFORM_ID);
  const snackBar = inject(MatSnackBar);

  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      if (err) {
        switch (err.status) {
          case 400:
            if (err.error.errors) {
              const modelStateError: String[] = [];
              for (const key in err.error.errors) {
                modelStateError.push(err.error.errors[key]);
              }
              throw modelStateError;
            }
            else { snackBar.open(err.error, 'close', { horizontalPosition: 'center', verticalPosition: 'top', duration: 7000 }); }
            break;

          case 401:
            snackBar.open('Ypu are unauthorized.', 'close', { horizontalPosition: 'center', verticalPosition: 'top', duration: 7000 });

            if (isPlatformBrowser(platformId)) localStorage.clear();

            router.navigateByUrl('account/login');
            break;

          case 403:
            router.navigateByUrl('/no-access');
            break;

          case 404:
            router.navigateByUrl('/not-found');
            break;

          case 500:
            const navigationExtras: NavigationExtras = { state: {error: err.error }};

            router.navigateByUrl('/server-error');
            break;

          default:
            snackBar.open('Something unexpected went wrong.', 'close', {horizontalPosition: 'center', verticalPosition: 'top', duration: 7000 });
            console.log(err);
            break;
        }
      }
      throw err;
    })
  );
};
