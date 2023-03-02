export enum UploadStatus {
  InProgress,
  Successful,
  Failed,
  Cancelled,
}

export interface UploadProcess {
  request: VideoUploadRequest | ImageUploadRequest;
  createDate: Date;
  progress: number;
  status: UploadStatus;
}

export interface ImageUploadRequest {
  type: 'image';
  uploadToken: string;
  file: File;
}

export interface VideoUploadRequest {
  type: 'video';
  videoId: string;
  uploadToken: string;
  file: File;
}

export interface ImageUploadResponse {
  fileId: string;
  category: string;
  contentType: string | null;
  fileSize: number;
  url: string;
  token: string;
}
