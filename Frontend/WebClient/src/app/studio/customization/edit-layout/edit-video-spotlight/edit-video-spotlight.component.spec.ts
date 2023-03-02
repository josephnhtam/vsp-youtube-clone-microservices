import {ComponentFixture, TestBed} from '@angular/core/testing';

import {EditVideoSpotlightComponent} from './edit-video-spotlight.component';

describe('EditVideoSpotlightComponent', () => {
  let component: EditVideoSpotlightComponent;
  let fixture: ComponentFixture<EditVideoSpotlightComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditVideoSpotlightComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditVideoSpotlightComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
