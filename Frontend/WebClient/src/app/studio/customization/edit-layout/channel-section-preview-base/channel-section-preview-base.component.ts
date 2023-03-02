import {Component, EventEmitter, OnInit, Output} from '@angular/core';

@Component({
  selector: 'app-channel-section-preview-base',
  templateUrl: './channel-section-preview-base.component.html',
  styleUrls: ['./channel-section-preview-base.component.css'],
})
export class ChannelSectionPreviewBaseComponent implements OnInit {
  @Output() onRemoved = new EventEmitter();

  focusing = false;

  constructor() {}

  ngOnInit(): void {}

  openOptionsMenu() {
    this.focusing = true;
    return false;
  }

  optionsMenuClosed() {
    this.focusing = false;
  }

  removeSection() {
    this.onRemoved.emit();
  }
}
