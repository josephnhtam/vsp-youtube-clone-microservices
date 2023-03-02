import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  AddVideoToPlaylistRequest,
  PlaylistsWithVideo,
} from '../../core/models/library';
import { environment } from 'src/environments/environment';

@Injectable()
export class AddToPlaylistActionService {
  constructor(private httpClient: HttpClient) {}

  getPlaylistsWithVideo(videoId: string): Observable<PlaylistsWithVideo> {
    const url =
      environment.appSetup.apiUrl + '/api/v1/PlaylistLibrary/WithVideo';

    const params = new HttpParams({
      fromObject: {
        videoId,
      },
    });

    return this.httpClient.get<PlaylistsWithVideo>(url, {
      params,
    });
  }

  addVideoToPlaylist(videoId: string, playlist: string): Observable<void> {
    const url = environment.appSetup.apiUrl + '/api/v1/PlaylistLibrary/Items';

    const request: AddVideoToPlaylistRequest = {
      videoId,
      playlist,
    };

    return this.httpClient.post<void>(url, request);
  }

  removeItemFromPlaylist(itemId: string, playlist: string): Observable<void> {
    const url = environment.appSetup.apiUrl + '/api/v1/PlaylistLibrary/Items';

    const params = new HttpParams({
      fromObject: {
        itemId,
        playlist,
      },
    });

    return this.httpClient.delete<void>(url, {
      params,
    });
  }

  removeVideoFromPlaylist(videoId: string, playlist: string): Observable<void> {
    const url =
      environment.appSetup.apiUrl + '/api/v1/PlaylistLibrary/Items/Video';

    const params = new HttpParams({
      fromObject: {
        videoId,
        playlist,
      },
    });

    return this.httpClient.delete<void>(url, {
      params,
    });
  }
}
