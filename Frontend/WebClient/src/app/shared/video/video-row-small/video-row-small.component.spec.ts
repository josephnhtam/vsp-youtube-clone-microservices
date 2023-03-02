import {ComponentFixture, TestBed} from '@angular/core/testing';

import {VideoRowSmallComponent} from './video-row-small.component';

describe('VideoRowSmallComponent', () => {
  let component: VideoRowSmallComponent;
  let fixture: ComponentFixture<VideoRowSmallComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ VideoRowSmallComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(VideoRowSmallComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
