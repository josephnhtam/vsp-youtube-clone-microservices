import { ConfirmDialogService } from 'src/app/shared/confirm-dialog/confirm-dialog.service';
import { VideoCommentEditorComponent } from './../video-comment-editor/video-comment-editor.component';
import { concatLatestFrom } from '@ngrx/effects';
import {
  selectComment,
  selectCommentReplies,
  selectDislikedCommentIds,
} from './../selectors';
import { VideoForumAction } from 'src/app/community/video-forum/actions';
import { AuthService } from './../../../auth/services/auth.service';
import { map, of, take, tap } from 'rxjs';
import { VideoCommentAdderComponent } from '../video-comment-adder/video-comment-adder.component';
import { ChangeDetectorRef, Component, Input, ViewChild } from '@angular/core';
import { Store } from '@ngrx/store';
import { selectLikedCommentIds } from '../selectors';
import { VideoCommentClient, VoteType } from '../models';
import { getCommentReplies, voteComment } from '../actions/video-forum';
import * as moment from 'moment';
import { UserProfileService } from 'src/app/core/services/user-profile.service';

@Component({
  selector: 'app-video-comment-block',
  templateUrl: './video-comment-block.component.html',
  styleUrls: ['./video-comment-block.component.css'],
})
export class VideoCommentBlockComponent {
  @Input()
  commentClient!: VideoCommentClient;

  @Input()
  small = false;

  @ViewChild('replyAdder')
  replyAdder?: VideoCommentAdderComponent;

  @ViewChild('commentEditor')
  editor?: VideoCommentEditorComponent;

  isFocusing = false;
  isReplying = false;
  isEditing = false;

  constructor(
    private store: Store,
    private authService: AuthService,
    private userProfileService: UserProfileService,
    private changeDetector: ChangeDetectorRef,
    private confirmDialog: ConfirmDialogService
  ) {}

  reply() {
    this.isReplying = true;
    this.changeDetector.detectChanges();
    this.replyAdder?.focus();
  }

  cancelReply() {
    this.isReplying = false;
  }

  edit() {
    this.isEditing = true;
    this.changeDetector.detectChanges();
    this.editor?.focus();
  }

  cancelEdit() {
    this.isEditing = false;
  }

  get userProfile() {
    return this.commentClient.userProfile;
  }

  get comment() {
    return this.commentClient.comment;
  }

  get likesCount() {
    return this.commentClient.likesCount;
  }

  get dislikesCount() {
    return this.commentClient.dislikesCount;
  }

  get likedCommentIds$() {
    return this.store.select(selectLikedCommentIds);
  }

  get dislikedCommentIds$() {
    return this.store.select(selectDislikedCommentIds);
  }

  get commentId() {
    return this.commentClient.id;
  }

  get isLiked$() {
    return this.likedCommentIds$.pipe(
      map((ids) => ids.some((id) => id === this.commentId))
    );
  }

  get isDisliked$() {
    return this.dislikedCommentIds$.pipe(
      map((ids) => ids.some((id) => id === this.commentId))
    );
  }

  get repliesCount() {
    return this.commentClient.repliesCount;
  }

  get hasReply() {
    return this.repliesCount > 0;
  }

  get showingReplies() {
    return this.commentClient.showReplies;
  }

  toggleShowReplies() {
    if (this.commentClient.showReplies) {
      this.store.dispatch(
        VideoForumAction.hideReplies({
          commentId: this.commentClient.id,
        })
      );
    } else {
      this.store.dispatch(
        VideoForumAction.showReplies({
          commentId: this.commentClient.id,
        })
      );
    }
  }

  get isAuthenticated$() {
    return this.authService.isAuthenticated$;
  }

  get isMine$() {
    return this.authService.authInfo$.pipe(
      map((authInfo) => authInfo?.sub == this.commentClient.userProfile.id)
    );
  }

  get isUserReady$() {
    return this.userProfileService.userReady$;
  }

  get edited() {
    return !!this.commentClient.editDate;
  }

  get replies$() {
    return this.store.select(
      selectCommentReplies(this.commentClient.id, false)
    );
  }

  get hasNewReply$() {
    return this.newReplies$.pipe(map((x) => x.length > 0));
  }

  get newReplies$() {
    return this.store.select(selectCommentReplies(this.commentClient.id, true));
  }

  get hasMoreReply$() {
    if (this.commentClient.obtainedLastReply) {
      return of(false);
    }

    return this.replies$.pipe(
      map((replies) => replies.length < this.commentClient.repliesCount)
    );
  }

  get relativeDate() {
    return moment(this.commentClient.parsedCreateDate).fromNow();
  }

  get loadingReplies$() {
    return this.store
      .select(selectComment(this.commentClient.id))
      .pipe(map((x) => x?.pending));
  }

  showMoreReplies() {
    this.store.dispatch(
      getCommentReplies({
        commentId: this.commentClient.id,
      })
    );
  }

  trackCommentClient(
    index: number,
    videoCommentClient: VideoCommentClient
  ): any {
    return videoCommentClient.id;
  }

  openOptionsMenu() {
    this.isFocusing = true;
    return false;
  }

  optionsMenuClosed() {
    this.isFocusing = false;
  }

  deleteComment() {
    const doDelete = () => {
      this.store.dispatch(
        VideoForumAction.deleteComment({ commentId: this.commentId })
      );
    };

    this.confirmDialog.openConfirmDialog(
      'Delete comment',
      'Delete your comment permanently?',
      null,
      doDelete,
      null,
      'Delete',
      'Cancel'
    );
  }

  onClickLike() {
    this.isLiked$
      .pipe(
        take(1),

        concatLatestFrom(() => this.authService.isAuthenticated$),

        tap(([isLiked, isAuthenticated]) => {
          if (!isAuthenticated) {
            return;
          }

          if (isLiked) {
            this.store.dispatch(
              voteComment({
                commentId: this.commentId,
                voteType: VoteType.None,
              })
            );
          } else {
            this.store.dispatch(
              voteComment({
                commentId: this.commentId,
                voteType: VoteType.Like,
              })
            );
          }
        })
      )
      .subscribe();
  }

  onClickDislike() {
    this.isDisliked$
      .pipe(
        take(1),

        concatLatestFrom(() => this.authService.isAuthenticated$),

        tap(([isDisliked, isAuthenticated]) => {
          if (!isAuthenticated) {
            return;
          }

          if (isDisliked) {
            this.store.dispatch(
              voteComment({
                commentId: this.commentId,
                voteType: VoteType.None,
              })
            );
          } else {
            this.store.dispatch(
              voteComment({
                commentId: this.commentId,
                voteType: VoteType.Dislike,
              })
            );
          }
        })
      )
      .subscribe();
  }
}
