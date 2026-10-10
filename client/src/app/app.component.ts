import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { FormBuilder, FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterOutlet, RouterModule, RouterLink  } from '@angular/router';
import { AccountService } from './services/account.service';
import { Login } from './models/login.model';
import { FooterComponent } from "./components/footer/footer.component";
import { HomeComponent } from './components/home/home.component';
import { NavbarComponent } from "./components/navbar/navbar.component";

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
  
  ngOnInit(): void {
    let loggedInUserStr: string | null  = localStorage.getItem('loggedInUser');
    console.log(loggedInUserStr);
    
    if (loggedInUserStr != null){
      this.accountService.authorizeLoggedInUser();

      this.accountService.setCurrentUser(JSON.parse(loggedInUserStr))
    }
  }
}
