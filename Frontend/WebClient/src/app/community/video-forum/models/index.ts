export const userPinPriority = 0;

export interface GetVideoForumResponse {
  commentsCount: number;
  rootCommentsCount: number;
  likedCommentIds: number[];
  dislikedCommentIds: number[];
  comments: VideoComment[];
  pinnedUserComments: VideoComment[] | null;
  loadTime: string;
}

export interface VideoComment {
  id: number;
  userProfile: UserProfile;
  comment: string;
  likesCount: number;
  dislikesCount: number;
  repliesCount: number;
  newRepliesCount: number;
  createDate: string;
  editDate: string | null;
}

export interface UserProfile {
  id: string;
  handle: string | null;
  displayName: string;
  thumbnailUrl: string | null;
}

export enum VideoCommentSort {
  Date = 0,
  LikesCount = 1,
  RepliesCount = 2,
}

export interface VideoCommentClient extends VideoComment {
  parentCommentId: number | null;
  showReplies: boolean;
  pending: boolean;
  addingComment: boolean;
  editingComment: boolean;
  error: any | null;
  parsedCreateDate: Date;
  obtainedLastReply: boolean;
  pinPriority: number;
  newlyAdded: boolean;
  likesCountForSort: number;
  repliesCountForSort: number;
}

export interface AddCommentRequest {
  videoId: string;
  comment: string;
}

export interface ReplyToCommentRequest {
  commentId: number;
  comment: string;
}

export interface EditCommentRequest {
  commentId: number;
  comment: string;
}

export interface VoteVideoCommentRequest {
  commentId: number;
  voteType: VoteType;
}

export interface CommentAddedResponse {
  comment: VideoComment | null;
}

export enum VoteType {
  None = 0,
  Like = 1,
  Dislike = -1,
}
