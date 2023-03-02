import { AuthService } from 'src/app/auth/services/auth.service';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  catchError,
  concatMap,
  finalize,
  forkJoin,
  Observable,
  of,
  take,
  tap,
} from 'rxjs';
import { environment } from 'src/environments/environment';
import { SearchResponse } from '../../core/models/history';
import {
  GetPlaylistInfosResponse,
  Playlist,
  PlaylistInfo,
} from '../../core/models/library';
import { UserSubscriptionInfo } from '../../core/models/subscription';

@Injectable()
export class LibraryService {
  userWatchRecords: SearchResponse | null = null;
  watchLaterPlaylist: Playlist | null = null;
  likedPlaylist: Playlist | null = null;
  playlistInfos: PlaylistInfo[] | null = null;
  playlistsCount: number | null = null;
  userSubscriptionInfo: UserSubscriptionInfo | null = null;
  publicVideosCount: number | null = null;
  loaded = false;

  private playlistsPage: number = 1;
  private playlistsPageSize: number = 12;
  private loadedLastPlaylistInfos = false;
  private loadingMorePlaylists = false;

  constructor(
    private httpClient: HttpClient,
    private authService: AuthService
  ) {}

  reset() {
    this.userWatchRecords = null;
    this.watchLaterPlaylist = null;
    this.likedPlaylist = null;
    this.playlistInfos = null;
  }

  loadLibrary(
    historySize: number,
    watchLaterSize: number,
    likedSize: number,
    playlistSize: number
  ) {
    this.playlistsPageSize = playlistSize;

    return forkJoin({
      history: this.loadHistory(historySize),
      watchLater: this.loadPlaylist('WL', watchLaterSize),
      liked: this.loadPlaylist('LL', likedSize),
      playlists: this.loadPlaylists(playlistSize),
    }).pipe(
      tap((result) => {
        this.userWatchRecords = result.history;
        this.watchLaterPlaylist = result.watchLater;
        this.likedPlaylist = result.liked;
        this.loaded = true;
      })
    );
  }

  isLoadingMorePlaylists() {
    return this.loadingMorePlaylists;
  }

  canLoadMorePlaylists() {
    if (!this.playlistsCount || !this.playlistInfos) return false;
    return (
      this.loadedLastPlaylistInfos ||
      this.playlistsCount > this.playlistInfos?.length
    );
  }

  loadMorePlaylists() {
    return this.doLoadMorePlaylists();
  }

  doLoadMorePlaylists(): Observable<GetPlaylistInfosResponse | null> {
    if (this.playlistInfos == null || this.loadingMorePlaylists)
      return of(null);

    this.loadingMorePlaylists = true;

    const url =
      environment.appSetup.apiUrl + '/api/v1/PlaylistLibrary/Infos/Self';

    const page = this.playlistsPage + 1;

    let params = new HttpParams({
      fromObject: {
        page,
        pageSize: this.playlistsPageSize,
      },
    });

    return this.httpClient.get<GetPlaylistInfosResponse>(url, { params }).pipe(
      finalize(() => (this.loadingMorePlaylists = false)),

      tap((response) => {
        for (let info of response.infos) {
          if (!this.playlistInfos?.find((x) => x.id === info.id)) {
            this.playlistInfos?.push(info);
          }
        }

        this.playlistsPage = page;
        this.playlistsCount = response.totalCount;
        this.loadedLastPlaylistInfos =
          response.infos.length < this.playlistsPageSize;
      }),

      catchError((error) => {
        console.error(error);
        return of(null);
      })
    );
  }

  loadPlaylists(
    playlistCount?: number
  ): Observable<GetPlaylistInfosResponse | null> {
    const url =
      environment.appSetup.apiUrl + '/api/v1/PlaylistLibrary/Infos/Self';

    let params = new HttpParams();

    if (!!playlistCount) {
      params = params.append('page', 1);
      params = params.append('pageSize', playlistCount);
    }

    return this.httpClient.get<GetPlaylistInfosResponse>(url, { params }).pipe(
      tap((response) => {
        this.playlistInfos = response.infos ?? null;
        this.playlistsCount = response.totalCount ?? null;
        this.playlistsPage = 1;
        this.loadedLastPlaylistInfos = false;
      }),

      catchError((error) => {
        console.error(error);
        return of(null);
      })
    );
  }

  loadHistory(itemCount: number): Observable<SearchResponse | null> {
    const params = new HttpParams({
      fromObject: {
        pageSize: itemCount,
      },
    });

    const url = environment.appSetup.apiUrl + '/api/v1/UserHistory';

    return this.httpClient
      .get<SearchResponse>(url, {
        params,
      })
      .pipe(
        catchError((error) => {
          console.error(error);
          return of(null);
        })
      );
  }

  loadPlaylist(
    playlist: string,
    itemCount: number
  ): Observable<Playlist | null> {
    const url = environment.appSetup.apiUrl + '/api/v1/PlaylistLibrary';

    const params = new HttpParams({
      fromObject: {
        playlist,
        page: 1,
        pageSize: itemCount,
      },
    });

    return this.httpClient
      .get<Playlist>(url, {
        params,
      })
      .pipe(
        catchError((error) => {
          console.error(error);
          return of(null);
        })
      );
  }

  getUserSubscriptionInfo(): Observable<UserSubscriptionInfo | null> {
    const url = environment.appSetup.apiUrl + '/api/v1/Subscriptions/Info';

    return this.httpClient.get<UserSubscriptionInfo>(url).pipe(
      tap((info) => {
        this.userSubscriptionInfo = info;
      }),

      catchError((error) => {
        console.error(error);
        return of(null);
      })
    );
  }

  getPublicVideosCount(): Observable<number | null> {
    return this.authService.authInfo$.pipe(
      take(1),
      concatMap((info) => {
        if (!info) return of(null);

        const url = environment.appSetup.apiUrl + '/api/v1/VideoLibrary/Count';

        const params = new HttpParams({
          fromObject: {
            userId: info?.sub,
          },
        });

        return this.httpClient.get<number>(url, { params }).pipe(
          tap((result) => {
            this.publicVideosCount = result;
          }),

          catchError((error) => {
            console.error(error);
            return of(null);
          })
        );
      })
    );
  }
}
