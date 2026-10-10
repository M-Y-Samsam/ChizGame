import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Register } from '../models/register.model';
import { Observable } from 'rxjs';
import { LoggedIn } from '../models/logged-in.model';
import { Login } from '../models/login.model';
import { Member } from '../models/member.model';
import { environmetn } from '../environments/environmet.development';

@Injectable({
  providedIn: 'root'
})
export class MemberService {
  http = inject(HttpClient);
  private readonly _baseApiUrl: string = environmetn.apiUrl + 'api/member/';

  GetAllGamers(): Observable<Member[]> {
    let member$: Observable<Member[]> = this.http.get<Member[]>(this._baseApiUrl + 'get-all');
    
    return member$;
  }

  getByUserName(userNameInput: string): Observable<Member | undefined>{
    return this.http.get<Member>(this._baseApiUrl + 'get-by-name/' + userNameInput);
  }
}
