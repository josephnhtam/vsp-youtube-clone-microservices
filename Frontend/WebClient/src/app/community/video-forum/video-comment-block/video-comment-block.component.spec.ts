import {ComponentFixture, TestBed} from '@angular/core/testing';
import {VideoCommentBlockComponent} from './video-comment-block.component';

describe('VideoCommentBlockComponent', () => {
  let component: VideoCommentBlockComponent;
  let fixture: ComponentFixture<VideoCommentBlockComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [VideoCommentBlockComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(VideoCommentBlockComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
