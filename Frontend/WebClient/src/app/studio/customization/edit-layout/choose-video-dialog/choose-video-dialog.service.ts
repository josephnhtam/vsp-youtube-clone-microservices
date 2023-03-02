import {ChooseVideoDialogComponent} from './choose-video-dialog.component';
import {Injectable} from '@angular/core';
import {MatDialog} from '@angular/material/dialog';
import {Video} from 'src/app/core/models/channel';

@Injectable({
  providedIn: 'root',
})
export class ChooseVideoDialogService {
  constructor(private dialog: MatDialog) {}

  openDialog(onChosen: OnChosen) {
    const dialogRef = this.dialog.open(ChooseVideoDialogComponent, {
      width: '60vw',
      height: '80vh',
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
