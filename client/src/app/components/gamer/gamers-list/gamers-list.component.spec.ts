import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GamersListComponent } from './gamers-list.component';

describe('GamersListComponent', () => {
  let component: GamersListComponent;
  let fixture: ComponentFixture<GamersListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GamersListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GamersListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
