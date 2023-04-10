import { UploaderDialogComponent } from './uploader-dialog.component';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { Injectable } from '@angular/core';

@Injectable()
export class UploaderDialogService {
  private instance: MatDialogRef<UploaderDialogComponent> | null = null;

  constructor(private dialog: MatDialog) {}

  openDialog() {
    if (!!this.instance) return;

    const dialog = this.dialog.open(UploaderDialogComponent, {
      width: '384px',
      maxWidth: '50dvw',
      minHeight: '0',
      disableClose: true,
      hasBackdrop: false,
      autoFocus: false,
      restoreFocus: false,
      closeOnNavigation: false,
      panelClass: 'uploader-dialog',
      position: { bottom: '48px', right: '48px' },
    });

    this.instance = dialog;

    dialog.afterClosed().subscribe(() => {
      if (this.instance == dialog) {
        this.instance = null;
      }
    });
  }
}
