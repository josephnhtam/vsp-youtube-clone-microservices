import {ChannelSectionType, Playlist, PlaylistInfo, Video,} from 'src/app/core/models/channel';

export type ChannelSectionInstance =
  | VideosSectionInstance
  | CreatedPlaylistsSectionInstance
  | SinglePlaylistSectionInstance
  | MultiplePlaylistsSectionInstance;

export interface VideosSectionInstance {
  type: ChannelSectionType.Videos;
  id: string | null;
  videos: Video[];
}

export interface CreatedPlaylistsSectionInstance {
  type: ChannelSectionType.CreatedPlaylists;
  id: string | null;
  playlists: PlaylistInfo[];
}

export interface SinglePlaylistSectionInstance {
  type: ChannelSectionType.SinglePlaylist;
  id: string | null;
  playlistId: string;
  playlist: Playlist;
}

export interface MultiplePlaylistsSectionInstance {
  type: ChannelSectionType.MultiplePLaylists;
  id: string | null;
  title: string;
  playlistIds: string[];
  playlists: PlaylistInfo[];
}
