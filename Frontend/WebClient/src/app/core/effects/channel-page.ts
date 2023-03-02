import {
  ChannelPageContent,
  ChannelPageContentType,
  ChannelPageRequest,
  ChannelPageType,
  PlaylistPageContent,
  PlaylistsPageContent,
  VideosPageContent,
} from '../reducers/channel-page';
import {catchError, concatMap, filter, map, Observable, of} from 'rxjs';
import {ChannelHelperService} from 'src/app/core/services/channel-helper.service';
import {Store} from '@ngrx/store';
import {Actions, concatLatestFrom, createEffect, ofType} from '@ngrx/effects';
import {Injectable} from '@angular/core';
import {ChannelPageAction, ChannelPageApiAction} from '../actions';
import {selectChannelPage} from '../selectors/channel-page';

@Injectable()
export class ChannelPageEffects {
  constructor(
    private actions: Actions,
    private store: Store,
    private service: ChannelHelperService
  ) {}

  loadMoreChannelPageResultsEffect$ = createEffect(() =>
    this.actions.pipe(
      ofType(ChannelPageAction.loadMoreResults),

      concatLatestFrom(({ userId, contextId }) =>
        this.store.select(selectChannelPage(userId, contextId))
      ),

      filter(([_, state]) => {
        return state != null;
      }),

      concatMap(([{ userId, contextId }, state]) => {
        const page = state!.currentPage + 1;
        const pageSize = state!.pageSize;
        const request = state!.request;

        return this.loadChannelPage(
          userId,
          contextId,
          page,
          pageSize,
          request
        ).pipe(
          map((content) => {
            return ChannelPageApiAction.channelPageObtained({
              userId,
              contextId,
              request,
              currentPage: page,
              pageSize,
              content,
            });
          }),

          catchError((error) => {
            console.error(error);
            return of(
              ChannelPageApiAction.failedToObtainChannelPage({
                userId,
                contextId,
                error,
              })
            );
          })
        );
      })
    )
  );

  loadChannelPageEffect$ = createEffect(() =>
    this.actions.pipe(
      ofType(ChannelPageAction.loadChannelPage),

      concatMap(({ userId, contextId, request, pageSize }) => {
        return this.loadChannelPage(
          userId,
          contextId,
          1,
          pageSize,
          request
        ).pipe(
          map((content) => {
            return ChannelPageApiAction.channelPageObtained({
              userId,
              contextId,
              request,
              currentPage: 1,
              pageSize,
              content,
            });
          }),

          catchError((error) => {
            console.error(error);
            return of(
              ChannelPageApiAction.failedToObtainChannelPage({
                userId,
                contextId,
                error,
              })
            );
          })
        );
      })
    )
  );

  loadChannelPage(
    userId: string,
    contextId: string,
    page: number,
    pageSize: number,
    request: ChannelPageRequest
  ): Observable<ChannelPageContent> {
    switch (request.type) {
      case ChannelPageType.UploadedVideos:
        return this.service.getVideos(userId, page, pageSize).pipe(
          map((response) => {
            const result: VideosPageContent = {
              type: ChannelPageContentType.Videos,
              items: response.videos,
              totalCount: response.totalCount,
            };

            return result;
          })
        );

      case ChannelPageType.CreatedPlaylists:
        return this.service
          .getCreatedPlaylistInfos(userId, page, pageSize)
          .pipe(
            map((response) => {
              const result: PlaylistsPageContent = {
                type: ChannelPageContentType.Playlists,
                items: response.infos,
                totalCount: response.totalCount,
              };

              return result;
            })
          );

      case ChannelPageType.SinglePlaylist:
        return this.service
          .getPlaylist(request.playlistId, page, pageSize)
          .pipe(
            map((playlist) => {
              const result: PlaylistPageContent = {
                type: ChannelPageContentType.Playlist,
                playlist,
                items: playlist.items
                  .map((x) => x.video)
                  .filter((x) => 'title' in x) as any,
                totalCount: playlist.itemsCount,
              };

              return result;
            })
          );

      case ChannelPageType.MultiplePlaylists: {
        if (request.playlistIds.length == 0) {
          const result: PlaylistsPageContent = {
            type: ChannelPageContentType.Playlists,
            items: [],
            totalCount: 0,
          };

          return of(result);
        }

        const startIndex = Math.max(0, (page - 1) * pageSize);

        const endIndex = Math.min(
          startIndex + pageSize,
          request.playlistIds.length
        );

        const playlistIds = request.playlistIds.slice(startIndex, endIndex);

        return this.service.getPublicPlaylistInfos(playlistIds).pipe(
          map((infos) => {
            const result: PlaylistsPageContent = {
              type: ChannelPageContentType.Playlists,
              items: infos,
              totalCount: request.playlistIds.length,
            };

            return result;
          })
        );
      }
    }
  }
}
