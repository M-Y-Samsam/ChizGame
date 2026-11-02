
import { Component } from '@angular/core';
import { ActivatedRoute, Router, RouteReuseStrategy, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';

import { RegisterComponent } from '../register/register.component';
import { LoginComponent } from '../login/login.component';

@Component({
  selector: 'app-auth',
  standalone: true,
  imports: [
    RouterLink,
    MatButtonModule,
    RegisterComponent,
    LoginComponent
  ],
  templateUrl: './auth.component.html',
  styleUrl: './auth.component.scss'
})
export class AuthComponent {
  activeTab: 'login' | 'register' = 'login';

  constructor(private router: Router) {

    if (router.url == '/account/register') {
      this.activeTab = 'register'
    }
    else {
      this.activeTab = 'login'
    }
  }
}
