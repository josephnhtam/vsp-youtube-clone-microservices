import {
  GetLibraryResourcesRequest,
  GetLibraryResourcesResponse,
  SpotlightVideoSection,
} from '../models/channel';
import {
  ChannelSectionInstance,
  ChannelSectionType,
  CreatedPlaylistsSectionInstance,
  Playlist,
  PlaylistInfo,
  SinglePlaylistSectionInstance,
  SpotlightVideoSectionInstance,
  Video,
  VideosSectionInstance,
} from '../models/channel';
import {ChannelSection, MultiplePlaylistsSectionInstance,} from 'src/app/core/models/channel';
import {catchError, concatMap, forkJoin, map, of} from 'rxjs';
import {selectChannelSectionInstances} from './../selectors/channel-sections';
import {Actions, concatLatestFrom, createEffect, ofType} from '@ngrx/effects';
import {Store} from '@ngrx/store';
import {Injectable} from '@angular/core';
import {ChannelSectionAction, ChannelSectionApiAction} from '../actions';
import {ChannelHelperService} from 'src/app/core/services/channel-helper.service';

@Injectable()
export class ChannelSectionsEffect {
  constructor(
    private actions$: Actions,
    private store: Store,
    private service: ChannelHelperService
  ) {}

  maxItemsCount = 12;

  createSectionInstancesEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ChannelSectionAction.instantiate),

      concatLatestFrom(({ userId, sections }) =>
        this.store.select(
          selectChannelSectionInstances(
            userId,
            sections
              .map((s) => s.id)
              .filter((id): id is string => {
                return !!id;
              })
          )
        )
      ),

      concatMap(([{ userId, contextId, sections }, existingInstances]) => {
        sections = sections.filter((s) =>
          this.requireUpdate(s, existingInstances)
        );

        return this.instantiate(userId, sections).pipe(
          map(({ sectionInstances }) => {
            return ChannelSectionApiAction.instantiated({
              userId,
              contextId,
              sections,
              sectionInstances,
            });
          })
        );
      })
    )
  );

  private instantiate(userId: string, sections: ChannelSection[]) {
    const spotlightVideoId = this.getSpotlightVideoId(sections);

    const playlistIds = new Array<string>();
    const playlistInfoIds = new Array<string>();

    for (let section of sections) {
      this.collectPlaylistIds(section, playlistIds);
      this.collectPlaylistInfoIds(section, playlistInfoIds);
    }

    const request: GetLibraryResourcesRequest = {
      targetUserId: userId,
      requireUploadedVideos: this.isVideosRequired(sections),
      requireCreatedPlaylistInfos: this.isCreatedPlaylistsRequired(sections),
      requireVideos: spotlightVideoId != null ? [spotlightVideoId] : null,
      requirePlaylists: playlistIds,
      requirePlaylistInfos: playlistInfoIds,
      maxUploadedVideosCount: this.maxItemsCount,
      maxCreatedPlaylistsCount: this.maxItemsCount,
      maxPlaylistItemsCount: this.maxItemsCount,
    };

    if (
      !request.requireUploadedVideos &&
      !request.requireCreatedPlaylistInfos &&
      (request.requireVideos?.length ?? 0) == 0 &&
      (request.requirePlaylists?.length ?? 0) == 0 &&
      (request.requirePlaylistInfos?.length ?? 0) == 0
    ) {
      return of({
        sectionInstances: new Array<ChannelSectionInstance>(),
      });
    }

    return this.service.getResources(request).pipe(
      map((results) => {
        const instances = sections
          .map((section) => this.createInstance(results, section))
          .filter((instance): instance is ChannelSectionInstance => !!instance);

        return {
          sectionInstances: instances,
        };
      })
    );
  }

  private createInstance(
    results: GetLibraryResourcesResponse,
    section: ChannelSection
  ) {
    if (section.type === ChannelSectionType.Videos) {
      const videos = results.uploadedVideos;

      if (!videos || videos.length == 0) return null;

      const result: VideosSectionInstance = {
        type: section.type,
        id: section.id,
        videos: videos,
      };

      return result;
    } else if (section.type === ChannelSectionType.CreatedPlaylists) {
      const createdPlaylists = results.createdPlaylistInfos;

      if (!createdPlaylists || createdPlaylists.length == 0) return null;

      const result: CreatedPlaylistsSectionInstance = {
        type: section.type,
        id: section.id,
        playlists: createdPlaylists,
      };

      return result;
    } else if (section.type === ChannelSectionType.SinglePlaylist) {
      const playlistId = section.content.playlistId;
      const playlist = results.playlists?.find(
        (x) => x.id == section.content.playlistId
      );

      if (!playlist || playlist.items.length == 0) return null;

      const result: SinglePlaylistSectionInstance = {
        type: section.type,
        id: section.id,
        playlistId,
        playlist,
      };

      return result;
    } else if (section.type === ChannelSectionType.MultiplePLaylists) {
      const playlistInfos = results.playlistInfos;

      const playlistIds = section.content.playlistIds;
      const playlists = playlistIds
        .map((id) => playlistInfos?.find((info) => info.id == id))
        .filter((info): info is PlaylistInfo => !!info);

      if (playlists.length == 0) return null;

      const result: MultiplePlaylistsSectionInstance = {
        type: section.type,
        id: section.id,
        title: section.content.title,
        playlistIds,
        playlists,
      };

      return result;
    } else if (section.type === ChannelSectionType.SpotlightVideo) {
      if (!results.videos || results.videos.length == 0) return null;

      const videoId = section.content.videoId;
      const video = results.videos[0];

      const result: SpotlightVideoSectionInstance = {
        type: section.type,
        id: section.id,
        videoId,
        video,
      };

      return result;
    }

    return null;
  }

  private loadVideo(videoId: string) {
    return this.service.getVideo(videoId).pipe(
      catchError((error) => {
        console.error(error);
        return of(null);
      })
    );
  }

  private loadVideos(userId: string) {
    return this.service.getVideos(userId, 1, this.maxItemsCount).pipe(
      map((result) => result.videos),
      catchError((error) => {
        console.error(error);
        return of(new Array<Video>());
      })
    );
  }

  private loadCreatedPlaylists(userId: string) {
    return this.service
      .getCreatedPlaylistInfos(userId, 1, this.maxItemsCount)
      .pipe(
        map((result) => result.infos),
        catchError((error) => {
          console.error(error);
          return of(new Array<PlaylistInfo>());
        })
      );
  }

  private loadPlaylists(playlistIds: string[]) {
    playlistIds = makeUnique(playlistIds);

    if (playlistIds.length == 0) {
      return of(new Array<Playlist>());
    }

    return forkJoin(
      playlistIds.map((playlistId) =>
        this.service.getPlaylist(playlistId, 1, this.maxItemsCount).pipe(
          catchError((error) => {
            console.error(error);
            return of(null);
          })
        )
      )
    ).pipe(
      map((playlists) => {
        return playlists.filter((playlist): playlist is Playlist => !!playlist);
      })
    );
  }

  private loadPlaylistInfos(playlistInfoIds: string[]) {
    playlistInfoIds = makeUnique(playlistInfoIds);

    if (playlistInfoIds.length == 0) {
      return of(new Array<PlaylistInfo>());
    }

    return this.service.getPublicPlaylistInfos(playlistInfoIds);
  }

  private getSpotlightVideoId(sections: ChannelSection[]) {
    const section = sections.find(
      (x) => x.type === ChannelSectionType.SpotlightVideo
    );

    if (section) {
      return (section as SpotlightVideoSection).content.videoId;
    }

    return null;
  }

  private isVideosRequired(sections: ChannelSection[]) {
    return sections.some((x) => x.type === ChannelSectionType.Videos);
  }

  private isCreatedPlaylistsRequired(sections: ChannelSection[]) {
    return sections.some((x) => x.type === ChannelSectionType.CreatedPlaylists);
  }

  private collectPlaylistIds(section: ChannelSection, playlistIds: string[]) {
    if (section.type === ChannelSectionType.SinglePlaylist) {
      playlistIds.push(section.content.playlistId);
    }
  }

  private collectPlaylistInfoIds(
    section: ChannelSection,
    playlistInfoIds: string[]
  ) {
    if (section.type === ChannelSectionType.MultiplePLaylists) {
      playlistInfoIds.push(...section.content.playlistIds);
    }
  }

  private requireUpdate(
    section: ChannelSection,
    existingInstances?: ChannelSectionInstance[]
  ) {
    const existingSection = existingInstances?.find(
      (x) => x.id == section.id && x.type == section.type
    );

    if (!existingSection) return true;

    if (section.type === ChannelSectionType.SinglePlaylist) {
      const _existingSection = existingSection as SinglePlaylistSectionInstance;

      if (section.content.playlistId != _existingSection.playlistId) {
        return true;
      }
    }

    if (section.type === ChannelSectionType.MultiplePLaylists) {
      const _existingSection =
        existingSection as MultiplePlaylistsSectionInstance;

      if (
        section.content.playlistIds.length !=
        _existingSection.playlistIds.length
      ) {
        return true;
      }

      for (let i = 0; i < section.content.playlistIds.length; ++i) {
        if (section.content.playlistIds[i] != _existingSection.playlistIds[i]) {
          return true;
        }
      }
    }

    return false;
  }
}

function makeUnique(array: any[]) {
  return array.filter(onlyUnique);
}

function onlyUnique(value: any, index: number, array: any[]) {
  return array.indexOf(value) === index;
}
