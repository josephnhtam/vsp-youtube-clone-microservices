import {createAction, props} from '@ngrx/store';
import {PlaylistVisibility, SimplePlaylistInfo,} from '../models/playlist';

export const playlistCreated = createAction(
  '[playlist management / api] playlist created',
  props<{
    playlistId: string;
    simplePlaylistInfo: SimplePlaylistInfo;
    contextId?: number;
  }>()
);

export const failedToCreatePlaylist = createAction(
  '[playlist management / api] failed to create playlist',
  props<{ contextId?: number; error: any }>()
);

export const playlistRemoved = createAction(
  '[playlist management / api] playlist removed',
  props<{ playlistId: string; contextId?: number }>()
);

export const failedToRemovePlaylist = createAction(
  '[playlist management / api] failed to remove playlist',
  props<{ playlistId: string; contextId?: number; error: any }>()
);

export const simplePlaylistInfosRetrieved = createAction(
  '[playlist management / api] simple playlist infos retrieved',
  props<{ simplePlaylistInfos: SimplePlaylistInfo[]; totalCount: number }>()
);

export const failedToRetrieveSimplePlaylistInfos = createAction(
  '[playlist management / api] failed to retrieve simple playlist infos'
);

export const playlistUpdated = createAction(
  '[playlist management / api] playlist updated',
  props<{
    playlistId: string;
    title?: string;
    description?: string;
    visibility?: PlaylistVisibility;
  }>()
);

export const failedToUpdatePlaylist = createAction(
  '[playlist management / api] failed to update playlist',
  props<{ playlistId: string; error: any }>()
);

export const playlistRefCreated = createAction(
  '[playlist management / api] playlist ref created',
  props<{ playlistId: string; simplePlaylistInfo: SimplePlaylistInfo | null }>()
);

export const failedToCreatePlaylistRef = createAction(
  '[playlist management / api] failed to create playlist ref',
  props<{ playlistId: string; error: any }>()
);

export const playlistRefRemoved = createAction(
  '[playlist management / api] playlist ref removed',
  props<{ playlistId: string }>()
);

export const failedToRemovePlaylistRef = createAction(
  '[playlist management / api] failed to remove playlist ref',
  props<{ playlistId: string; error: any }>()
);

export const playlistRefChecked = createAction(
  '[playlist management / api] playlist ref checked',
  props<{
    playlistId: string;
    exists: boolean;
    simplePlaylistInfo: SimplePlaylistInfo | null;
  }>()
);

export const failedToCheckPlaylistRef = createAction(
  '[playlist management / api] failed to check playlist ref',
  props<{ playlistId: string; error: any }>()
);
