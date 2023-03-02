import { GetLibraryResourcesResponse } from '../models/channel';
import { map, Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  ChannelSectionType,
  DetailedUserChannelInfo,
  GetLibraryResourcesRequest,
  GetPlaylistInfosResponse,
  GetVideosResponse,
  Playlist,
  PlaylistInfo,
  SimplePlaylistInfo,
  UserChannel,
  Video,
} from '../models/channel';
import { environment } from 'src/environments/environment';

@Injectable()
export class ChannelHelperService {
  constructor(private httpClient: HttpClient) {}

  getChannel(
    userId: string,
    maxSectionItemsCount: number = 12,
    sectionType?: ChannelSectionType
  ) {
    const url = environment.appSetup.apiUrl + '/api/v1/UserChannels';

    let params = new HttpParams({
      fromObject: {
        userId,
        maxSectionItemsCount,
      },
    });

    if (sectionType != null) {
      params = params.append('sectionType', sectionType);
    }

    return this.httpClient.get<UserChannel>(url, {
      params,
    });
  }

  getChannelInfo(userId?: string, handle?: string) {
    if (!userId && !handle) {
      throw new Error('no user id and handle provided');
    }

    const url = environment.appSetup.apiUrl + '/api/v1/UserChannels/Info';

    let params = new HttpParams();
    if (!!userId) params = params.append('userId', userId);
    if (!!handle) params = params.append('handle', handle);

    return this.httpClient.get<DetailedUserChannelInfo>(url, {
      params,
    });
  }

  getResources(request: GetLibraryResourcesRequest) {
    const url = environment.appSetup.apiUrl + '/api/v1/Library';

    return this.httpClient.post<GetLibraryResourcesResponse>(url, request);
  }

  getVideo(videoId: string): Observable<Video | null> {
    const url = environment.appSetup.apiUrl + `/api/v1/VideoLibrary/${videoId}`;

    return this.httpClient.get<Video | null>(url);
  }

  getVideos(
    userId: string,
    page?: number,
    pageSize?: number
  ): Observable<GetVideosResponse> {
    const url = environment.appSetup.apiUrl + '/api/v1/VideoLibrary';

    let params = new HttpParams({
      fromObject: {
        userId,
      },
    });

    if (!!page && !!pageSize) {
      params = params.append('page', page).append('pageSize', pageSize);
    }

    return this.httpClient.get<GetVideosResponse>(url, {
      params,
    });
  }

  getPlaylist(
    playlistId: string,
    page?: number,
    pageSize?: number
  ): Observable<Playlist> {
    const url = environment.appSetup.apiUrl + '/api/v1/PlaylistLibrary';

    let params = new HttpParams({
      fromObject: {
        playlist: playlistId,
      },
    });

    if (!!page && !!pageSize) {
      params = params.append('page', page).append('pageSize', pageSize);
    }

    return this.httpClient.get<Playlist>(url, {
      params,
    });
  }

  getCreatedPlaylistInfos(
    userId: string,
    page?: number,
    pageSize?: number
  ): Observable<GetPlaylistInfosResponse> {
    const url =
      environment.appSetup.apiUrl + '/api/v1/PlaylistLibrary/Infos/User';

    let params = new HttpParams({
      fromObject: {
        userId,
      },
    });

    if (!!page && !!pageSize) {
      params = params.append('page', page).append('pageSize', pageSize);
    }

    return this.httpClient.get<GetPlaylistInfosResponse>(url, {
      params,
    });
  }

  getPublicPlaylistInfos(playlistIds: string[]): Observable<PlaylistInfo[]> {
    const url = environment.appSetup.apiUrl + '/api/v1/PlaylistLibrary/Infos';

    return this.httpClient
      .post<PlaylistInfo[]>(url, {
        playlistIds,
      })
      .pipe(
        map((infos) => {
          const result = playlistIds
            .map((id) => infos.find((info) => info.id == id)!)
            .filter((x) => !!x);

          return result;
        })
      );
  }

  getPublicSimplePlaylistInfos(
    playlistIds: string[]
  ): Observable<SimplePlaylistInfo[]> {
    const url =
      environment.appSetup.apiUrl + '/api/v1/PlaylistLibrary/SimpleInfos';

    return this.httpClient
      .post<SimplePlaylistInfo[]>(url, {
        playlistIds,
      })
      .pipe(
        map((infos) => {
          const result = playlistIds
            .map((id) => infos.find((info) => info.id == id)!)
            .filter((x) => !!x);

          return result;
        })
      );
  }

  getTotalViewsCount(userId: string) {
    const url = environment.appSetup.apiUrl + '/api/v1/VideoLibrary/TotalViews';

    let params = new HttpParams({
      fromObject: {
        userId,
      },
    });

    return this.httpClient.get<number>(url, {
      params,
    });
  }
}
