import {createAction, props} from '@ngrx/store';

export const loadPlaylist = createAction(
  '[video playlist] load playlist',
  props<{
    playlistId: string;
    index: number | null;
    videoId: string;
    pageSize: number;
  }>()
);

export const reset = createAction('[video playlist] reset');

export const removePlaylistItem = createAction(
  '[video playlist] remove playlist item',
  props<{ itemId: string }>()
);
