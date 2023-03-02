import {Video} from '../models';

export interface CreateVideoRequestDto {
  title: string;
  description: string;
}

export interface VideoUploadTokenResponseDto {
  videoUploadToken: string;
}

export interface GetVideosResponseDto {
  videos: Video[];
  totalCount: number;
}
