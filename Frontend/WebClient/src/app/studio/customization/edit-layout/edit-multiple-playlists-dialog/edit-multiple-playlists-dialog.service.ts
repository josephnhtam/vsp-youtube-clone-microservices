import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MultiplePlaylistsSection } from 'src/app/core/models/channel';
import { EditMultiplePlaylistsDialogComponent } from './edit-multiple-playlists-dialog.component';

@Injectable({
  providedIn: 'root',
})
export class EditMultiplePlaylistsDialogService {
  constructor(private dialog: MatDialog) {}

  openDialog(
    multiplePlaylistsSection: MultiplePlaylistsSection | null,
    onCompleteEdit: OnCompleteEdit
  ) {
    let sectionClone: MultiplePlaylistsSection | null = null;
    if (multiplePlaylistsSection != null) {
      sectionClone = JSON.parse(JSON.stringify(multiplePlaylistsSection));
    }

    const dialogRef = this.dialog.open(EditMultiplePlaylistsDialogComponent, {
      width: '90dvw',
      height: '90dvh',
      maxWidth: '1200px',
      maxHeight: '720px',
      disableClose: true,
      autoFocus: false,
      restoreFocus: false,
      panelClass: 'edit-single-playlist-dialog',
    });

    dialogRef.componentInstance.initialize(sectionClone, onCompleteEdit);
  }
}

export interface OnCompleteEdit {
  (multiplePlaylistsSection: MultiplePlaylistsSection): void;
}
