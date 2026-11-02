import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { FormBuilder, FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterOutlet, RouterModule, RouterLink, Router, NavigationEnd } from '@angular/router';
import { AccountService } from './services/account.service';
import { Login } from './models/login.model';
import { FooterComponent } from "./components/footer/footer.component";
import { HomeComponent } from './components/home/home.component';
import { NavbarComponent } from "./components/navbar/navbar.component";
import { every, filter } from 'rxjs';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    FormsModule, ReactiveFormsModule,
    MatButtonModule, RouterModule, RouterLink,
    HomeComponent,
    FooterComponent,
    NavbarComponent
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  accountService = inject(AccountService);
  showFooter = true;

  ngOnInit(): void {
    let loggedInUserStr: string | null = localStorage.getItem('loggedInUser');
    console.log(loggedInUserStr);

    if (loggedInUserStr != null) {
      this.accountService.authorizeLoggedInUser();

      this.accountService.setCurrentUser(JSON.parse(loggedInUserStr))
    }
  }

  constructor(private router: Router) {
    this.router.events
      .pipe(
        filter(event => event instanceof NavigationEnd)
      )
      .subscribe((event: NavigationEnd) => {
        this.showFooter = ![
          '/account/login',
          '/account/register'
        ].includes(event.urlAfterRedirects);
      });
  }
}
