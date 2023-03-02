import {VideoStatus, VideoVisibility} from './video';

export interface SearchPeriod {
  from: string | null;
  to: string | null;
}

export interface SearchResponse {
  totalCount: number;
  items: UserWatchRecord[];
}

export interface UserWatchRecord {
  id: string;
  video: Video | HiddenVideo;
  date: string;
  parsedDate: Date;
}

export interface HiddenVideo {
  id: string;
  visibility: VideoVisibility | null;
}

export interface Video {
  id: string;
  creatorProfile: CreatorProfile;
  title: string;
  description: string;
  thumbnailUrl: string | null;
  previewThumbnailUrl: string | null;
  lengthSeconds: number;
  visibility: VideoVisibility;
  status: VideoStatus;
  metrics: VideoMetrics;
  createDate: string;
  publishDate: string | null;
}

export interface CreatorProfile {
  id: string;
  handle: string | null;
  displayName: string;
  thumbnailUrl: string | null;
}

export interface VideoMetrics {
  viewsCount: number;
}

export interface HistoryParams {
  query: string | null;
}

export interface SearchUserWatchHistoryRequest {
  query: string | null;
  period: SearchPeriod | null;
}

export interface UserHistorySettings {
  recordWatchHistory: boolean;
}

export function isVideoAvailable(video: Video | HiddenVideo): video is Video {
  return !!video && 'title' in video;
}
