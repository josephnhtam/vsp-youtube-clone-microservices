import {VideoCommentSort, VoteType} from './../models/index';
import {createAction, props} from '@ngrx/store';

export const loadForum = createAction(
  '[video forum] load forum',
  props<{
    videoId: string;
    rootPageSize: number;
    commentPageSize: number;
    sort: VideoCommentSort;
  }>()
);

export const getRootComments = createAction('[video forum] get root comments');

export const getCommentReplies = createAction(
  '[video forum] get comment replies',
  props<{ commentId: number }>()
);

export const showReplies = createAction(
  '[video forum] show replies',
  props<{ commentId: number }>()
);

export const hideReplies = createAction(
  '[video forum] hide replies',
  props<{ commentId: number }>()
);

export const addComment = createAction(
  '[video forum] add comment',
  props<{ comment: string }>()
);

export const replyToComment = createAction(
  '[video forum] reply to comment',
  props<{ commentId: number; comment: string }>()
);

export const editComment = createAction(
  '[video forum] edit comment',
  props<{ commentId: number; comment: string }>()
);

export const voteComment = createAction(
  '[video forum] vote comment',
  props<{ commentId: number; voteType: VoteType }>()
);

export const deleteComment = createAction(
  '[video forum] delete comment',
  props<{ commentId: number }>()
);
