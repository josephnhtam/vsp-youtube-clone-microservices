import { finalize } from 'rxjs';
import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
  SimpleChanges,
} from '@angular/core';
import { ChannelHelperService } from 'src/app/core/services/channel-helper.service';
import { Video } from 'src/app/core/models/channel';
import { ChooseVideoDialogService } from '../../choose-video-dialog/choose-video-dialog.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-video-spotlight',
  templateUrl: './video-spotlight.component.html',
  styleUrls: ['./video-spotlight.component.css'],
})
export class VideoSpotlightComponent implements OnInit, OnChanges {
  @Input() icon = 'movie';
  @Input() title = '';
  @Input() description = '';

  @Input() videoId: string | null = null;
  @Output() videoIdChange = new EventEmitter<string | null>();

  video: Video | null = null;
  pending: boolean = false;

  constructor(
    private service: ChannelHelperService,
    private dialogService: ChooseVideoDialogService
  ) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (this.videoId != this.video?.id) {
      this.video = null;

      if (!!this.videoId) {
        this.loadVideo();
      }
    }
  }

  ngOnInit(): void {}

  loadVideo() {
    if (!this.videoId) return;
    this.pending = true;

    this.service
      .getVideo(this.videoId)
      .pipe(
        finalize(() => {
          this.pending = false;
        })
      )
      .subscribe((video) => {
        this.video = video;
      });
  }

  get thumbnailUrl() {
    return (
      this.video?.thumbnailUrl || environment.assetSetup.noThumbnailIconUrl
    );
  }

  chooseVideo() {
    this.dialogService.openDialog((video) => {
      this.video = video;
      this.videoId = video.id;
      this.videoIdChange.emit(video.id);
    });
  }

  removeVideo() {
    this.video = null;
    this.videoId = null;
    this.videoIdChange.emit(null);
  }
}
