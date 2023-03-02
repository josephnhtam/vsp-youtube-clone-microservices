import {createAction, props} from '@ngrx/store';
import {Playlist} from '../models/library';

export const playlistLoaded = createAction(
  '[playlist / api] playlist loaded',
  props<{
    playlist: Playlist;
    pageSize: number;
  }>()
);

export const failedToLoadVideos = createAction(
  '[playlist / api] failed to load videos',
  props<{ playlistId: string; error: any }>()
);

export const failedToLoadPlaylist = createAction(
  '[playlist / api] failed to load playlist',
  props<{ playlistId: string; error: any }>()
);

export const videosLoaded = createAction(
  '[playlist / api] videos loaded',
  props<{ playlist: Playlist; page: number }>()
);

export const playlistItemRemoved = createAction(
  '[playlist / api] playlist item removed',
  props<{ playlistId: string; itemId: string }>()
);
