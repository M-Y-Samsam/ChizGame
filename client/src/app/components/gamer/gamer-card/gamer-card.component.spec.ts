import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GamerCardComponent } from './gamer-card.component';

describe('GamerCardComponent', () => {
  let component: GamerCardComponent;
  let fixture: ComponentFixture<GamerCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GamerCardComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GamerCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
