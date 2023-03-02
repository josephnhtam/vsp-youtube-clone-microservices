import {environment} from 'src/environments/environment';

export enum VideoStatus {
  Created = 0,
  Registered = 1,
  RegistrationFailed = 2,
  Unregistered = 3,
}

export enum VideoVisibility {
  Private = 0,
  Unlisted = 1,
  Public = 2,
}

export enum VideoProcessingStatus {
  WaitingForUserUpload = 0,
  VideoUploaded = 1,
  VideoBeingProcessed = 2,
  VideoProcessed = 3,
  VideoProcessingFailed = 4,
}

export enum VideoThumbnailStatus {
  Waiting = 0,
  Processed = 1,
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

export interface VideoThumbnail {
  imageFileId: string;
  label: string;
  width: number;
  height: number;
  url: string;
}

export interface VideoMetrics {
  viewsCount: number;
  commentsCount: number;
  likesCount: number;
  dislikesCount: number;
}

export interface Video {
  id: string;
  creatorId: string;
  title: string;
  description: string;
  tags: string;
  thumbnailId: string | null;
  status: VideoStatus;
  visibility: VideoVisibility;
  allowedToPublish: boolean;

  processingStatus: VideoProcessingStatus;
  originalVideoFileId: string;
  originalVideoFileName: string | null;
  originalVideoUrl: string | null;
  videos: ProcessedVideo[];

  thumbnailStatus: VideoThumbnailStatus;
  thumbnails: VideoThumbnail[];

  metrics: VideoMetrics;

  createDate: string;
}

export function getVideoThumbnail(video: Video): VideoThumbnail | undefined {
  if (video.processingStatus == VideoProcessingStatus.VideoProcessingFailed)
    return undefined;

  if (video.thumbnails.length == 0) return undefined;

  let thumbnail: VideoThumbnail | undefined;

  if (video.thumbnailId) {
    thumbnail = video.thumbnails.find(
      (x) => x.imageFileId === video.thumbnailId
    );
  }

  if (!thumbnail) {
    thumbnail = video.thumbnails[0];
  }

  return thumbnail;
}

export enum DetailedVideoStatus {
  WaitingForUserUpload,
  VideoUploaded,
  ProcessingThumbnail,
  ProcessingSD,
  ProcessingHD,
  ProcessingComplete,
  ProcessingFailed,
  RegistrationFailed,
}

export function GetDetailedVideoStatus(video?: Video) {
  if (!!video) {
    if (video.status == VideoStatus.RegistrationFailed) {
      return DetailedVideoStatus.RegistrationFailed;
    }

    switch (video.processingStatus) {
      case VideoProcessingStatus.WaitingForUserUpload: {
        return DetailedVideoStatus.WaitingForUserUpload;
      }

      case VideoProcessingStatus.VideoUploaded: {
        return DetailedVideoStatus.VideoUploaded;
      }

      case VideoProcessingStatus.VideoBeingProcessed: {
        if (video.thumbnails.length === 0) {
          return DetailedVideoStatus.ProcessingThumbnail;
        } else if (
          video.videos.length == 0 ||
          !video.videos.some(
            (x) => x.height > environment.studioSetup.maxSdResolution
          )
        ) {
          return DetailedVideoStatus.ProcessingSD;
        } else {
          return DetailedVideoStatus.ProcessingHD;
        }
      }

      case VideoProcessingStatus.VideoProcessed: {
        return DetailedVideoStatus.ProcessingComplete;
      }

      case VideoProcessingStatus.VideoProcessingFailed: {
        return DetailedVideoStatus.ProcessingFailed;
      }
    }
  }

  return DetailedVideoStatus.WaitingForUserUpload;
}
