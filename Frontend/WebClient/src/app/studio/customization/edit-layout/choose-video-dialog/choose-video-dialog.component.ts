import {OnChosen} from './choose-video-dialog.service';
import {Component, OnInit} from '@angular/core';
import {Video} from 'src/app/core/models/channel';
import {DialogRef} from '@angular/cdk/dialog';
import {ChannelHelperService} from 'src/app/core/services/channel-helper.service';
import {AuthService} from 'src/app/auth/services/auth.service';
import {concatMap, filter, take} from 'rxjs';

@Component({
  selector: 'app-choose-video-dialog',
  templateUrl: './choose-video-dialog.component.html',
  styleUrls: ['./choose-video-dialog.component.css'],
})
export class ChooseVideoDialogComponent implements OnInit {
  private onChosen: OnChosen | null = null;

  videos: Video[] = [];

  constructor(
    private dialogRef: DialogRef,
    private service: ChannelHelperService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.authService.authInfo$
      .pipe(
        take(1),
        filter((authInfo) => !!authInfo),
        concatMap((authInfo) => {
          return this.service.getVideos(authInfo!.sub);
        })
      )
      .subscribe((response) => {
        this.videos = response.videos;
      });
  }

  initialize(onChosen: OnChosen) {
    this.onChosen = onChosen;
  }

  close() {
    this.dialogRef.close();
  }

  select(video: Video) {
    if (this.onChosen) {
      this.onChosen(video);
    }

    this.close();
  }
}
