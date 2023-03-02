import {ComponentFixture, TestBed} from '@angular/core/testing';

import {VideoCommentAdderComponent} from './video-comment-adder.component';

describe('VideoCommentAdderComponent', () => {
  let component: VideoCommentAdderComponent;
  let fixture: ComponentFixture<VideoCommentAdderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [VideoCommentAdderComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(VideoCommentAdderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
