export enum SearchTarget {
  Video,
  UserProfile,
  Playlist,
}

export enum SearchSort {
  Relevance,
  CreateDate,
  ViewsCount,
  LikesCount,
}

export interface SearchPeriod {
  from: string | null;
  to: string | null;
}

export interface SearchByTagsRequest {
  type: 'tags';
  tags: string;
}

export interface SearchByQueryRequest {
  type: 'query';
  searchTarget: SearchTarget;
  query: string;
  sort: SearchSort | null;
  period: SearchPeriod | null;
}

export interface SearchByCreatorsRequest {
  type: 'creators';
  creatorIds: string[];
  sort: SearchSort | null;
  period: SearchPeriod | null;
}

export interface SearchResponse {
  totalCount: number;
  items: SearchableItem[];
}

export interface SearchableItem {
  id: string;
  title: string;
  creatorProfile: CreatorProfile;
}

export interface Video extends SearchableItem {
  type: 'video';
  description: string | null;
  thumbnailUrl: string | null;
  previewThumbnailUrl: string | null;
  lengthSeconds: number | null;
  metrics: VideoMetrics;
  createDate: string;
}

export interface CreatorProfile {
  id: string;
  handle: string | null;
  displayName: string;
  thumbnailUrl: string | null;
}

export interface UserProfile extends SearchableItem {
  type: 'userProfile';
  displayName: string;
  thumbnailUrl: string | null;
}

export interface VideoMetrics {
  viewsCount: number;
  likesCount: number;
  dislikesCount: number;
}

export function isVideo(item: SearchableItem): item is Video {
  return 'type' in item && (item as any).type === 'video';
}

export function isUserProfile(item: SearchableItem): item is UserProfile {
  return 'type' in item && (item as any).type === 'userProfile';
}

export interface SearchParams {
  query: string;
}
