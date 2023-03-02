import {ComponentFixture, TestBed} from '@angular/core/testing';

import {VideoGridItemComponent} from './video-grid-item.component';

describe('VideoGridItemComponent', () => {
  let component: VideoGridItemComponent;
  let fixture: ComponentFixture<VideoGridItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ VideoGridItemComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(VideoGridItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
