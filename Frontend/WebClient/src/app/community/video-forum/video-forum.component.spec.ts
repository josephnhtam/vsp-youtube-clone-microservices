import {ComponentFixture, TestBed} from '@angular/core/testing';

import {VideoForumComponent} from './video-forum.component';

describe('VideoForumComponent', () => {
  let component: VideoForumComponent;
  let fixture: ComponentFixture<VideoForumComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ VideoForumComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(VideoForumComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
