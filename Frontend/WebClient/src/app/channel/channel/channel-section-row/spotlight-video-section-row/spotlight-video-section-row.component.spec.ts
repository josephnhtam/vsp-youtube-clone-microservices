import {ComponentFixture, TestBed} from '@angular/core/testing';

import {SpotlightVideoSectionRowComponent} from './spotlight-video-section-row.component';

describe('SpotlightVideoSectionRowComponent', () => {
  let component: SpotlightVideoSectionRowComponent;
  let fixture: ComponentFixture<SpotlightVideoSectionRowComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SpotlightVideoSectionRowComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SpotlightVideoSectionRowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
