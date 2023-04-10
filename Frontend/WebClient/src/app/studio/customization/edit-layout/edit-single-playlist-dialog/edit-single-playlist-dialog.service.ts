import { EditSinglePlaylistDialogComponent } from './edit-single-playlist-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { Injectable } from '@angular/core';
import { SinglePlaylistSection } from 'src/app/core/models/channel';

@Injectable()
export class EditSinglePlaylistDialogService {
  constructor(private dialog: MatDialog) {}

  openDialog(
    singlePlaylistSection: SinglePlaylistSection | null,
    onCompleteEdit: OnCompleteEdit
  ) {
    let sectionClone: SinglePlaylistSection | null = null;
    if (singlePlaylistSection != null) {
      sectionClone = JSON.parse(JSON.stringify(singlePlaylistSection));
    }

    const dialogRef = this.dialog.open(EditSinglePlaylistDialogComponent, {
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
  (SinglePlaylistSection: SinglePlaylistSection): void;
}
