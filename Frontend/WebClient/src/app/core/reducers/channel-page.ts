import {PlaylistInfo, Video} from 'src/app/core/models/channel';
import {createEntityAdapter, EntityState} from '@ngrx/entity';
import {createReducer, on} from '@ngrx/store';
import {ChannelPageAction, ChannelPageApiAction} from '../actions';
import {Playlist} from '../models/library';

export enum ChannelPageType {
  UploadedVideos,
  CreatedPlaylists,
  SinglePlaylist,
  MultiplePlaylists,
}

export enum ChannelPageContentType {
  Videos,
  Playlist,
  Playlists,
}

export type ChannelPageRequest =
  | UploadedVideosPageRequest
  | CreatedPlaylistsPageRequest
  | SinglePlaylistPageRequest
  | MultiplePlaylistsPageRequest;

export interface UploadedVideosPageRequest {
  type: ChannelPageType.UploadedVideos;
}

export interface CreatedPlaylistsPageRequest {
  type: ChannelPageType.CreatedPlaylists;
}

export interface SinglePlaylistPageRequest {
  type: ChannelPageType.SinglePlaylist;
  playlistId: string;
}

export interface MultiplePlaylistsPageRequest {
  type: ChannelPageType.MultiplePlaylists;
  playlistIds: string[];
}

export type ChannelPageContent =
  | VideosPageContent
  | PlaylistPageContent
  | PlaylistsPageContent;

export interface VideosPageContent {
  type: ChannelPageContentType.Videos;
  items: Video[];
  totalCount: number;
}

export interface PlaylistPageContent {
  type: ChannelPageContentType.Playlist;
  playlist: Playlist;
  items: Video[];
  totalCount: number;
}

export interface PlaylistsPageContent {
  type: ChannelPageContentType.Playlists;
  items: PlaylistInfo[];
  totalCount: number;
}

export interface ChannelPage {
  userId: string;
  contextId: string;
  request: ChannelPageRequest;
  content: ChannelPageContent | null;
  currentPage: number;
  pageSize: number;
  loadedLastItem: boolean;
  loaded: boolean;
  pending: boolean;
  error: any | null;
}

export interface State extends EntityState<ChannelPage> {}

export function getId(userId: string, contextId: string) {
  return userId + ':' + contextId;
}

export const adapter = createEntityAdapter<ChannelPage>({
  selectId: (page) => {
    return getId(page.userId, page.contextId);
  },
});

const initialState = adapter.getInitialState();

export const reducer = createReducer<State>(
  initialState,

  on(
    ChannelPageApiAction.failedToObtainChannelPage,
    (state, { userId, contextId, error }) => {
      const id = getId(userId, contextId);

      return adapter.updateOne(
        {
          id,
          changes: {
            pending: false,
            error,
          },
        },
        {
          ...state,
        }
      );
    }
  ),

  on(
    ChannelPageApiAction.channelPageObtained,
    (state, { userId, contextId, request, currentPage, pageSize, content }) => {
      const id = getId(userId, contextId);

      if (!!state.entities[id]) {
        return adapter.mapOne(
          {
            id,
            map: (page) => {
              let updatedContent: any;

              if (!!page.content && content.type === page.content.type) {
                const newItems = [...page.content.items];

                for (let item of content.items) {
                  if (!newItems.some((x) => x.id == item.id)) {
                    newItems.push(item as any);
                  }
                }

                updatedContent = {
                  ...page.content,
                  totalCount: content.totalCount,
                  items: newItems,
                };
              } else {
                updatedContent = content;
              }

              return {
                ...page,
                currentPage,
                loadedLastItem: content.items.length == 0,
                loaded: true,
                pending: false,
                error: null,
                content: updatedContent,
              };
            },
          },
          {
            ...state,
          }
        );
      } else {
        const page: ChannelPage = {
          userId,
          contextId,
          request,
          currentPage,
          pageSize,
          loadedLastItem: content.items.length == 0,
          loaded: true,
          pending: false,
          error: null,
          content,
        };

        return adapter.addOne(page, {
          ...state,
        });
      }
    }
  ),

  on(ChannelPageAction.loadMoreResults, (state, { userId, contextId }) => {
    const id = getId(userId, contextId);

    return adapter.updateOne(
      {
        id,
        changes: {
          pending: true,
          error: null,
        },
      },
      {
        ...state,
      }
    );
  }),

  on(
    ChannelPageAction.loadChannelPage,
    (state, { userId, contextId, request, pageSize }) => {
      const channelPage: ChannelPage = {
        userId,
        contextId,
        request,
        currentPage: 1,
        pageSize,
        loadedLastItem: false,
        loaded: false,
        pending: true,
        error: null,
        content: null,
      };

      return adapter.upsertOne(channelPage, {
        ...state,
      });
    }
  )
);
