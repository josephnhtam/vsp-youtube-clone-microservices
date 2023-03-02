import {CdkDragDrop, moveItemInArray} from '@angular/cdk/drag-drop';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ChannelSection} from 'src/app/core/models/channel';

@Component({
  selector: 'app-channel-section-previews-list',
  templateUrl: './channel-section-previews-list.component.html',
  styleUrls: ['./channel-section-previews-list.component.css'],
})
export class ChannelSectionPreviewsListComponent implements OnInit {
  @Input() sections!: ChannelSection[];
  @Output() sectionsChange = new EventEmitter<ChannelSection[]>();

  constructor() {}

  ngOnInit(): void {}

  drop(event: CdkDragDrop<ChannelSection[]>) {
    moveItemInArray(this.sections, event.previousIndex, event.currentIndex);
    this.sectionsChange.emit(this.sections);
  }

  removeSection(section: ChannelSection) {
    this.sections = this.sections.filter((x) => x != section);
    this.sectionsChange.emit(this.sections);
  }
}
