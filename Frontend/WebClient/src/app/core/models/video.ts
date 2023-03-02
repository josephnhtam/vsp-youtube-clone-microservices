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

export interface ProcessedVideo {
  videoFileId: string;
  label: string;
  width: number;
  height: number;
  size: number;
  lengthSeconds: number;
  url: string;
}

export interface VideoMetrics {
  viewsCount: number;
}

export interface CreatorProfile {
  id: string;
  handle: string | null;
  displayName: string;
  thumbnailUrl: string | null;
}

export interface Video {
  id: string;
  creatorProfile: CreatorProfile;
  title: string;
  description: string;
  tags: string;
  thumbnailUrl: string | null;
  previewThumbnailUrl: string | null;
  visibility: VideoVisibility;
  status: VideoStatus;
  videos: ProcessedVideo[];
  metrics: VideoMetrics;
  createDate: string;
  publishDate: string | null;
}
