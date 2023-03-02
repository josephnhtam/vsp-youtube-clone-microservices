import {Video} from '../../models';

export interface VideoClient {
  video?: Video;
  contextId?: number;
  pending: boolean;
  videoUploadToken: string | null;
  error: any | null;
  processError: any | null;
}
