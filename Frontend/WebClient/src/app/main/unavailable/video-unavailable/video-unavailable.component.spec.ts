import {ComponentFixture, TestBed} from '@angular/core/testing';

import {VideoUnavailableComponent} from './video-unavailable.component';

describe('VideoUnavailableComponent', () => {
  let component: VideoUnavailableComponent;
  let fixture: ComponentFixture<VideoUnavailableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ VideoUnavailableComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(VideoUnavailableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
