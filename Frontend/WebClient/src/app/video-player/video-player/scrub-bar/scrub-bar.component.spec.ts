import {ComponentFixture, TestBed} from '@angular/core/testing';

import {ScrubBarComponent} from './scrub-bar.component';

describe('ScrubBarComponent', () => {
  let component: ScrubBarComponent;
  let fixture: ComponentFixture<ScrubBarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ScrubBarComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ScrubBarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
