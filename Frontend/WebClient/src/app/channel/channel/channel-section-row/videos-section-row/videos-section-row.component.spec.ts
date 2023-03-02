import {ComponentFixture, TestBed} from '@angular/core/testing';

import {VideosSectionRowComponent} from './videos-section-row.component';

describe('VideosSectionRowComponent', () => {
  let component: VideosSectionRowComponent;
  let fixture: ComponentFixture<VideosSectionRowComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ VideosSectionRowComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(VideosSectionRowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
