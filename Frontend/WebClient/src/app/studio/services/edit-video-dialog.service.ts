import { EditVideoDialogComponent } from '../edit-video-dialog/edit-video-dialog.component';
import { Injectable } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';

@Injectable()
export class EditVideoDialogService {
  static instances: MatDialogRef<EditVideoDialogComponent>[] = [];

  constructor(private dialog: MatDialog) {}

  openDialog(): void;
  openDialog(videoId: string): void;
  openDialog(videoId?: string): void {
    const dialog = this.dialog.open(EditVideoDialogComponent, {
      width: '90dvw',
      height: '90dvh',
      maxWidth: '1200px',
      maxHeight: '720px',
      disableClose: true,
      autoFocus: false,
      restoreFocus: false,
      panelClass: 'edit-video-dialog',
    });

    if (videoId) {
      dialog.componentInstance.subscribeToVideoClient(videoId);
    }

    EditVideoDialogService.instances.push(dialog);

    dialog.afterClosed().subscribe(() => {
      EditVideoDialogService.instances =
        EditVideoDialogService.instances.filter((x) => x != dialog);
    });
  }
}
