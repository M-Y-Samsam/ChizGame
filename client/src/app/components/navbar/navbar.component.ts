import { Component, inject } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { RouterLink, RouterModule } from '@angular/router';
import { AccountService } from '../../services/account.service';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';
import { MatListModule } from '@angular/material/list';
import { environmetn } from '../../environments/environmet.development';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink, RouterModule, CommonModule,
    MatButtonModule, MatToolbarModule,
    MatIconModule, MatMenuModule,
    MatDividerModule, MatListModule
  ],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent {
  accountService = inject(AccountService);
  apiUrl = environmetn.apiUrl;

  logOut(): void {
    this.accountService.logout();
  }
}
