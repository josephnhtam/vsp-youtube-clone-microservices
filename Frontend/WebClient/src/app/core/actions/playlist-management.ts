import {createAction, props} from '@ngrx/store';
import {PlaylistVisibility} from '../models/playlist';

export const createPlaylist = createAction(
  '[playlist management] create playlist',
  props<{
    title: string;
    description: string;
    visibility: PlaylistVisibility;
    contextId?: number;
  }>()
);

export const removePlaylist = createAction(
  '[playlist management] remove playlist',
  props<{ playlistId: string }>()
);

export const retrieveSimplePlaylistInfos = createAction(
  '[playlist management] retrieve simple playlist infos',
  props<{ maxCount?: number }>()
);

export const clearSimplePlaylistInfos = createAction(
  '[playlist management] clear simple playlist infos'
);

export const updatePlaylist = createAction(
  '[playlist management] update playlist',
  props<{
    playlistId: string;
    title?: string;
    description?: string;
    visibility?: PlaylistVisibility;
  }>()
);

export const createPlaylistRef = createAction(
  '[playlist management] create playlist ref',
  props<{ playlistId: string; title?: string }>()
);

export const removePlaylistRef = createAction(
  '[playlist management] remove playlist ref',
  props<{ playlistId: string }>()
);

export const checkPlaylistRef = createAction(
  '[playlist management] check playlist ref',
  props<{ playlistId: string; title?: string }>()
);
