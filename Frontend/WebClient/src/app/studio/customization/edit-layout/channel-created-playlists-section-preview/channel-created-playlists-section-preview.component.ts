import {GetPlaylistInfosResponse} from '../../../../core/models/library';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ChannelHelperService} from 'src/app/core/services/channel-helper.service';
import {AuthService} from 'src/app/auth/services/auth.service';
import {catchError, concatMap, filter, finalize, take} from 'rxjs';
import {CreatedPlaylistsSection} from 'src/app/core/models/channel';

@Component({
  selector: 'app-channel-created-playlists-section-preview',
  templateUrl: './channel-created-playlists-section-preview.component.html',
  styleUrls: ['./channel-created-playlists-section-preview.component.css'],
})
export class ChannelCreatedPlaylistsSectionPreviewComponent implements OnInit {
  @Input() section!: CreatedPlaylistsSection;
  @Input() maxPreviewCount: number = 5;
  @Output() onRemoved = new EventEmitter();

  pending: boolean = false;
  getPlaylistInfosResponse: GetPlaylistInfosResponse | null = null;

  constructor(
    private service: ChannelHelperService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.pending = true;

    this.authService.authInfo$
      .pipe(
        take(1),

        filter((authInfo) => !!authInfo),

        concatMap((authInfo) => {
          return this.service
            .getCreatedPlaylistInfos(authInfo!.sub, 1, this.maxPreviewCount)
            .pipe(
              finalize(() => {
                this.pending = false;
              }),

              catchError((error) => {
                console.error(error);

                this.getPlaylistInfosResponse = {
                  totalCount: 0,
                  infos: [],
                };

                throw error;
              })
            );
        })
      )
      .subscribe((response) => {
        this.getPlaylistInfosResponse = response;
      });
  }
}
