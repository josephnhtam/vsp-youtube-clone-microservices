import {
  userPinPriority,
  VideoComment,
  VideoCommentClient,
  VideoCommentSort,
  VoteType,
} from './../models';
import { createEntityAdapter, EntityState } from '@ngrx/entity';
import { createReducer, on } from '@ngrx/store';
import { VideoForumAction, VideoForumApiAction } from '../actions';

export interface State extends EntityState<VideoCommentClient> {
  videoId: string | null;
  loaded: boolean;
  pending: boolean;
  loadingMoreComments: boolean;
  addingComment: boolean;
  error: any | null;
  currentPage: number;
  rootPageSize: number;
  commentPageSize: number;
  sort: VideoCommentSort;
  rootCommentsCount: number;
  commentsCount: number;
  likedCommentIds: number[];
  dislikedCommentIds: number[];
  loadTime: Date;
  obtainedLastComment: boolean;
}

export const adapter = createEntityAdapter<VideoCommentClient>({
  selectId: (x) => x.id,
});

const initialState = adapter.getInitialState({
  videoId: null,
  loaded: false,
  pending: false,
  loadingMoreComments: false,
  addingComment: false,
  error: null,
  currentPage: 1,
  rootPageSize: 20,
  commentPageSize: 10,
  sort: VideoCommentSort.Date,
  rootCommentsCount: 0,
  commentsCount: 0,
  likedCommentIds: [],
  dislikedCommentIds: [],
  loadTime: new Date(),
  obtainedLastComment: false,
});

