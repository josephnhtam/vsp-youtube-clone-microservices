import {createAction, props} from '@ngrx/store';
import {Playlist} from '../models/library';

export const playlistLoaded = createAction(
  '[video playlist / api] load playlist',
  props<{
    playlistId: string;
    playlist: Playlist;
    index: number | null;
    videoId: string;
    pageSize: number;
  }>()
);

export const failedToLoadPlaylist = createAction(
  '[video playlist / api] failed to load playlist',
  props<{ playlistId: string; error: any }>()
);
