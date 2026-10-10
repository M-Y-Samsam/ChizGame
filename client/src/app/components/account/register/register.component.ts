import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { AccountService } from '../../../services/account.service';
import { FormBuilder, FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Register } from '../../../models/register.model';
import { RouterLink } from '@angular/router';
import { Observable, Subscription } from 'rxjs';
import { LoggedIn } from '../../../models/logged-in.model';
import { MatFormField, MatLabel, MatHint, MatSuffix, MatError } from "@angular/material/form-field";
import { MatDatepickerModule } from '@angular/material/datepicker';
import { HttpErrorResponse } from '@angular/common/http';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-register',
  imports: [RouterLink, FormsModule, ReactiveFormsModule, MatFormField, MatLabel, MatDatepickerModule, MatHint, MatSuffix, MatInputModule, MatError, MatButtonModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent implements OnInit, OnDestroy {
  accountService = inject(AccountService);
  fB = inject(FormBuilder);
  subscribedRegisterUser: Subscription | undefined;
  errors: string[] | undefined;

  minDate = new Date();
  maxDate = new Date();

  passwordNotMatch: boolean | undefined;

  ngOnInit(): void {
    const currentYear = new Date().getFullYear();
    this.minDate = new Date(currentYear - 99, 0, 1);
    this.maxDate = new Date(currentYear - 12, 0, 1);
  }

  ngOnDestroy(): void {
    this.subscribedRegisterUser?.unsubscribe();
  }

  registerFg = this.fB.group({
    nameCtrl: '',
    emailCtrl: ['', [Validators.required, Validators.email]],
    genderCtrl: 'male',
    dateOfBirthCtrl: '',
    passwordCtrl: '',
    confirmPasswordCtrl: ''
  });

  get NameCtrl(): FormControl {
    return this.registerFg.get('nameCtrl') as FormControl
  };

  get EmailCtrl(): FormControl {
    return this.registerFg.get('emailCtrl') as FormControl
  };

  get GenderCtrl(): FormControl {
    return this.registerFg.get('genderCtrl') as FormControl
  };

  get DateOfBirthCtrl(): FormControl {
    return this.registerFg.get('dateOfBirthCtrl') as FormControl
  };

  get PasswordCtrl(): FormControl {
    return this.registerFg.get('passwordCtrl') as FormControl
  };

  get ConfirmPasswordCtrl(): FormControl {
    return this.registerFg.get('confirmPasswordCtrl') as FormControl
  };

  register(): void {
    const dob: string | undefined = this.getDateOnly(this.DateOfBirthCtrl.value);

    if (this.PasswordCtrl.value === this.ConfirmPasswordCtrl.value) {
      let gamer: Register = {
        name: this.NameCtrl.value,
        email: this.EmailCtrl.value,
        gender: this.GenderCtrl.value,
        dateOfBirth: dob,
        password: this.PasswordCtrl.value,
        confirmPassword: this.ConfirmPasswordCtrl.value
      }

      this.subscribedRegisterUser  = this.accountService.register(gamer).subscribe({
        next: (res) => 
          console.log(res),
        
        error: (err : HttpErrorResponse) => {
          if ( err.error.errors ) { this.errors = err.error.errors }
        }
      });
    }
    else {
      this.passwordNotMatch = true;
    }
  }

  getDateOnly(dob: string | null): string | undefined {
    if (!dob) return undefined;

    let theDob: Date = new Date(dob);
    return new Date(theDob.setMinutes(theDob.getMinutes() - theDob.getTimezoneOffset())).toISOString().slice(0, 10);
  }
} 