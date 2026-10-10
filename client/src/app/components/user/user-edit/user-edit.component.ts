import { CommonModule, isPlatformBrowser } from '@angular/common';
import { Component, inject, OnInit, PLATFORM_ID } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatTabsModule } from '@angular/material/tabs';
import { PhotoEditorComponent } from '../photo-editor/photo-editor.component';
import { Member } from '../../../models/member.model';
import { LoggedIn } from '../../../models/logged-in.model';
import { MemberService } from '../../../services/member.service';
import { max, take } from 'rxjs';
import { MatButtonModule } from '@angular/material/button';
import { environmetn } from '../../../environments/environmet.development';
import { AbstractControl, FormBuilder, FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { UserService } from '../../../services/user.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UserUpdate } from '../../../models/user-update.model';
import { ApiResponse } from '../../../models/helpers/apiResponse.model';

@Component({
  selector: 'app-user-edit',
  standalone: true,
  imports: [
    CommonModule, FormsModule, ReactiveFormsModule,
    MatCardModule, MatTabsModule, CommonModule, PhotoEditorComponent, MatButtonModule,
    MatFormFieldModule, MatInputModule,
  ],
  templateUrl: './user-edit.component.html',
  styleUrl: './user-edit.component.scss'
})
export class UserEditComponent implements OnInit {
  private _platformId = inject(PLATFORM_ID);
  private _memberService = inject(MemberService);
  private _userService = inject(UserService);
  private _snackBar = inject(MatSnackBar);
  private _fB = inject(FormBuilder);

  readonly maxInChars = 30;
  readonly minInChars = 2;

  apiUrl = environmetn.apiUrl;
  member: Member | undefined;

  userEditFg = this._fB.group({
    nameCtrl: ['', [Validators.required, Validators.minLength(this.minInChars), Validators.maxLength((this.maxInChars))]],
    countryCtrl: ['', [Validators.minLength(this.minInChars), Validators.maxLength(this.maxInChars)]],
    cityCtrl: ['', [Validators.minLength(this.minInChars), Validators.maxLength(this.maxInChars)]]
  });

  ngOnInit(): void {

  }

  getMember(): void {
    if (isPlatformBrowser(this._platformId)) {
      const loggedInUserStr: string | null = localStorage.getItem('loggedInUser');

      if (loggedInUserStr) {
        const loggedInUser: LoggedIn = JSON.parse(loggedInUserStr);

        this._memberService.getByUserName(loggedInUser.userName).pipe(take(1)).subscribe({
          next: (res) => {
            this.member = res;

            this.initialControllerValue(res!);
          }
        })
      }
    }
  }

  get NameCtrl(): FormControl {
    return this.userEditFg.get("nameCtrl") as FormControl;
  }

  get CountryCtrl(): FormControl {
    return this.userEditFg.get("countryCtrl") as FormControl;
  }

  get CityCtrl(): FormControl {
    return this.userEditFg.get("cityCtrl") as FormControl;
  }

  initialControllerValue(member: Member) {
    this.NameCtrl.setValue(member.name);
    this.CountryCtrl.setValue(member.country.toUpperCase());
    this.CityCtrl.setValue(member.city.toUpperCase());
  }

  updateUser(): void {
    if (this.member) {
      let updateUser: UserUpdate = {
        name: this.NameCtrl.value,
        country: this.CountryCtrl.value,
        city: this.CityCtrl.value
      }

      this._userService.updateUser(updateUser).pipe(take(1)).subscribe({
        next: (res: ApiResponse) => {
          if (res.message) {
            this._snackBar.open(res.message, 'close', {
              horizontalPosition: 'center',
              verticalPosition: 'top',
              duration: 7000
            })
          }
        }
      })
    }
  }
}
