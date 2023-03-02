import { AddToPlaylistDialogService } from '../../../shared/add-to-playlist-dialog/add-to-playlist-dialog.service';
import { Observable } from 'rxjs';
import { VoteType } from '../../../community/video-forum/models';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { VideoMetadata, VoteVideoRequest } from 'src/app/core/models/library';

@Injectable()
export class VideoActionService {
  constructor(
    private httpClient: HttpClient,
    private dialogService: AddToPlaylistDialogService
  ) {}

  getVideoMetadata(videoId: string): Observable<VideoMetadata> {
    const url =
      environment.appSetup.apiUrl + `/api/v1/VideoLibrary/${videoId}/Metadata`;

    return this.httpClient.get<VideoMetadata>(url);
  }

  voteVideo(videoId: string, voteType: VoteType): Observable<void> {
    const url = environment.appSetup.apiUrl + '/api/v1/VideoLibrary/Vote';

    const request: VoteVideoRequest = {
      videoId,
      voteType,
    };

    return this.httpClient.post<void>(url, request);
  }

  openAddToPlaylistDialog(videoId: string, itemId?: string) {
    this.dialogService.openAddToPlaylistDialog(videoId, itemId);
  }
}
