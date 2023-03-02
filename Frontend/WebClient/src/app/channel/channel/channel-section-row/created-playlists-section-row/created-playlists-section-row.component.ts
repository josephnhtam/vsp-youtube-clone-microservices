import {Component, Input, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {CreatedPlaylistsSectionInstance} from 'src/app/core/models/channel';

@Component({
  selector: 'app-created-playlists-section-row',
  templateUrl: './created-playlists-section-row.component.html',
  styleUrls: ['./created-playlists-section-row.component.css'],
})
export class CreatedPlaylistsSectionRowComponent implements OnInit {
  @Input() section!: CreatedPlaylistsSectionInstance;

  constructor(private router: Router, private route: ActivatedRoute) {}

  ngOnInit(): void {}
}
