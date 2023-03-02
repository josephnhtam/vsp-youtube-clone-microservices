import {CreatorProfile} from '../models/video';
import {PlaylistItem, PlaylistItemClient, PlaylistVisibility} from '../models/library';
import {createEntityAdapter, EntityState} from '@ngrx/entity';
import {createReducer, on} from '@ngrx/store';
import {PlaylistAction, PlaylistApiAction} from '../actions';
import {PlaylistManagementAction} from 'src/app/core/actions';

export interface PlaylistInfo {
  title: string | null;
  description: string | null;
  visibility: PlaylistVisibility | null;
  createDate: Date;
  updateDate: Date;
  creatorProfile: CreatorProfile;
}

export interface State extends EntityState<PlaylistItemClient> {
  id: string | null;
  playlistInfo: PlaylistInfo | null;

  loaded: boolean;
  pending: boolean;
  loadingMoreVideos: boolean;
  error: any | null;
  currentPage: number;
  pageSize: number;
  videosCount: number;
  obtainedLastVideo: boolean;
}

export const adapter = createEntityAdapter<PlaylistItemClient>({
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
  loadingMoreVideos: false,
  error: null,
  currentPage: 1,
  pageSize: 20,
  videosCount: 0,
  obtainedLastVideo: false,
});

export const reducer = createReducer<State>(
  initialState,

  on(
    PlaylistManagementAction.updatePlaylist,
    (state, { title, description, visibility }) => {
      if (state.playlistInfo == null) return state;

      const newTitle = title ?? state.playlistInfo.title;
      const newDescription = description ?? state.playlistInfo.description;
      const newVisibility = visibility ?? state.playlistInfo.visibility;

      return {
        ...state,
        playlistInfo: {
          ...state.playlistInfo,
          title: newTitle,
          description: newDescription,
          visibility: newVisibility,
        },
      };
    }
  ),

  on(PlaylistApiAction.playlistItemRemoved, (state, { itemId }) => {
    let position = state.entities[itemId]!.position;

    return adapter.map(
      (x) => {
        if (position != null && x.position != null && x.position > position) {
          return {
            ...x,
            position: x.position - 1,
          };
        }
        return x;
      },
      adapter.removeOne(itemId, {
        ...state,
        pending: false,
        videosCount: state.videosCount - 1,
        currentPage: state.currentPage - 1,
      })
    );
  }),

  on(
    PlaylistAction.removePlaylistItem,
    PlaylistAction.removePlaylistVideo,
    (state) => {
      return {
        ...state,
        pending: true,
      };
    }
  ),

  on(PlaylistApiAction.failedToLoadVideos, (state, { playlistId, error }) => {
    if (playlistId != state.id) {
      return state;
    }

    return {
      ...state,
      obtainedLastVideo: true,
      loadingMoreVideos: false,
      pending: false,
      error,
    };
  }),

  on(PlaylistAction.movePlaylistItemToTop, (state, { itemId }) => {
    let fromPosition = state.entities[itemId]!.position;

    if (fromPosition == null) {
      return state;
    }

    return adapter.updateOne(
      {
        id: itemId,
        changes: {
          position: 0,
        },
      },
      adapter.map((x) => {
        if (x.position! < fromPosition! && x.position! >= 0) {
          return {
            ...x,
            position: x.position! + 1,
          };
        } else {
          return x;
        }
      }, state)
    );
  }),

  on(PlaylistAction.movePlaylistItemToBottom, (state, { itemId }) => {
    let fromPosition = state.entities[itemId]!.position;

    if (fromPosition == null) {
      return state;
    }

    let toPosition = state.videosCount - 1;

    const newState = adapter.map(
      (x) => {
        if (x.position! > fromPosition! && x.position! <= toPosition) {
          return {
            ...x,
            position: x.position! - 1,
          };
        } else {
          return x;
        }
      },
      {
        ...state,
        currentPage: state.currentPage - 1,
      }
    );

    if (state.ids.length >= state.videosCount) {
      return adapter.updateOne(
        {
          id: itemId,
          changes: {
            position: state.videosCount - 1,
          },
        },
        newState
      );
    } else {
      return adapter.removeOne(itemId, newState);
    }
  }),

  on(PlaylistAction.movePlaylistItem, (state, { itemId, precedingItemId }) => {
    let fromPosition = state.entities[itemId]!.position;

    if (fromPosition == null) {
      return state;
    }

    let toPosition = precedingItemId
      ? state.entities[precedingItemId]!.position!
      : -1;

    if (fromPosition > toPosition) {
      toPosition++;
    }

    if (fromPosition < toPosition) {
      return adapter.updateOne(
        {
          id: itemId,
          changes: {
            position: toPosition,
          },
        },
        adapter.map((x) => {
          if (x.position! > fromPosition! && x.position! <= toPosition) {
            return {
              ...x,
              position: x.position! - 1,
            };
          } else {
            return x;
          }
        }, state)
      );
    } else if (fromPosition > toPosition) {
      return adapter.updateOne(
        {
          id: itemId,
          changes: {
            position: toPosition,
          },
        },
        adapter.map((x) => {
          if (x.position! < fromPosition! && x.position! >= toPosition) {
            return {
              ...x,
              position: x.position! + 1,
            };
          } else {
            return x;
          }
        }, state)
      );
    } else {
      return state;
    }
  }),

  on(PlaylistApiAction.videosLoaded, (state, { playlist, page }) => {
    const items = convertPlaylistItemsToPlaylistItemClients(playlist.items);

    return adapter.upsertMany(items, {
      ...state,
      currentPage: page,
      pending: false,
      error: null,
      loadingMoreVideos: false,
      obtainedLastVideo: items.length === 0,
      videosCount: playlist.itemsCount,
    });
  }),

  on(PlaylistAction.loadMoreVideos, (state) => {
    return {
      ...state,
      pending: true,
      error: null,
      loadingMoreVideos: true,
    };
  }),

  on(PlaylistApiAction.playlistLoaded, (state, { playlist, pageSize }) => {
    const playlistItemClients = convertPlaylistItemsToPlaylistItemClients(
      playlist.items
    );

    return adapter.setAll(playlistItemClients, {
      ...state,
      // id: playlist.id,
      playlistInfo: {
        title: playlist.title,
        description: playlist.description,
        visibility: playlist.visibility,
        createDate: new Date(playlist.createDate),
        updateDate: new Date(playlist.updateDate),
        creatorProfile: playlist.creatorProfile,
      },
      pending: false,
      loaded: true,
      error: null,
      currentPage: 1,
      pageSize,
      loadingMoreVideos: false,
      videosCount: playlist.itemsCount,
    });
  }),

  on(PlaylistApiAction.failedToLoadPlaylist, (state, { playlistId, error }) => {
    if (playlistId != state.id) {
      return state;
    }

    return {
      ...state,
      pending: false,
      error,
    };
  }),

  on(PlaylistAction.loadPlaylist, (state, { playlistId, pageSize }) => {
    return {
      ...state,
      id: playlistId,
      pending: true,
      loadingMoreVideos: false,
      loaded: false,
      error: null,
      currentPage: 1,
      pageSize,
      videosCount: 0,
    };
  })
);

function convertPlaylistItemsToPlaylistItemClients(
  playlistItems: PlaylistItem[]
): PlaylistItemClient[] {
  return playlistItems.map((x) => {
    return {
      ...x,
      pending: false,
    };
  });
}
