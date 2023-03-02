import {createAction, props} from '@ngrx/store';

export const loadPlaylist = createAction(
  '[playlist] load playlist',
  props<{ playlistId: string; pageSize: number }>()
);

export const loadMoreVideos = createAction('[playlist] load more videos');

export const movePlaylistItem = createAction(
  '[playlist] move playlist item',
  props<{ itemId: string; precedingItemId: string | null }>()
);

export const removePlaylistVideo = createAction(
  '[playlist] remove playlist video',
  props<{ itemId: string }>()
);

export const removePlaylistItem = createAction(
  '[playlist] remove playlist item',
  props<{ itemId: string }>()
);

export const movePlaylistItemToTop = createAction(
  '[playlist] move playlist item to top',
  props<{ itemId: string }>()
);

export const movePlaylistItemToBottom = createAction(
  '[playlist] move playlist item to bottom',
  props<{ itemId: string }>()
);
