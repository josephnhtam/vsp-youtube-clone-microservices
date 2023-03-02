import { AuthService } from '../../../auth/services/auth.service';
import {
  AddCommentRequest,
  CommentAddedResponse,
  EditCommentRequest,
  GetVideoForumResponse,
  ReplyToCommentRequest,
  VideoComment,
  VoteVideoCommentRequest,
} from '../models';
import {
  selectComment,
  selectCommentReplies,
  selectRootComments,
  selectVideoId,
} from '../selectors';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import {
  catchError,
  concatMap,
  EMPTY,
  map,
  mergeMap,
  of,
  switchMap,
} from 'rxjs';
import { Store } from '@ngrx/store';
import { Injectable } from '@angular/core';
import { Actions, concatLatestFrom, createEffect, ofType } from '@ngrx/effects';
import { VideoForumAction, VideoForumApiAction } from '../actions';
import { selectVideoForumState } from '../../selectors';

@Injectable()
export class VideoForumEffect {
  constructor(
    private actions$: Actions,
    private store: Store,
    private authService: AuthService,
    private httpClient: HttpClient
  ) {}

  deleteCommentEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(VideoForumAction.deleteComment),

      concatLatestFrom(() => this.store.select(selectVideoId)),

      concatMap(([{ commentId }, videoId]) => {
        if (videoId == null) {
          throw new Error('Video forum is not loaded yet.');
        }

        const url = environment.appSetup.apiUrl + '/api/v1/VideoForum/Comments';

        const params = new HttpParams({
          fromObject: {
            commentId,
          },
        });

        return this.httpClient.delete(url, { params }).pipe(
          map(() => {
            return VideoForumApiAction.commentDeleted({
              commentId,
            });
          }),

          catchError((error) => {
            console.error(error);

            return of(
              VideoForumApiAction.failedToDeleteComment({
                commentId,
                error,
              })
            );
          })
        );
      })
    )
  );

  editCommentEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(VideoForumAction.editComment),

      concatLatestFrom(() => this.store.select(selectVideoId)),

      concatMap(([{ commentId, comment }, videoId]) => {
        if (videoId == null) {
          throw new Error('Video forum is not loaded yet.');
        }

        const url = environment.appSetup.apiUrl + '/api/v1/VideoForum/Comments';

        const request: EditCommentRequest = {
          commentId,
          comment,
        };

        return this.httpClient.put(url, request).pipe(
          map(() => {
            return VideoForumApiAction.commentEdited({
              commentId,
              comment,
            });
          }),

          catchError((error) => {
            console.error(error);

            return of(
              VideoForumApiAction.failedToEditComment({
                commentId,
                error,
              })
            );
          })
        );
      })
    )
  );

  voteCommentEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(VideoForumAction.voteComment),

      switchMap(({ commentId, voteType }) => {
        const url =
          environment.appSetup.apiUrl + '/api/v1/VideoForum/Comments/Votes';

        const request: VoteVideoCommentRequest = {
          commentId,
          voteType,
        };

        return this.httpClient.post(url, request).pipe(
          map(() => {
            return VideoForumApiAction.commentVoted({
              commentId,
              voteType,
            });
          }),

          catchError((error) => {
            console.error(error);

            return of(
              VideoForumApiAction.failedToVoteComment({
                commentId,
                voteType,
                error,
              })
            );
          })
        );
      })
    )
  );

  replyToCommentEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(VideoForumAction.replyToComment),

      concatLatestFrom(() => this.store.select(selectVideoId)),

      concatMap(([{ commentId, comment }, videoId]) => {
        if (videoId == null) {
          throw new Error('Video forum is not loaded yet.');
        }

        const url =
          environment.appSetup.apiUrl + '/api/v1/VideoForum/Comments/Replies';

        const request: ReplyToCommentRequest = {
          commentId,
          comment,
        };

        return this.httpClient.post<CommentAddedResponse>(url, request).pipe(
          map((response) => {
            return VideoForumApiAction.commentReplied({
              commentId,
              comment: response.comment!,
            });
          }),

          catchError((error) => {
            console.error(error);

            return of(
              VideoForumApiAction.failedToReplyToComment({
                commentId,
                error,
              })
            );
          })
        );
      })
    )
  );

  addCommentEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(VideoForumAction.addComment),

      concatLatestFrom(() => this.store.select(selectVideoId)),

      concatMap(([{ comment }, videoId]) => {
        if (videoId == null) {
          throw new Error('Video forum is not loaded yet.');
        }

        const url = environment.appSetup.apiUrl + '/api/v1/VideoForum/Comments';

        const request: AddCommentRequest = {
          videoId: videoId,
          comment,
        };

        return this.httpClient.post<CommentAddedResponse>(url, request).pipe(
          map((response) => {
            return VideoForumApiAction.commentAdded({
              videoId,
              comment: response.comment!,
            });
          }),

          catchError((error) => {
            console.error(error);

            return of(
              VideoForumApiAction.failedToAddComment({
                videoId,
                error,
              })
            );
          })
        );
      })
    )
  );

  showCommentRepliesEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(VideoForumAction.showReplies),

      concatLatestFrom(({ commentId }) =>
        this.store.select(selectComment(commentId))
      ),

      concatLatestFrom(([{ commentId }]) =>
        this.store.select(selectCommentReplies(commentId, false))
      ),

      mergeMap(([[{ commentId }, commentClient], commentReplies]) => {
        if (
          (commentClient?.repliesCount ?? 0) > 0 &&
          commentReplies.length == 0
        ) {
          return of(VideoForumAction.getCommentReplies({ commentId }));
        } else {
          return EMPTY;
        }
      })
    )
  );

  getCommentRepliesEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(VideoForumAction.getCommentReplies),

      concatLatestFrom(() => this.store.select(selectVideoForumState)),

      concatLatestFrom(([{ commentId }]) =>
        this.store.select(selectCommentReplies(commentId, false))
      ),

      mergeMap(([[{ commentId }, videoForumState], obtainedComments]) => {
        const url =
          environment.appSetup.apiUrl + '/api/v1/VideoForum/Comments/Replies';

        const params = new HttpParams({
          fromObject: {
            commentId: commentId,
            pageSize: videoForumState.commentPageSize,
            page:
              1 +
              Math.floor(
                obtainedComments.length / videoForumState.commentPageSize
              ),
          },
        });

        return this.httpClient.get<VideoComment[]>(url, { params }).pipe(
          map((comments) => {
            return VideoForumApiAction.commentRepliesObtained({
              parentCommentId: commentId,
              comments,
            });
          }),

          catchError((error) => {
            console.error(error);

            return of(
              VideoForumApiAction.failedToObtainCommentReplies({
                commentId,
                error,
              })
            );
          })
        );
      })
    )
  );

  getRootCommentsEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(VideoForumAction.getRootComments),

      concatLatestFrom(() => this.store.select(selectVideoForumState)),

      concatLatestFrom(() => this.store.select(selectRootComments)),

      mergeMap(([[action, videoForumState], rootComments]) => {
        const url = environment.appSetup.apiUrl + '/api/v1/VideoForum/Comments';

        const maxTimestamp = videoForumState.loadTime.getTime();
        const page = videoForumState.currentPage + 1;

        const params = new HttpParams({
          fromObject: {
            videoId: videoForumState.videoId!,
            pageSize: videoForumState.rootPageSize,
            sort: videoForumState.sort,
            maxTimestamp,
            page,
          },
        });

        return this.httpClient.get<VideoComment[]>(url, { params }).pipe(
          map((comments) => {
            return VideoForumApiAction.rootCommentsObtained({
              videoId: videoForumState.videoId!,
              page,
              comments,
            });
          }),

          catchError((error) => {
            console.error(error);

            return of(
              VideoForumApiAction.failedToObtainRootComments({
                videoId: videoForumState.videoId!,
                error,
              })
            );
          })
        );
      })
    )
  );

  loadForumEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(VideoForumAction.loadForum),

      mergeMap(({ videoId, rootPageSize, commentPageSize, sort }) => {
        const url = environment.appSetup.apiUrl + '/api/v1/VideoForum';

        const params = new HttpParams({
          fromObject: {
            videoId,
            pageSize: rootPageSize,
            sort,
          },
        });

        return this.httpClient
          .get<GetVideoForumResponse>(url, {
            params,
          })
          .pipe(
            map((response) => {
              return VideoForumApiAction.videoForumLoaded({
                videoId,
                rootPageSize,
                commentPageSize,
                sort,
                commentsCount: response.commentsCount,
                rootCommentsCount: response.rootCommentsCount,
                likedCommentIds: response.likedCommentIds,
                dislikedCommentIds: response.dislikedCommentIds,
                comments: response.comments,
                pinnedUserComments: response.pinnedUserComments,
                loadTime: new Date(response.loadTime),
              });
            }),

            catchError((error) => {
              console.error(error);

              return of(
                VideoForumApiAction.failedToLoadVideoForum({
                  videoId,
                  error,
                })
              );
            })
          );
      })
    )
  );
}
