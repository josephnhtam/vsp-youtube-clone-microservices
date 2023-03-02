import {ComponentFixture, TestBed} from '@angular/core/testing';

import {VideoCommentEditorComponent} from './video-comment-editor.component';

describe('VideoCommentEditorComponent', () => {
  let component: VideoCommentEditorComponent;
  let fixture: ComponentFixture<VideoCommentEditorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ VideoCommentEditorComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(VideoCommentEditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
