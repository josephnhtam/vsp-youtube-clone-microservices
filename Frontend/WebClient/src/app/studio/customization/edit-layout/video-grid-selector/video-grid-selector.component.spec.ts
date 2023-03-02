import {ComponentFixture, TestBed} from '@angular/core/testing';

import {VideoGridSelectorComponent} from './video-grid-selector.component';

describe('VideoGridSelectorComponent', () => {
  let component: VideoGridSelectorComponent;
  let fixture: ComponentFixture<VideoGridSelectorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ VideoGridSelectorComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(VideoGridSelectorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
