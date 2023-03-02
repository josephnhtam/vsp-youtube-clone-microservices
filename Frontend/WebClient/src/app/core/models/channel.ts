import {SubscriptionStatus} from "./subscription";

export interface VideoMetrics {
  viewsCount: number;
  likesCount: number;
  dislikesCount: number;
}

export interface Video {
  id: string;
  creatorProfile: CreatorProfile;
  title: string;
  description: string | null;
  thumbnailUrl: string | null;
  previewThumbnailUrl: string | null;
  visibility: VideoVisibility;
  status: VideoStatus;
  metrics: VideoMetrics;
  createDate: string;
  publishDate: string | null;
  lengthSeconds: number;
}

export enum PlaylistVisibility {
  Private = 0,
  Unlisted = 1,
  Public = 2,
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

export interface SimplePlaylistInfo {
  id: string;
  title: string;
  updateDate: string;
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

export interface GetPlaylistInfosResponse {
  totalCount: number;
  infos: PlaylistInfo[];
}

export interface HiddenVideo {
  id: string;
  visibility: VideoVisibility | null;
}

export interface PlaylistItem {
  id: string;
  video: Video | HiddenVideo;
  position: number | null;
  createDate: string;
}

export interface CreatorProfile {
  id: string;
  handle: string | null;
  displayName: string;
  thumbnailUrl: string | null;
}

export enum VideoVisibility {
  Private = 0,
  Unlisted = 1,
  Public = 2,
}

export enum VideoStatus {
  Preparing = 0,
  Ready = 1,
  Published = 2,
}

export interface GetVideosResponse {
  totalCount: number;
  videos: Video[];
}

export interface DetailedUserProfile {
  id: string;
  displayName: string;
  description: string;
  handle: string | null;
  email: string | null;
  thumbnailUrl: string | null;
  createDate: string;
}

export interface UserChannelInfo {
  bannerUrl: string | null;
}

export interface DetailedUserChannelInfo {
  userProfile: DetailedUserProfile;
  userChannelInfo: UserChannelInfo;
}

export interface ChannelData extends DetailedUserChannelInfo {
  subscriptionStatus: SubscriptionStatus | null;
}

export enum ChannelSectionType {
  SpotlightVideo = -1,
  Videos = 0,
  CreatedPlaylists = 1,
  SinglePlaylist = 2,
  MultiplePLaylists = 4,
}

export type ChannelSection =
  | VideosSection
  | CreatedPlaylistsSection
  | SinglePlaylistSection
  | MultiplePlaylistsSection
  | SpotlightVideoSection;

export interface VideosSection {
  type: ChannelSectionType.Videos;
  id: string | null;
}

export interface CreatedPlaylistsSection {
  type: ChannelSectionType.CreatedPlaylists;
  id: string | null;
}

export interface SinglePlaylistSection {
  type: ChannelSectionType.SinglePlaylist;
  id: string | null;
  content: SinglePlaylistSectionContent;
}

export interface MultiplePlaylistsSection {
  type: ChannelSectionType.MultiplePLaylists;
  id: string | null;
  content: MultiplePlaylistSectionContent;
}

export interface SpotlightVideoSection {
  type: ChannelSectionType.SpotlightVideo;
  id: string | null;
  content: SpotlightVideoSectionContent;
}

export interface SinglePlaylistSectionContent {
  playlistId: string;
}

export interface MultiplePlaylistSectionContent {
  title: string;
  playlistIds: string[];
}

export interface SpotlightVideoSectionContent {
  videoId: string;
}

export interface UserChannel {
  id: string;
  handle: string | null;
  unsubscribedSpotlightVideoId: string | null;
  subscribedSpotlightVideoId: string | null;
  sections: ChannelSection[];
}

export interface UserChannelInstance {
  id: string;
  handle: string | null;
  sections: ChannelSectionInstance[];
}

export type ChannelSectionInstance =
  | VideosSectionInstance
  | CreatedPlaylistsSectionInstance
  | SinglePlaylistSectionInstance
  | MultiplePlaylistsSectionInstance
  | SpotlightVideoSectionInstance;

export interface VideosSectionInstance {
  type: ChannelSectionType.Videos;
  id: string | null;
  videos: Video[];
}

export interface CreatedPlaylistsSectionInstance {
  type: ChannelSectionType.CreatedPlaylists;
  id: string | null;
  playlists: PlaylistInfo[];
}

export interface SinglePlaylistSectionInstance {
  type: ChannelSectionType.SinglePlaylist;
  id: string | null;
  playlistId: string;
  playlist: Playlist;
}

export interface MultiplePlaylistsSectionInstance {
  type: ChannelSectionType.MultiplePLaylists;
  id: string | null;
  title: string;
  playlistIds: string[];
  playlists: PlaylistInfo[];
}

export interface SpotlightVideoSectionInstance {
  type: ChannelSectionType.SpotlightVideo;
  id: string | null;
  videoId: string;
  video: Video;
}

export interface GetLibraryResourcesRequest {
  targetUserId: string;
  requireUploadedVideos: boolean;
  requireCreatedPlaylistInfos: boolean;
  requireVideos: string[] | null;
  requirePlaylists: string[] | null;
  requirePlaylistInfos: string[] | null;
  maxUploadedVideosCount: number | null;
  maxCreatedPlaylistsCount: number | null;
  maxPlaylistItemsCount: number | null;
}

export interface GetLibraryResourcesResponse {
  uploadedVideos: Video[] | null;
  createdPlaylistInfos: PlaylistInfo[] | null;
  videos: Video[] | null;
  playlists: Playlist[] | null;
  playlistInfos: PlaylistInfo[] | null;
}
