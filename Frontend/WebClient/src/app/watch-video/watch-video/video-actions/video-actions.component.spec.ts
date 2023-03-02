import {ComponentFixture, TestBed} from '@angular/core/testing';

import VideoActionComponent from './video-actions.component';

describe('VideoActionComponent', () => {
  let component: VideoActionComponent;
  let fixture: ComponentFixture<VideoActionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [VideoActionComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(VideoActionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
