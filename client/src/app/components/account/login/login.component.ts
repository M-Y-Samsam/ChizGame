import { Component, inject } from '@angular/core';
import { AccountService } from '../../../services/account.service';
import { FormBuilder, FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Login } from '../../../models/login.model';
import { LoggedIn } from '../../../models/logged-in.model';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { Observable } from 'rxjs';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-login',
  imports: [
    RouterLink,
    FormsModule, ReactiveFormsModule,
    MatButtonModule, MatFormField,
    MatLabel,MatInputModule
],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  accountService = inject(AccountService);
  fB = inject(FormBuilder);
  loggedInRes : LoggedIn | undefined | null ;

  loginFg = this.fB.group({
    nameCtrl: ['', Validators.required],
    passwordCtrl: ['']
  })

  get NameCtrll(): FormControl {
    return this.loginFg.get('nameCtrl') as FormControl;
  }

  get PasswordCtrl(): FormControl {
    return this.loginFg.get('passwordCtrl') as FormControl;
  }

  login(): void {
    let gamer: Login = {
      name: this.NameCtrll.value,
      password: this.PasswordCtrl.value
    }

    let loginResponse$: Observable<LoggedIn | null> = this.accountService.login(gamer);

    loginResponse$.subscribe({
      next: (res => {
        console.log(res);
        this.loggedInRes = res;
      }),
      error: (err => {
        console.log(err.error);
      })
    });
  }
} 