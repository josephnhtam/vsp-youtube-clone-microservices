import { AddToPlaylistActionService } from './add-to-playlist-actions.service';
import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import {
  AddToPlaylistDialogComponent,
  AddToPlaylistDialogData,
} from './add-to-playlist-dialog.component';

@Injectable()
export class AddToPlaylistDialogService {
  private _pending = false;

  get pending() {
    return this._pending;
  }

  constructor(
    private dialog: MatDialog,
    private actions: AddToPlaylistActionService
  ) {}

  openAddToPlaylistDialog(videoId: string, itemId?: string) {
    if (this.pending) return;

    this._pending = true;
    this.actions.getPlaylistsWithVideo(videoId).subscribe({
      next: (playlists) => {
        const data: AddToPlaylistDialogData = {
          videoId,
          playlists,
          itemId,
        };

        this.dialog.open(AddToPlaylistDialogComponent, {
          data,
          minWidth: '300px',
          maxWidth: '50dvw',
          maxHeight: '80dvh',
          autoFocus: false,
          restoreFocus: false,
          panelClass: 'add-to-playlist-dialog',
        });
      },

      error: (error) => {
        console.error(error);
        this._pending = false;
      },

      complete: () => {
        this._pending = false;
      },
    });
  }
}
