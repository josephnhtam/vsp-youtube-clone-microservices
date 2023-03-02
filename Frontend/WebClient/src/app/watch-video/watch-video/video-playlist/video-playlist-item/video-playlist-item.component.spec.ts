import {ComponentFixture, TestBed} from '@angular/core/testing';

import {VideoPlaylistItemComponent} from './video-playlist-item.component';

describe('VideoPlaylistItemComponent', () => {
  let component: VideoPlaylistItemComponent;
  let fixture: ComponentFixture<VideoPlaylistItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ VideoPlaylistItemComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(VideoPlaylistItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
