import { Component, Input } from '@angular/core';
import { Member } from '../../../models/member.model';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { environmetn } from '../../../environments/environmet.development';

@Component({
  selector: 'app-gamer-card',
  imports: [
    CommonModule, MatCardModule, MatIconModule
  ],
  templateUrl: './gamer-card.component.html',
  styleUrl: './gamer-card.component.scss'
})
export class GamerCardComponent {
@Input('gamerInput') gamerIn: Member | undefined;
apiUrl = environmetn.apiUrl;
}
