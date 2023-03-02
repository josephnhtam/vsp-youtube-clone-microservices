import {Component, Inject, OnInit, ViewChild, ViewContainerRef,} from '@angular/core';
import {MAT_DIALOG_DATA} from '@angular/material/dialog';
import {ConfirmDialogData} from './confirm-dialog.service';

@Component({
  selector: 'app-confirm-dialog',
  templateUrl: './confirm-dialog.component.html',
  styleUrls: ['./confirm-dialog.component.css'],
})
export class ConfirmDialogComponent implements OnInit {
  @ViewChild('contentContainer', { static: true, read: ViewContainerRef })
  contentContainer!: ViewContainerRef;

  constructor(@Inject(MAT_DIALOG_DATA) public data: ConfirmDialogData) {}

  ngOnInit(): void {
    if (this.data.contentComponent != null) {
      this.contentContainer.createComponent(this.data.contentComponent);
    }
  }

  get title() {
    return this.data.title;
  }

  get content() {
    return this.data.content;
  }

  get confirmBtnText() {
    return this.data.confirmBtnText;
  }

  get cancelBtnText() {
    return this.data.cancelBtnText;
  }
}
