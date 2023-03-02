export interface GetSimplePlaylistInfosResponse {
  totalCount: number;
  infos: SimplePlaylistInfo[];
}

export interface SimplePlaylistInfo {
  id: string;
  title: string;
  updateDate: string;
  parsedUpdateDate: Date;
}

export interface CreatorProfile {
  id: string;
  handle: string | null;
  displayName: string;
  thumbnailUrl: string | null;
}

export enum PlaylistVisibility {
  Private = 0,
  Unlisted = 1,
  Public = 2,
}

export interface CreatePlaylistRequest {
  title: string;
  description: string;
  visibility: PlaylistVisibility;
}

export interface UpdatePlaylistRequest {
  playlistId: string;
  title: string | null;
  description: string | null;
  visibility: PlaylistVisibility | null;
}

export interface AddPlaylistToLibraryRequest {
  playlistId: string;
}

export interface GetPlaylistRefResponse {
  exists: boolean;
  createDate: string | null;
}
