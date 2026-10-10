import { HttpClient } from '@angular/common/http';
import { inject, Injectable, PLATFORM_ID, signal } from '@angular/core';
import { Register } from '../models/register.model';
import { map, Observable } from 'rxjs';
import { LoggedIn } from '../models/logged-in.model';
import { Login } from '../models/login.model';
import { Member } from '../models/member.model';
import { isPlatformBrowser } from '@angular/common';
import { Router } from '@angular/router';
import { environmetn } from '../environments/environmet.development';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  http = inject(HttpClient);
  private readonly _baseApiUrl: string = environmetn.apiUrl + 'api/';
  platformId = inject(PLATFORM_ID);
  router = inject(Router);
  loggedInUserSig = signal<LoggedIn | null>(null);

  register(userInput: Register): Observable<LoggedIn | null> {
    return this.http.post<LoggedIn>(this._baseApiUrl + 'account/register', userInput)
      .pipe(
        map(res => {
          if (res) {
            this.setCurrentUser(res);

            this.router.navigateByUrl('game/games-list');

            return res;
          }

          return null;
        })
      );
  }

  login(userInput: Login): Observable<LoggedIn | null> {
    return this.http.post<LoggedIn>(this._baseApiUrl + 'account/login', userInput)
      .pipe(
        map(res => {
          if (res) {
            this.setCurrentUser(res);

            this.router.navigateByUrl('game/games-list');

            return res;
          }

          return null;
        })
      )
  }

  authorizeLoggedInUser(): void {
    console.log('ok');

    this.http.get<LoggedIn>(this._baseApiUrl + 'account').subscribe({
      next: (res) => {
        if (res)
          console.log(res);
        this.setCurrentUser(res);
      },
      error: (err) => {
        console.log(err.error);
        this.logout();
      }
    });
  }

  setCurrentUser(loggedInUser: LoggedIn): void {
    this.loggedInUserSig.set(loggedInUser);

    if (isPlatformBrowser(this.platformId)) {
      localStorage.setItem('loggedInUser', JSON.stringify(loggedInUser));
    }
  }

  logout(): void {
    this.loggedInUserSig.set(null);

    if (isPlatformBrowser(this.platformId)) {
      localStorage.clear();
    }

    this.router.navigateByUrl('account/login');
  }
}
