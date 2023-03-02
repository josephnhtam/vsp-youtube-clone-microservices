import {createEntityAdapter, EntityState} from '@ngrx/entity';
import {createReducer, on} from '@ngrx/store';
import {PlaylistManagementAction, PlaylistManagementApiAction,} from '../actions';
import {SimplePlaylistInfo} from '../models/playlist';

export interface State extends EntityState<SimplePlaylistInfo> {
  retrievingInfos: boolean;
}

export const adapter = createEntityAdapter<SimplePlaylistInfo>({
  selectId: (x) => x.id,
  sortComparer: (a, b) => {
    return b.parsedUpdateDate.getTime() - a.parsedUpdateDate.getTime();
  },
});

const initialState: State = adapter.getInitialState({
  retrievingInfos: false,
});

export const reducer = createReducer<State>(
  initialState,

  on(
    PlaylistManagementApiAction.playlistRefChecked,
    (state, { playlistId, exists, simplePlaylistInfo }) => {
      if (exists) {
        if (simplePlaylistInfo) {
          return adapter.upsertOne(simplePlaylistInfo, {
            ...state,
          });
        } else {
          return {
            ...state,
          };
        }
      } else {
        return adapter.removeOne(playlistId, {
          ...state,
        });
      }
    }
  ),

  on(
    PlaylistManagementApiAction.playlistRefRemoved,
    (state, { playlistId }) => {
      return adapter.removeOne(playlistId, {
        ...state,
      });
    }
  ),

  on(
    PlaylistManagementApiAction.playlistRefCreated,
    (state, { simplePlaylistInfo }) => {
      if (simplePlaylistInfo != null) {
        return adapter.upsertOne(simplePlaylistInfo, {
          ...state,
        });
      } else {
        return {
          ...state,
        };
      }
    }
  ),

  on(PlaylistManagementApiAction.playlistRemoved, (state, { playlistId }) => {
    return adapter.removeOne(playlistId, { ...state });
  }),

  on(
    PlaylistManagementApiAction.playlistUpdated,
    (state, { playlistId, title }) => {
      let changes: any = {};

      if (!!title) {
        changes.title = title;
      }

      return adapter.updateOne(
        {
          id: playlistId,
          changes,
        },
        { ...state }
      );
    }
  ),

  on(PlaylistManagementApiAction.playlistRemoved, (state, { playlistId }) => {
    return adapter.removeOne(playlistId, {
      ...state,
    });
  }),

  on(
    PlaylistManagementApiAction.playlistCreated,
    (state, { simplePlaylistInfo }) => {
      return adapter.upsertOne(simplePlaylistInfo, {
        ...state,
      });
    }
  ),

  on(
    PlaylistManagementApiAction.failedToRetrieveSimplePlaylistInfos,
    (state) => {
      return {
        ...state,
        retrievingInfos: false,
      };
    }
  ),

  on(
    PlaylistManagementApiAction.simplePlaylistInfosRetrieved,
    (state, { simplePlaylistInfos }) => {
      simplePlaylistInfos = simplePlaylistInfos.map((info) => {
        return {
          ...info,
          parsedUpdateDate: new Date(info.updateDate),
        };
      });

      return adapter.upsertMany(simplePlaylistInfos, {
        ...state,
        retrievingInfos: false,
      });
    }
  ),

  on(PlaylistManagementAction.retrieveSimplePlaylistInfos, (state) => {
    return adapter.removeAll({
      ...state,
      retrievingInfos: true,
    });
  }),

  on(PlaylistManagementAction.clearSimplePlaylistInfos, (state) => {
    return adapter.removeAll({
      ...state,
      retrievingInfos: false,
    });
  })
);
