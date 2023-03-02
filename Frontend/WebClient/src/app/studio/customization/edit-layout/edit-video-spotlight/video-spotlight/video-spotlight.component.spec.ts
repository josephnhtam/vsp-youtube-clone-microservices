import {ComponentFixture, TestBed} from '@angular/core/testing';

import {VideoSpotlightComponent} from './video-spotlight.component';

describe('VideoSpotlightComponent', () => {
  let component: VideoSpotlightComponent;
  let fixture: ComponentFixture<VideoSpotlightComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ VideoSpotlightComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(VideoSpotlightComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
