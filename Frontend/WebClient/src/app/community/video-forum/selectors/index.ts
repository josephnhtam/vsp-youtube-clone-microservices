import {createSelector} from '@ngrx/store';
import {selectVideoForumState} from '../../selectors';
import {VideoCommentSort} from '../models';
import {adapter} from '../reducers';

const selectors = adapter.getSelectors();

export const selectVideoId = createSelector(
  selectVideoForumState,
  (state) => state.videoId
);

export const selectCommentsCount = createSelector(
  selectVideoForumState,
  (state) => state.commentsCount
);

export const selectLikedCommentIds = createSelector(
  selectVideoForumState,
  (state) => state.likedCommentIds
);

export const selectDislikedCommentIds = createSelector(
  selectVideoForumState,
  (state) => state.dislikedCommentIds
);

export const selectComment = (commentId: number) =>
  createSelector(selectVideoForumState, (state) => {
    const comment = selectors
      .selectAll(state)
      .filter((comment) => comment.id == commentId);

    if (comment.length > 0) {
      return comment[0];
    } else {
      return null;
    }
  });

export const selectRootComments = createSelector(
  selectVideoForumState,
  (state) => {
    let rootComments = selectors
      .selectAll(state)
      .filter((x) => x.parentCommentId == null)
      .sort(
        (a, b) => b.parsedCreateDate.getTime() - a.parsedCreateDate.getTime()
      );

    switch (state.sort) {
      case VideoCommentSort.LikesCount:
        rootComments = rootComments.sort(
          (a, b) => b.likesCountForSort - a.likesCountForSort
        );
        break;
      case VideoCommentSort.RepliesCount:
        rootComments = rootComments.sort(
          (a, b) => b.repliesCountForSort - a.repliesCountForSort
        );
        break;
    }

    rootComments = rootComments.sort((a, b) => b.pinPriority - a.pinPriority);

    return rootComments;
  }
);

export const selectCommentReplies = (commentId: number, newlyAdded: boolean) =>
  createSelector(selectVideoForumState, (state) => {
    const rootComments = selectors
      .selectAll(state)
      .filter(
        (x) => x.parentCommentId == commentId && x.newlyAdded === newlyAdded
      );

    return rootComments.sort(
      (a, b) => a.parsedCreateDate.getTime() - b.parsedCreateDate.getTime()
    );
  });

export const selectHasMoreComments = createSelector(
  selectVideoForumState,
  (state) => {
    if (state.obtainedLastComment) return false;

    const rootCommentsCount = Object.values(state.entities).filter(
      (x) => x?.parentCommentId == null
    ).length;

    return state.rootCommentsCount > rootCommentsCount;
  }
);
