import {selectComment} from './../selectors/index';
import {VideoForumAction, VideoForumApiAction,} from 'src/app/community/video-forum/actions';
import {Store} from '@ngrx/store';
import {AuthService} from './../../../auth/services/auth.service';
import {AfterViewInit, Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild,} from '@angular/core';
import {FormControl, FormGroup, NgForm, Validators} from '@angular/forms';
import {filter, map, Observable, take, tap} from 'rxjs';
import {Actions, ofType} from '@ngrx/effects';

@Component({
  selector: 'app-video-comment-editor',
  templateUrl: './video-comment-editor.component.html',
  styleUrls: ['./video-comment-editor.component.css'],
})
export class VideoCommentEditorComponent implements AfterViewInit, OnInit {
  @Input()
  commentId!: number;

  @Input()
  comment: string = '';

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
      comment: new FormControl('', [Validators.required]),
    });
  }

  ngOnInit(): void {
    this.commentForm.get('comment')?.setValue(this.comment);
  }

  ngAfterViewInit(): void {
    if (this.focusOnInit) {
      setTimeout(() => {
        this.focus();
      }, 0);
    }
  }

  cancelEditing() {
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
    return this.store
      .select(selectComment(this.commentId))
      .pipe(map((x) => x?.editingComment));
  }

  editComment() {
    if (!this.commentForm.valid) return;

    this.store.dispatch(
      VideoForumAction.editComment({
        commentId: this.commentId,
        comment: this.commentForm.get('comment')!.value,
      })
    );

    this.actions$
      .pipe(
        ofType(VideoForumApiAction.commentEdited),
        filter((x) => x.commentId == this.commentId),
        take(1),
        tap(() => this.cancelEditing())
      )
      .subscribe();
  }
}
