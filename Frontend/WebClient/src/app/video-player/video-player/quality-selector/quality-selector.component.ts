import {Component, EventEmitter, Input, OnInit, Output, ViewEncapsulation,} from '@angular/core';
import {VideoResource} from '../models';

@Component({
  selector: 'app-quality-selector',
  templateUrl: './quality-selector.component.html',
  styleUrls: ['./quality-selector.component.css'],
  encapsulation: ViewEncapsulation.None,
})
export class QualitySelectorComponent implements OnInit {
  @Input('videoResources')
  _videoResources?: VideoResource[];

  @Input('selectedHeight')
  height?: number;

  @Output()
  heightSelected = new EventEmitter<number>();

  showingPopup = false;

  constructor() {}

  ngOnInit(): void {}

  togglePopup() {
    this.showingPopup = !this.showingPopup;
  }

  selectQuality(height: number) {
    if (this.height != height) {
      this.height = height;
      this.heightSelected.emit(this.height);
    }

    this.showingPopup = false;
  }

  get currentQualityOption() {
    if (!this._videoResources) return undefined;
    return this._videoResources.find((x) => x.height == this.height);
  }

  get videoResources() {
    if (!this._videoResources) return [];

    return [...this._videoResources].sort((a, b) => b.height - a.height);
  }
}
