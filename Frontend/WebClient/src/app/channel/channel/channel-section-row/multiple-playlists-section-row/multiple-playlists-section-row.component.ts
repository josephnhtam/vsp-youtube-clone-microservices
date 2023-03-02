import {Component, Input, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {MultiplePlaylistsSectionInstance} from 'src/app/core/models/channel';

@Component({
  selector: 'app-multiple-playlists-section-row',
  templateUrl: './multiple-playlists-section-row.component.html',
  styleUrls: ['./multiple-playlists-section-row.component.css'],
})
export class MultiplePlaylistsSectionRowComponent implements OnInit {
  @Input() section!: MultiplePlaylistsSectionInstance;

  constructor(private router: Router, private route: ActivatedRoute) {}

  ngOnInit(): void {}

  get title() {
    const title = this.section.title.trim();

    if (title == '') {
      return 'Multiple playlists';
    } else {
      return title;
    }
  }
}
