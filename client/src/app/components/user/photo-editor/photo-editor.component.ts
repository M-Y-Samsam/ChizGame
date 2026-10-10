import { Component, inject, Input, OnInit } from '@angular/core';
import { Member } from '../../../models/member.model';
import { LoggedIn } from '../../../models/logged-in.model';
import { environmetn } from '../../../environments/environmet.development';
import { FileUploader, FileUploadModule } from 'ng2-file-upload';
import { AccountService } from '../../../services/account.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { Photo } from '../../../models/photo.model';
import { UserService } from '../../../services/user.service';
import { ApiResponse } from '../../../models/helpers/apiResponse.model';
import { take } from 'rxjs';

@Component({
  selector: 'app-photo-editor',
  imports: [
    CommonModule,
    MatFormFieldModule, MatInputModule, MatCardModule, MatIconModule, FileUploadModule, MatButtonModule
  ],
  templateUrl: './photo-editor.component.html',
  styleUrl: './photo-editor.component.scss'
})
export class PhotoEditorComponent implements OnInit {
  @Input('memberInput') member: Member | undefined;
  loggedInUser: LoggedIn | null | undefined;
  apiUrl: string = environmetn.apiUrl;
  uploader: FileUploader | undefined;
  hasBaseDropZoneOver = false;
  private _accountService = inject(AccountService);
  private _userService = inject(UserService);
  private _snackBar = inject(MatSnackBar);

  ngOnInit(): void {
    this.loggedInUser = this._accountService.loggedInUserSig();

    this.initializeUploader();
  }

  fileOverBase(event: boolean): void {
    this.hasBaseDropZoneOver = event
  }

  initializeUploader(): void {
    if (this.loggedInUser) {
      this.uploader = new FileUploader({
        url: this.apiUrl + 'api/user/upload-photo',
        authToken: 'Bearer' + this.loggedInUser.token,
        isHTML5: true,
        allowedFileType: ['image'],
        removeAfterUpload: true,
        autoUpload: false,
        maxFileSize: 4_000_000
      })

      this.uploader.onAfterAddingFile = (file) => { file.withCredentials = false }

      this.uploader.onSuccessItem = (item, response) => {
        if (response && this.member) {
          const photo: Photo = JSON.parse(response);
          this.member.photo = photo;

          this.setNavbarProfilePhoto(photo.url_165);
        }
      }
    }
  }

  setNavbarProfilePhoto(url_165: string): void {
    if (this.loggedInUser) {
      this.loggedInUser.profilePhotoUrl = url_165;

      this._accountService.loggedInUserSig.set(this.loggedInUser);
    }
  }

  deletePhoto(): void {
    this._userService.deletePhoto().pipe(take(1)).subscribe({
      next: (response: ApiResponse) => {
        if (response && this.member) {
          this.member.photo = null;

          this._snackBar.open(response.message, 'close', {
            horizontalPosition: 'center',
            verticalPosition: 'top',
            duration: 7000
          })
        }
      }
    })
  }
}
