import { Component } from '@angular/core';
import { GamerCardComponent } from '../gamer-card/gamer-card.component';

@Component({
  selector: 'app-gamers-list',
  imports: [GamerCardComponent],
  templateUrl: './gamers-list.component.html',
  styleUrl: './gamers-list.component.scss'
})
export class GamersListComponent {

}
