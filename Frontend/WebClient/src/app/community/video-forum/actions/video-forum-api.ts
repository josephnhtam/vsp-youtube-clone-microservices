import {VoteType} from './../models/index';
import {createAction, props} from '@ngrx/store';
import {VideoComment, VideoCommentSort} from '../models';

export const failedToVoteComment = createAction(
  '[video forum / api] failed to vote comment',
  props<{ commentId: number; voteType: VoteType; error: any }>()
);

export const commentVoted = createAction(
  '[video forum / api] comment voted',
  props<{ commentId: number; voteType: VoteType }>()
);

export const failedToAddComment = createAction(
  '[video forum / api] failed to add comment',
  props<{ videoId: string; error: any }>()
);

export const failedToReplyToComment = createAction(
  '[video forum / api] failed to reply to comment',
  props<{ commentId: number; error: any }>()
);

export const failedToEditComment = createAction(
  '[video forum / api] failed to edit comment',
  props<{ commentId: number; error: any }>()
);

export const failedToDeleteComment = createAction(
  '[video forum / api] failed to delete comment',
  props<{ commentId: number; error: any }>()
);

export const commentAdded = createAction(
  '[video forum / api] comment added',
  props<{ videoId: string; comment: VideoComment }>()
);

export const commentReplied = createAction(
  '[video forum / api] comment replied',
  props<{ commentId: number; comment: VideoComment }>()
);

export const commentEdited = createAction(
  '[video forum / api] comment edited',
  props<{ commentId: number; comment: string }>()
);

export const commentDeleted = createAction(
  '[video forum / api] comment deleted',
  props<{ commentId: number }>()
);

export const failedToObtainCommentReplies = createAction(
  '[video forum / api] failed to obtain comment replies',
  props<{ commentId: number; error: any }>()
);

export const failedToObtainRootComments = createAction(
  '[video forum / api] failed to obtain root comments',
  props<{ videoId: string; error: any }>()
);

export const failedToLoadVideoForum = createAction(
  '[video forum / api] failed to load video forum',
  props<{
    videoId: string;
    error: any;
  }>()
);

export const videoForumLoaded = createAction(
  '[video forum / api] video forum loaded',
  props<{
    videoId: string;
    rootPageSize: number;
    commentPageSize: number;
    sort: VideoCommentSort;
    commentsCount: number;
    rootCommentsCount: number;
    likedCommentIds: number[];
    dislikedCommentIds: number[];
    comments: VideoComment[];
    pinnedUserComments: VideoComment[] | null;
    loadTime: Date;
  }>()
);

export const rootCommentsObtained = createAction(
  '[video forum / api] root comments loaded',
  props<{
    videoId: string;
    page: number;
    comments: VideoComment[];
  }>()
);

export const commentRepliesObtained = createAction(
  '[video forum / api] comment replies loaded',
  props<{
    parentCommentId: number;
    comments: VideoComment[];
  }>()
);
