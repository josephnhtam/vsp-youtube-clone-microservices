import {CreatorProfile, VideoStatus, VideoVisibility} from './video';

export enum VoteType {
  None = 0,
  Like = 1,
  Dislike = -1,
}

export interface VideoMetadata {
  viewsCount: number;
  likesCount: number;
  dislikesCount: number;
  userVote: VoteType;
}

export interface VoteVideoRequest {
  videoId: string;
  voteType: VoteType;
}

export interface MovePlaylistItemRequest {
  playlist: string;
  itemId: string;
  toPosition: number;
}

export interface MovePlaylistItemByIdRequest {
  playlist: string;
  itemId: string;
  precedingItemId: string | null;
}

export interface MovePlaylistItemByIdRequest {
  playlist: string;
  itemId: string;
  precedingItemId: string | null;
}

export interface VideoMetrics {
  viewsCount: number;
  likesCount: number;
  dislikesCount: number;
}

export interface HiddenVideo {
  id: string;
  visibility: VideoVisibility | null;
}

export interface Video {
  id: string;
  creatorProfile: CreatorProfile;
  title: string;
  thumbnailUrl: string | null;
  previewThumbnailUrl: string | null;
  visibility: VideoVisibility;
  status: VideoStatus;
  metrics: VideoMetrics;
  createDate: string;
  publishDate: string | null;
  lengthSeconds: number;
}

export interface Playlist {
  id: string;
  creatorProfile: CreatorProfile;
  title: string | null;
  description: string | null;
  visibility: PlaylistVisibility | null;
  itemsCount: number;
  items: PlaylistItem[];
  createDate: string;
  updateDate: string;
}

export interface PlaylistItem {
  id: string;
  video: Video | HiddenVideo;
  position: number | null;
  createDate: string;
}

export enum PlaylistVisibility {
  Private = 0,
  Unlisted = 1,
  Public = 2,
}

export interface PlaylistClient extends Playlist {
  pending: boolean;
}

export interface SimplePlaylist {
  id: string;
  title: string | null;
  visibility: PlaylistVisibility | null;
  itemsCount: number;
  createDate: string;
}

export interface PlaylistsWithVideo {
  isAddedToWatchLaterPlaylist: boolean;
  playlistsWithVideo: SimplePlaylist[];
  playlistsWithoutVideo: SimplePlaylist[];
}

export interface AddVideoToPlaylistRequest {
  videoId: string;
  playlist: string;
}

export interface PlaylistItemClient extends PlaylistItem {
  pending: boolean;
}

export interface GetPlaylistInfosResponse {
  totalCount: number;
  infos: PlaylistInfo[];
}

export interface PlaylistInfo {
  id: string;
  creatorProfile: CreatorProfile;
  title: string;
  thumbnailUrl: string | null;
  videoId: string | null;
  visibility: PlaylistVisibility | null;
  itemsCount: number;
  createDate: string;
  updateDate: string;
}

export function isVideoAvailable(video: Video | HiddenVideo): video is Video {
  return !!video && 'title' in video;
}
