import {Actions, concatLatestFrom, ofType} from '@ngrx/effects';
import {Store} from '@ngrx/store';
import {filter, switchMap, take, tap} from 'rxjs';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {PlaylistsWithVideo, PlaylistVisibility, SimplePlaylist,} from '../../core/models/library';
import {DialogRef} from '@angular/cdk/dialog';
import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA} from '@angular/material/dialog';
import {AddToPlaylistActionService} from './add-to-playlist-actions.service';
import {PlaylistAction, VideoPlaylistAction} from '../../core/actions';
import {selectPlaylistItems} from '../../core/selectors/playlist';
import {PlaylistManagementAction, PlaylistManagementApiAction,} from 'src/app/core/actions';
import {selectPlaylistState} from "../../core/selectors";

@Component({
  selector: 'app-add-to-playlist-dialog',
  templateUrl: './add-to-playlist-dialog.component.html',
  styleUrls: ['./add-to-playlist-dialog.component.css'],
})
export class AddToPlaylistDialogComponent implements OnInit {
  videoId: string;
  itemId?: string;
  playlists!: PlaylistToAddVideo[];

  isCreatePlaylistOpened = false;
  isCreatingPlaylist = false;
  createPlaylistFormGroup: FormGroup;

  constructor(
    private dialogRef: DialogRef,
    @Inject(MAT_DIALOG_DATA) public data: AddToPlaylistDialogData,
    private actions: AddToPlaylistActionService,
    private store: Store,
    private actions$: Actions
  ) {
    this.videoId = data.videoId;
    this.itemId = data.itemId;
    this.createPlaylists(data);

    const fb = new FormBuilder();
    this.createPlaylistFormGroup = fb.group({
      name: ['', [Validators.required, Validators.maxLength(50)]],
      visibility: [0, [Validators.required]],
    });
  }

  private createPlaylists(data: AddToPlaylistDialogData) {
    this.playlists = [];

    const addPlaylist = (x: SimplePlaylist, isAdded: boolean) => {
      this.playlists.push({
        id: x.id,
        title: x.title ?? '',
        visibility: x.visibility ?? PlaylistVisibility.Private,
        isAdded,
      });
    };

    data.playlists.playlistsWithVideo.forEach((x) => addPlaylist(x, true));
    data.playlists.playlistsWithoutVideo.forEach((x) => addPlaylist(x, false));
    this.playlists.sort((a, b) => a.title.localeCompare(b.title));

    this.playlists = [
      {
        id: 'WL',
        title: 'Water later',
        visibility: PlaylistVisibility.Private,
        isAdded: data.playlists.isAddedToWatchLaterPlaylist,
      },
      ...this.playlists,
    ];
  }

  ngOnInit(): void {}

  close() {
    this.dialogRef.close();
  }

  getVisibilityIcon(visibility: PlaylistVisibility) {
    switch (visibility) {
      case PlaylistVisibility.Private:
        return 'lock';
      case PlaylistVisibility.Unlisted:
        return 'link';
      case PlaylistVisibility.Public:
        return 'public';
    }
  }

  toggleAddingVideoToPlaylist(playlistId: string, value: boolean) {
    const playlist = this.playlists.find((x) => x.id == playlistId);
    if (!playlist) return;

    playlist.isAdded = value;

    if (value) {
      this.actions.addVideoToPlaylist(this.videoId, playlist.id).subscribe();
    } else {
      const videoId = this.videoId;
      const itemId = this.itemId;

      this.removeItemFromVideoPlaylist(itemId);
      this.removeVideoOrItemFromPlaylist(playlistId, videoId, itemId);
    }

    this.itemId = undefined;
  }

  private removeVideoOrItemFromPlaylist(
    playlistId: string,
    videoId: string,
    itemId: string | undefined
  ) {
    this.store
      .select(selectPlaylistState)
      .pipe(
        take(1),

        concatLatestFrom(() => this.store.select(selectPlaylistItems)),

        tap(([state, items]) => {
          if (
            state.id == playlistId &&
            itemId != null &&
            items.some((x) => x.id === itemId)
          ) {
            this.store.dispatch(
              PlaylistAction.removePlaylistItem({ itemId: itemId })
            );
          } else {
            if (itemId != null) {
              this.actions
                .removeItemFromPlaylist(itemId, playlistId)
                .subscribe();
            } else {
              this.actions
                .removeVideoFromPlaylist(videoId, playlistId)
                .subscribe();
            }
          }
        })
      )
      .subscribe();
  }

  private removeItemFromVideoPlaylist(itemId: string | undefined) {
    if (itemId != null) {
      this.store.dispatch(VideoPlaylistAction.removePlaylistItem({ itemId }));
    }
  }

  openCreatePlaylist() {
    this.isCreatePlaylistOpened = true;
  }

  createPlaylist() {
    if (this.createPlaylistFormGroup.invalid || this.isCreatingPlaylist) return;

    this.isCreatingPlaylist = true;

    const formValue = this.createPlaylistFormGroup.value;

    const contextId = Date.now();

    this.store.dispatch(
      PlaylistManagementAction.createPlaylist({
        title: formValue['name'],
        description: '',
        visibility: formValue['visibility'],
        contextId,
      })
    );

    this.actions$
      .pipe(
        ofType(
          PlaylistManagementApiAction.playlistCreated,
          PlaylistManagementApiAction.failedToCreatePlaylist
        ),

        filter((action) => action.contextId === contextId),

        take(1),

        switchMap((action) => {
          if (
            action.type ===
            PlaylistManagementApiAction.failedToCreatePlaylist.type
          ) {
            throw action.error;
          }

          return this.actions.addVideoToPlaylist(
            this.videoId,
            action.simplePlaylistInfo.id
          );
        })
      )
      .subscribe({
        next: () => {
          this.isCreatingPlaylist = false;
          this.close();
        },
        error: () => {
          this.isCreatingPlaylist = false;
        },
      });
  }
}

interface PlaylistToAddVideo {
  id: string;
  title: string;
  visibility: PlaylistVisibility;
  isAdded: boolean;
}

export interface AddToPlaylistDialogData {
  videoId: string;
  itemId?: string;
  playlists: PlaylistsWithVideo;
}
