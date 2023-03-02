import {Component, Inject, OnInit} from '@angular/core';
import {MAT_SNACK_BAR_DATA, MatSnackBarRef,} from '@angular/material/snack-bar';

@Component({
  selector: 'app-error-dialog',
  templateUrl: './error-dialog.component.html',
  styleUrls: ['./error-dialog.component.css'],
})
export class ErrorDialogComponent implements OnInit {
  constructor(
    public snackBarRef: MatSnackBarRef<ErrorDialogComponent>,
    @Inject(MAT_SNACK_BAR_DATA) public message: string
  ) {}

  ngOnInit(): void {}
}
