import {concatMap, filter, take} from 'rxjs';
import {AuthService} from 'src/app/auth/services/auth.service';
import {ChannelHelperService} from 'src/app/core/services/channel-helper.service';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {PlaylistInfo, SimplePlaylistInfo,} from 'src/app/core/models/channel';

@Component({
  selector: 'app-playlists-browser',
  templateUrl: './playlists-browser.component.html',
  styleUrls: ['./playlists-browser.component.css'],
})
export class PlaylistsBrowserComponent implements OnInit {
  @Input() playlistsInSection!: SimplePlaylistInfo[];
  @Output() playlistsInSectionChange = new EventEmitter<SimplePlaylistInfo[]>();

  playlistInfos: PlaylistInfo[] | null = null;

  constructor(
    private service: ChannelHelperService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.authService.authInfo$
      .pipe(
        take(1),
        filter((authInfo) => !!authInfo),
        concatMap((authInfo) => {
          return this.service.getCreatedPlaylistInfos(authInfo!.sub);
        })
      )
      .subscribe((response) => {
        this.playlistInfos = response.infos;
      });
  }

  isSelected(playlistId: string) {
    return this.playlistsInSection.some((x) => x.id === playlistId);
  }

  selectionChange(info: PlaylistInfo, isSelected: boolean) {
    if (isSelected) {
      if (!this.playlistsInSection.some((x) => x.id === info.id)) {
        this.playlistsInSection.push({
          id: info.id,
          title: info.title,
          updateDate: info.updateDate,
        });
      }
    } else {
      this.playlistsInSection = this.playlistsInSection.filter(
        (x) => x.id != info.id
      );
    }

    this.playlistsInSectionChange.emit(this.playlistsInSection);
  }
}
