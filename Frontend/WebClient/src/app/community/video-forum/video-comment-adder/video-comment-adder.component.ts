import {selectVideoForumState} from './../../selectors/index';
import {selectComment} from './../selectors/index';
import {VideoForumAction, VideoForumApiAction,} from 'src/app/community/video-forum/actions';
import {Store} from '@ngrx/store';
import {AuthService} from './../../../auth/services/auth.service';
import {AfterViewInit, Component, ElementRef, EventEmitter, Input, Output, ViewChild,} from '@angular/core';
import {FormControl, FormGroup, NgForm, Validators} from '@angular/forms';
import {filter, map, Observable, take, tap} from 'rxjs';
import {Actions, ofType} from '@ngrx/effects';

@Component({
  selector: 'app-video-comment-adder',
  templateUrl: './video-comment-adder.component.html',
  styleUrls: ['./video-comment-adder.component.css'],
})
export class VideoCommentAdderComponent implements AfterViewInit {
  @Input()
  parentCommentId: number | null = null;

  @Input()
  label = 'Add a comment';

  @Input()
  small = false;

  @Input()
  focusOnInit = false;

  @Output()
  cancel = new EventEmitter<void>();

  @ViewChild('commentEl')
  commentEl?: ElementRef;

  @ViewChild('formDirective')
  formDirective?: NgForm;

  commentForm: FormGroup;
  focusingComment = false;

  constructor(
    private authService: AuthService,
    private store: Store,
    private actions$: Actions
  ) {
    this.commentForm = new FormGroup({
      comment: new FormControl('', [
        Validators.required,
        Validators.maxLength(5000),
      ]),
    });
  }

  ngAfterViewInit(): void {
    if (this.focusOnInit) {
      setTimeout(() => {
        this.focus();
      }, 0);
    }
  }

  cancelComment() {
    this.formDirective?.resetForm();
    this.focusingComment = false;
    this.commentForm.reset();
    this.cancel.emit();
  }

  focus() {
    this.commentEl?.nativeElement.focus();
  }

  get authInfo$() {
    return this.authService.authInfo$;
  }

  get userId$(): Observable<string | undefined> {
    return this.authInfo$.pipe(map((x) => x?.sub));
  }

  get pending$() {
    if (this.parentCommentId != null) {
      return this.store
        .select(selectComment(this.parentCommentId))
        .pipe(map((x) => x?.addingComment));
    } else {
      return this.store
        .select(selectVideoForumState)
        .pipe(map((x) => x.addingComment));
    }
  }

  addComment() {
    if (!this.commentForm.valid) return;

    if (this.parentCommentId != null) {
      this.store.dispatch(
        VideoForumAction.replyToComment({
          commentId: this.parentCommentId,
          comment: this.commentForm.get('comment')!.value,
        })
      );

      this.actions$
        .pipe(
          ofType(VideoForumApiAction.commentReplied),
          filter((x) => x.commentId == this.parentCommentId),
          take(1),
          tap(() => this.cancelComment())
        )
        .subscribe();
    } else {
      this.store.dispatch(
        VideoForumAction.addComment({
          comment: this.commentForm.get('comment')!.value,
        })
      );

      this.actions$
        .pipe(
          ofType(VideoForumApiAction.commentAdded),
          filter((x) => !!x.videoId),
          take(1),
          tap(() => this.cancelComment())
        )
        .subscribe();
    }
  }
}
