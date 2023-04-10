import { ConfirmDialogComponent } from './confirm-dialog.component';
import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';

@Injectable()
export class ConfirmDialogService {
  constructor(private dialog: MatDialog) {}

  openConfirmDialog(
    title: string | null,
    content: string | null,
    contentComponent: any | null = null,
    onConfirm: OnConfirm | null = null,
    onCancel: OnCancel | null = null,
    confirmBtnText: string = 'Confirm',
    cancelBtnText: string = 'Cancel'
  ) {
    const data: ConfirmDialogData = {
      title,
      content,
      contentComponent,
      confirmBtnText,
      cancelBtnText,
    };

    const instance = this.dialog.open(ConfirmDialogComponent, {
      data,
      minWidth: '250px',
      maxWidth: '30dvw',
      maxHeight: '80dvh',
      autoFocus: false,
      restoreFocus: false,
      panelClass: 'confirm-dialog',
    });

    instance.afterClosed().subscribe((result) => {
      console.log(result);
      if (result) {
        if (!!onConfirm) onConfirm();
      } else {
        if (!!onCancel) onCancel();
      }
    });
  }
}

export interface OnCancel {
  (): void;
}

export interface OnConfirm {
  (): void;
}

export interface ConfirmDialogData {
  title: string | null;
  content: string | null;
  contentComponent: any | null;
  confirmBtnText: string;
  cancelBtnText: string;
}
