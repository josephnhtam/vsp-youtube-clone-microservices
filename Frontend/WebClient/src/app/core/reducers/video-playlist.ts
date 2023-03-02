import {createEntityAdapter, EntityState} from '@ngrx/entity';
import {createReducer, on} from '@ngrx/store';
import {VideoPlaylistAction, VideoPlaylistApiAction,} from '../actions';
import {PlaylistItem, PlaylistVisibility} from '../models/library';
import {CreatorProfile} from '../models/video';

export interface PlaylistInfo {
  id: string;
  title: string | null;
  description: string | null;
  visibility: PlaylistVisibility | null;
  createDate: Date;
  updateDate: Date;
  creatorProfile: CreatorProfile;
  itemsCount: number;
}

export interface State extends EntityState<PlaylistItem> {
  id: string | null;
  playlistInfo: PlaylistInfo | null;
  loaded: boolean;
  pending: boolean;
  error: any | null;
}

export const adapter = createEntityAdapter<PlaylistItem>({
  selectId: (x) => x.id,
  sortComparer: (a, b) => {
    if (a.position != null && b.position != null) {
      return a.position - b.position;
    } else {
      return 0;
    }
  },
});

const initialState = adapter.getInitialState({
  id: null,
  playlistInfo: null,

  loaded: false,
  pending: false,
  error: null,
});

export const reducer = createReducer<State>(
  initialState,

  on(VideoPlaylistAction.removePlaylistItem, (state, { itemId }) => {
    const itemToRemove = state.entities[itemId] as PlaylistItem;

    return adapter.removeOne(
      itemId,
      adapter.map(
        (item) => {
          return {
            ...item,
            position:
              item.position! > itemToRemove.position!
                ? item.position! - 1
                : item.position,
          };
        },
        {
          ...state,
        }
      )
    );
  }),

  on(
    VideoPlaylistApiAction.failedToLoadPlaylist,
    (state, { playlistId, error }) => {
      if (state.id !== playlistId) {
        return {
          ...state,
        };
      } else {
        return {
          ...state,
          pending: false,
          error,
        };
      }
    }
  ),

  on(
    VideoPlaylistApiAction.playlistLoaded,
    (state, { playlistId, playlist }) => {
      if (state.id !== playlistId) {
        return {
          ...state,
        };
      }

      const playlistInfo: PlaylistInfo = {
        id: playlist.id,
        title: playlist.title,
        description: playlist.description,
        visibility: playlist.visibility,
        createDate: new Date(playlist.createDate),
        updateDate: new Date(playlist.updateDate),
        creatorProfile: playlist.creatorProfile,
        itemsCount: playlist.itemsCount,
      };

      const items = playlist.items.filter((item) => {
        const oldItem = state.entities[item.id] as PlaylistItem;
        if (oldItem == null) {
          return true;
        }
        if (oldItem.video.visibility != item.video.visibility) {
          return true;
        }
        if (oldItem.position != item.position) {
          return true;
        }
        return false;
      });

      return adapter.upsertMany(items, {
        ...state,
        playlistInfo,
        loaded: true,
        pending: false,
        error: null,
      });
    }
  ),

  on(VideoPlaylistAction.loadPlaylist, (state, { playlistId }) => {
    if (state.id !== playlistId) {
      return adapter.removeAll({
        ...state,
        id: playlistId,
        loaded: false,
        pending: true,
        error: null,
      });
    } else {
      return {
        ...state,
        id: playlistId,
        pending: true,
        error: null,
      };
    }
  }),

  on(VideoPlaylistAction.reset, (state) => {
    return adapter.removeAll({
      ...state,
      loaded: false,
      pending: false,
      error: null,
    });
  })
);