export const reducer = createReducer<State>(
  initialState,

  on(VideoForumApiAction.commentDeleted, (state, { commentId }) => {
    const commentToDelete = state.entities[commentId] as VideoCommentClient;

    if (!!commentToDelete) {
      const parentCommentId = commentToDelete.parentCommentId;

      if (!!parentCommentId) {
        return adapter.removeOne(
          commentId,
          adapter.updateOne(
            {
              id: parentCommentId,
              changes: {
                repliesCount: state.entities[parentCommentId]!.repliesCount - 1,
              },
            },
            {
              ...state,
              commentsCount:
                state.commentsCount -
                1 -
                (commentToDelete.repliesCount ?? 0) -
                (commentToDelete.newRepliesCount ?? 0),
            }
          )
        );
      } else {
        return adapter.removeOne(commentId, {
          ...state,
          commentsCount:
            state.commentsCount -
            1 -
            (commentToDelete.repliesCount ?? 0) -
            (commentToDelete.newRepliesCount ?? 0),
        });
      }
    }

    return state;
  }),

  on(VideoForumApiAction.failedToEditComment, (state, { commentId }) => {
    return adapter.updateOne(
      {
        id: commentId,
        changes: {
          editingComment: false,
        },
      },
      state
    );
  }),

  on(VideoForumApiAction.commentEdited, (state, { commentId, comment }) => {
    return adapter.updateOne(
      {
        id: commentId,
        changes: {
          editingComment: false,
          comment: comment,
          editDate: new Date().toDateString(),
        },
      },
      state
    );
  }),

  on(VideoForumAction.editComment, (state, { commentId, comment }) => {
    return adapter.updateOne(
      {
        id: commentId,
        changes: {
          editingComment: true,
        },
      },
      state
    );
  }),

  on(VideoForumAction.voteComment, (state, { commentId, voteType }) => {
    let likedCommentIds = state.likedCommentIds.filter((id) => id != commentId);

    let dislikedCommentIds = state.dislikedCommentIds.filter(
      (id) => id != commentId
    );

    const commentClient = state.entities[commentId];
    let likesCount = commentClient?.likesCount ?? 0;
    let dislikesCount = commentClient?.dislikesCount ?? 0;

    const isLiked = state.likedCommentIds.includes(commentId);
    const isDisliked = state.dislikedCommentIds.includes(commentId);

    if (isLiked) {
      likesCount--;
    }

    if (isDisliked) {
      dislikesCount--;
    }

    switch (voteType) {
      case VoteType.Like:
        likedCommentIds = [...likedCommentIds, commentId];
        likesCount++;
        break;
      case VoteType.Dislike:
        dislikedCommentIds = [...dislikedCommentIds, commentId];
        dislikesCount++;
        break;
    }

    return adapter.updateOne(
      {
        id: commentId,
        changes: {
          likesCount,
          dislikesCount,
        },
      },
      {
        ...state,
        likedCommentIds,
        dislikedCommentIds,
      }
    );
  }),

  on(VideoForumApiAction.failedToReplyToComment, (state, { commentId }) => {
    return adapter.updateOne(
      {
        id: commentId,
        changes: {
          addingComment: false,
        },
      },
      state
    );
  }),

  on(VideoForumApiAction.failedToAddComment, (state, { videoId }) => {
    if (videoId != state.videoId) return state;

    return {
      ...state,
      addingComment: false,
    };
  }),

  on(VideoForumApiAction.commentReplied, (state, { commentId, comment }) => {
    const [commentClient] = convertToVideoCommentClient(
      [comment],
      commentId,
      -1,
      true
    );

    return adapter.upsertOne(
      commentClient,
      adapter.updateOne(
        {
          id: commentId,
          changes: {
            addingComment: false,
            showReplies: false,
            newRepliesCount:
              (state.entities[commentId]?.newRepliesCount ?? 0) + 1,
          },
        },
        {
          ...state,
          commentsCount: state.commentsCount + 1,
        }
      )
    );
  }),

  on(VideoForumApiAction.commentAdded, (state, { videoId, comment }) => {
    if (videoId != state.videoId) return state;

    const [commentClient] = convertToVideoCommentClient(
      [comment],
      null,
      userPinPriority,
      true
    );

    return adapter.upsertOne(commentClient, {
      ...state,
      addingComment: false,
      commentsCount: state.commentsCount + 1,
      rootCommentsCount: state.rootCommentsCount + 1,
    });
  }),

  on(VideoForumAction.replyToComment, (state, { commentId }) => {
    return adapter.updateOne(
      {
        id: commentId,
        changes: {
          addingComment: true,
        },
      },
      state
    );
  }),

  on(VideoForumAction.addComment, (state) => {
    return {
      ...state,
      addingComment: true,
    };
  }),

  on(VideoForumAction.hideReplies, (state, { commentId }) => {
    return adapter.updateOne(
      {
        id: commentId,
        changes: {
          showReplies: false,
        },
      },
      state
    );
  }),

  on(VideoForumAction.showReplies, (state, { commentId }) => {
    return adapter.updateOne(
      {
        id: commentId,
        changes: {
          showReplies: true,
        },
      },
      state
    );
  }),

  on(
    VideoForumApiAction.failedToObtainCommentReplies,
    (state, { commentId, error }) => {
      return adapter.updateOne(
        {
          id: commentId,
          changes: {
            pending: false,
            error,
          },
        },
        state
      );
    }
  ),

  on(
    VideoForumApiAction.commentRepliesObtained,
    (state, { parentCommentId, comments }) => {
      const videoCommentClients = convertToVideoCommentClient(
        comments,
        parentCommentId
      );

      return adapter.updateOne(
        {
          id: parentCommentId,
          changes: {
            pending: false,
            obtainedLastReply: comments.length == 0,
          },
        },
        adapter.addMany(videoCommentClients, {
          ...state,
          pending: false,
        })
      );
    }
  ),

  on(VideoForumAction.getCommentReplies, (state, { commentId }) => {
    return adapter.updateOne(
      {
        id: commentId,
        changes: {
          pending: true,
        },
      },
      state
    );
  }),

  on(
    VideoForumApiAction.failedToObtainRootComments,
    (state, { videoId, error }) => {
      if (videoId != state.videoId) return state;

      return {
        ...state,
        loadingMoreComments: false,
        error,
      };
    }
  ),

  on(
    VideoForumApiAction.rootCommentsObtained,
    (state, { videoId, page, comments }) => {
      if (videoId != state.videoId) return state;

      const videoCommentClients = convertToVideoCommentClient(comments);
      const obtainedLastComment = videoCommentClients.length == 0;

      return adapter.addMany(videoCommentClients, {
        ...state,
        currentPage: obtainedLastComment
          ? state.currentPage
          : Math.max(state.currentPage, page),
        obtainedLastComment: obtainedLastComment,
        loadingMoreComments: false,
      });
    }
  ),

  on(VideoForumAction.getRootComments, (state) => {
    return {
      ...state,
      loadingMoreComments: true,
    };
  }),

  on(
    VideoForumApiAction.failedToLoadVideoForum,
    (state, { videoId, error }) => {
      if (videoId != state.videoId) {
        return state;
      }

      return {
        ...state,
        pending: false,
        error,
      };
    }
  ),

  on(
    VideoForumApiAction.videoForumLoaded,
    (
      state,
      {
        videoId,
        rootPageSize,
        commentPageSize,
        sort,
        commentsCount,
        rootCommentsCount,
        likedCommentIds,
        dislikedCommentIds,
        comments,
        pinnedUserComments,
        loadTime,
      }
    ) => {
      if (videoId != state.videoId) return state;

      const normalVideoCommentClients = convertToVideoCommentClient(comments);

      const pinnedUserVideoCommentClients = convertToVideoCommentClient(
        pinnedUserComments ?? [],
        null,
        userPinPriority
      );

      const videoCommentClients = [
        ...normalVideoCommentClients,
        ...pinnedUserVideoCommentClients,
      ];

      return adapter.setAll(videoCommentClients, {
        ...state,
        rootPageSize,
        commentPageSize,
        sort,
        loaded: true,
        pending: false,
        commentsCount,
        rootCommentsCount,
        likedCommentIds,
        dislikedCommentIds,
        loadTime,
        currentPage: 1,
      });
    }
  ),

  on(
    VideoForumAction.loadForum,
    (state, { videoId, rootPageSize, commentPageSize }) => {
      return {
        ...state,
        videoId,
        rootPageSize,
        commentPageSize,
        pending: true,
        error: null,
      };
    }
  )
);

function convertToVideoCommentClient(
  comments: VideoComment[],
  parentCommentId: number | null = null,
  pinPriority: number = -1,
  newlyAdded: boolean = false
): VideoCommentClient[] {
  const videoCommentClients: VideoCommentClient[] = comments.map((comment) => {
    return {
      ...comment,
      parentCommentId,
      showReplies: false,
      pending: false,
      addingComment: false,
      editingComment: false,
      error: null,
      parsedCreateDate: new Date(comment.createDate),
      obtainedLastReply: false,
      pinPriority,
      newlyAdded,

      likesCountForSort: comment.likesCount,
      repliesCountForSort: comment.repliesCount,
    };
  });

  return videoCommentClients;
}
