import {ErrorDialogComponent} from './error-dialog.component';
import {Injectable} from '@angular/core';
import {
  MatSnackBar,
  MatSnackBarHorizontalPosition,
  MatSnackBarRef,
  MatSnackBarVerticalPosition,
} from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root',
})
export class ErrorDialogService {
  constructor(private snackBar: MatSnackBar) {}

  private snackbarRef?: MatSnackBarRef<ErrorDialogComponent>;

  openErrorDialog(
    message: string,
    durationInSeconds?: number,
    horizontalPosition?: MatSnackBarHorizontalPosition,
    verticalPosition?: MatSnackBarVerticalPosition
  ) {
    if (this.snackbarRef && this.snackbarRef.instance.message == message) {
      return;
    }

    const newSnackbarRef = this.snackBar.openFromComponent(
      ErrorDialogComponent,
      {
        panelClass: 'error-dialog',
        data: message,
        duration:
          durationInSeconds != undefined ? durationInSeconds * 1000 : undefined,
        horizontalPosition: horizontalPosition || 'center',
        verticalPosition: verticalPosition || 'bottom',
      }
    );

    this.snackbarRef = newSnackbarRef;

    this.snackbarRef.afterDismissed().subscribe(() => {
      if (this.snackbarRef == newSnackbarRef) {
        this.snackbarRef = undefined;
      }
    });
  }
}
