import {catchError, concatMap, EMPTY, finalize, take, tap} from 'rxjs';
import {AuthService} from '../../../../auth/services/auth.service';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ChannelHelperService} from 'src/app/core/services/channel-helper.service';
import {GetVideosResponse, VideosSection,} from 'src/app/core/models/channel';

@Component({
  selector: 'app-channel-videos-section-preview',
  templateUrl: './channel-videos-section-preview.component.html',
  styleUrls: ['./channel-videos-section-preview.component.css'],
})
export class ChannelVideosSectionPreviewComponent implements OnInit {
  @Input() section!: VideosSection;
  @Input() maxPreviewCount: number = 5;
  @Output() onRemoved = new EventEmitter();

  pending: boolean = false;
  getVideoResponse: GetVideosResponse | null = null;

  constructor(
    private service: ChannelHelperService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.pending = true;

    this.authService.authInfo$
      .pipe(
        take(1),
        concatMap((authInfo) => {
          if (authInfo == null) {
            return EMPTY;
          }

          return this.service
            .getVideos(authInfo.sub, 1, this.maxPreviewCount)
            .pipe(
              finalize(() => {
                this.pending = false;
              }),

              tap((response) => {
                this.getVideoResponse = response;
              }),

              catchError((error) => {
                console.error(error);

                this.getVideoResponse = {
                  totalCount: 0,
                  videos: [],
                };

                throw error;
              })
            );
        })
      )
      .subscribe();
  }
}
