import { ChooseVideoDialogComponent } from './choose-video-dialog.component';
import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Video } from 'src/app/core/models/channel';

@Injectable({
  providedIn: 'root',
})
export class ChooseVideoDialogService {
  constructor(private dialog: MatDialog) {}

  openDialog(onChosen: OnChosen) {
    const dialogRef = this.dialog.open(ChooseVideoDialogComponent, {
      width: '90dvw',
      height: '90dvh',
      maxWidth: '1200px',
      maxHeight: '720px',
      disableClose: true,
      autoFocus: false,
      restoreFocus: false,
      panelClass: 'choose-video-dialog',
    });

    dialogRef.componentInstance.initialize(onChosen);
  }
}

export interface OnChosen {
  (video: Video): void;
}
