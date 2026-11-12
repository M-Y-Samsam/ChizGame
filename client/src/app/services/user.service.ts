import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/helpers/apiResponse.model'
import { UserUpdate } from '../models/user-update.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private _http = inject(HttpClient);
  private readonly _apiUrl = environment.apiUrl + 'api/user/';

  updateUser(userInput: UserUpdate): Observable<ApiResponse> {
    return this._http.put<ApiResponse>(this._apiUrl + 'update', userInput);
  }

  deletePhoto(): Observable<ApiResponse> {
    return this._http.delete<ApiResponse>(this._apiUrl + 'delete-photo');
  }
}
