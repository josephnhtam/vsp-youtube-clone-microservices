import { ConfirmDialogService } from './../../shared/confirm-dialog/confirm-dialog.service';
import {
  DetailedVideoStatus,
  GetDetailedVideoStatus,
  VideoProcessingStatus,
} from 'src/app/studio/models';
import { environment } from 'src/environments/environment';
import { Router } from '@angular/router';
import { VideoStatus, VideoVisibility } from './../models/index';
import { Actions, ofType } from '@ngrx/effects';
import { getVideos } from './actions/videos-management';
import { Store } from '@ngrx/store';
import {
  AfterViewInit,
  Component,
  OnDestroy,
  OnInit,
  ViewChild,
} from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import {
  map,
  merge,
  Observable,
  of,
  pipe,
  startWith,
  Subscription,
  tap,
} from 'rxjs';
import {
  selectIsFetchingVideos,
  selectLastFetchVideoClients,
  selectTotalVideosCount,
} from './selectors';
import { VideoClient } from './models';
import { EditVideoDialogService } from '../services/edit-video-dialog.service';
import { getVideoThumbnail, Video } from '../models';
import { VideosManagementAction, VideosManagementApiAction } from './actions';
import {
  selectUploadProcess,
  selectUploadProcesses,
} from '../uploader/selectors';
import { UploaderDialogService } from '../uploader/uploader-dialog/uploader-dialog.service';
import { UploadStatus } from '../uploader/models';
import { AddToPlaylistDialogService } from 'src/app/shared/add-to-playlist-dialog/add-to-playlist-dialog.service';

@Component({
  selector: 'app-videos-management',
  templateUrl: './videos-management.component.html',
  styleUrls: ['./videos-management.component.css'],
})
export class VideosManagementComponent
  implements OnInit, AfterViewInit, OnDestroy
{
  displayedColumns: string[] = [
    'video',
    'visibility',
    'date',
    'views',
    'comments',
    'likes',
  ];

  pageSizeOptions: number[] = [10, 30, 50];

  resultsLength = 0;
  isFetchingVideos$!: Observable<boolean>;
  totalVideosCount$!: Observable<number>;
  videoClients$!: Observable<VideoClient[]>;

  focusedRow: VideoClient | null = null;

  private operationSub?: Subscription;
  private videoCreatedSub?: Subscription;
  private processesSub?: Subscription;

  @ViewChild(MatPaginator, { static: true }) paginator!: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort!: MatSort;

  constructor(
    private store: Store,
    private actions$: Actions,
    private editVideoDialog: EditVideoDialogService,
    private router: Router,
    private uploaderDialog: UploaderDialogService,
    private addToPlaylistDialog: AddToPlaylistDialogService,
    private confirmDialog: ConfirmDialogService
  ) {}

  ngOnDestroy(): void {
    this.operationSub?.unsubscribe();
    this.videoCreatedSub?.unsubscribe();
    this.processesSub?.unsubscribe();
  }

  ngOnInit(): void {
    this.paginator.pageIndex = 0;
    this.paginator.pageSize = this.pageSizeOptions[1];

    this.isFetchingVideos$ = this.store.select(selectIsFetchingVideos);
    this.totalVideosCount$ = this.store.select(selectTotalVideosCount);
    this.videoClients$ = this.store.select(selectLastFetchVideoClients);

    this.createVideoCreatedSubscription();
    this.clearLastFetchVideoClients();
    this.createOperationSubscription();
    this.createProcessesSubscription();
  }

  private createVideoCreatedSubscription() {
    this.videoCreatedSub = this.actions$
      .pipe(
        ofType(VideosManagementApiAction.videoCreated),
        pipe(
          tap(() => {
            this.fetchVideos();
          })
        )
      )
      .subscribe();
  }

  private createProcessesSubscription() {
    this.processesSub = this.store
      .select(selectUploadProcesses)
      .subscribe((processes) => {
        if (
          EditVideoDialogService.instances.length == 0 &&
          processes.some((x) => x.status == UploadStatus.InProgress)
        ) {
          this.uploaderDialog.openDialog();
        }
      });
  }

  private createOperationSubscription() {
    this.operationSub = merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        startWith({}),
        tap(() => {
          this.fetchVideos();
        })
      )
      .subscribe();
  }

  private clearLastFetchVideoClients() {
    this.store.dispatch(VideosManagementAction.clearLastFetchVideoClients());
  }

  private fetchVideos() {
    this.store.dispatch(
      getVideos({
        page: this.paginator.pageIndex + 1,
        pageSize: this.paginator.pageSize,
        sort: `${this.sort.active}-${this.sort.direction}`,
      })
    );
  }

  ngAfterViewInit(): void {}

  editVideo(videoClient: VideoClient) {
    if (videoClient.video?.id) {
      this.editVideoDialog.openDialog(videoClient.video?.id);
    } else {
      this.editVideoDialog.openDialog();
    }
    return false;
  }

  viewVideo(videoClient: VideoClient) {
    if (videoClient.video?.id) {
      const url = this.router.createUrlTree(['/watch', videoClient.video.id]);
      window.open(url.toString());
    }
  }

  getViewVideoLink(videoClient: VideoClient) {
    if (videoClient.video?.id) {
      return ['/watch', videoClient.video.id];
    } else {
      return '';
    }
  }

  getVideoThumbnailUrl(video: Video) {
    return (
      getVideoThumbnail(video)?.url ?? environment.assetSetup.noThumbnailIconUrl
    );
  }

  getVideoLength(video: Video) {
    if (video.videos.length > 0) {
      const lengthSeconds = video.videos[0].lengthSeconds ?? 0;

      if (lengthSeconds > 3600) {
        return new Date(lengthSeconds * 1000).toISOString().slice(11, 19);
      } else {
        return new Date(lengthSeconds * 1000).toISOString().slice(14, 19);
      }
    }

    return null;
  }

  getVisibilityText(video: Video) {
    switch (video.visibility) {
      default:
      case VideoVisibility.Private:
        return 'Private';
      case VideoVisibility.Unlisted:
        return 'Unlisted';
      case VideoVisibility.Public:
        return 'Public';
    }
  }

  getVisibilityIcon(video: Video) {
    switch (video.visibility) {
      default:
      case VideoVisibility.Private:
        return 'visibility_off';
      case VideoVisibility.Unlisted:
        return 'visibility';
      case VideoVisibility.Public:
        return 'visibility';
    }
  }

  unregisterVideo(videoClient: VideoClient) {
    const video = videoClient.video;
    if (!video) return;

    const doUnregisterVideo = () => {
      this.store.dispatch(
        VideosManagementAction.unregisterVideo({ videoId: video.id })
      );
    };

    if (
      video.processingStatus == VideoProcessingStatus.VideoProcessingFailed ||
      video.processingStatus == VideoProcessingStatus.WaitingForUserUpload
    ) {
      doUnregisterVideo();
    } else {
      this.confirmDialog.openConfirmDialog(
        'Delete video',
        `<div>Are you sure you want to delete <strong>${video.title}</strong></div>
       <div class="note">Note: Deleting videos is a permanent action and cannot be undone.</div>`,
        null,
        doUnregisterVideo,
        null,
        'Delete',
        'Cancel'
      );
    }
  }

  openOptionsMenu(videoClient: VideoClient) {
    this.focusedRow = videoClient;
    return false;
  }

  optionsMenuClosed() {
    this.focusedRow = null;
  }

  canUnregisterVideo(videoClient: VideoClient) {
    const video = videoClient.video;
    if (!video) return false;

    if (video.status != VideoStatus.RegistrationFailed) {
      if (
        video.processingStatus > VideoProcessingStatus.WaitingForUserUpload &&
        video.processingStatus < VideoProcessingStatus.VideoProcessed
      ) {
        return false;
      }
    }

    return true;
  }

  getStatus$(videoClient: VideoClient) {
    const detailedStatus = GetDetailedVideoStatus(videoClient?.video);

    switch (detailedStatus) {
      case DetailedVideoStatus.WaitingForUserUpload:
        return this.getUploadProgressStatusText$(videoClient);

      case DetailedVideoStatus.VideoUploaded:
        return of('Processing will begin shortly');

      case DetailedVideoStatus.ProcessingThumbnail:
        return of('Processing thumbnails');

      case DetailedVideoStatus.ProcessingSD:
        return of('Processing SD');

      case DetailedVideoStatus.ProcessingHD:
        return of('Processing HD');

      case DetailedVideoStatus.ProcessingComplete:
        return null;

      case DetailedVideoStatus.ProcessingFailed:
        return of('Processing failed');

      case DetailedVideoStatus.RegistrationFailed:
        return of('Processing failed');
    }
  }

  getStatusIcon(videoClient: VideoClient) {
    const detailedStatus = GetDetailedVideoStatus(videoClient?.video);

    switch (detailedStatus) {
      case DetailedVideoStatus.WaitingForUserUpload:
        return 'upgrade';

      case DetailedVideoStatus.VideoUploaded:
        return 'upgrade';

      case DetailedVideoStatus.ProcessingThumbnail:
        return 'upgrade';

      case DetailedVideoStatus.ProcessingSD:
        return 'sd';

      case DetailedVideoStatus.ProcessingHD:
        return 'hd';

      case DetailedVideoStatus.ProcessingComplete:
        return null;

      case DetailedVideoStatus.ProcessingFailed:
        return 'error';

      case DetailedVideoStatus.RegistrationFailed:
        return 'error';
    }
  }

  getStatusClass(videoClient: VideoClient) {
    const detailedStatus = GetDetailedVideoStatus(videoClient?.video);

    switch (detailedStatus) {
      case DetailedVideoStatus.WaitingForUserUpload:
        return 'waiting';

      case DetailedVideoStatus.VideoUploaded:
        return 'normal';

      case DetailedVideoStatus.ProcessingThumbnail:
        return 'normal';

      case DetailedVideoStatus.ProcessingSD:
        return 'normal';

      case DetailedVideoStatus.ProcessingHD:
        return 'normal';

      case DetailedVideoStatus.ProcessingComplete:
        return '';

      case DetailedVideoStatus.ProcessingFailed:
        return 'error';

      case DetailedVideoStatus.RegistrationFailed:
        return 'error';
    }
  }

  getUploadProgressStatusText$(videoClient: VideoClient): Observable<string> {
    if (videoClient.videoUploadToken == null) {
      return of('Waiting for user upload');
    } else {
      return this.store
        .select(selectUploadProcess(videoClient.videoUploadToken))
        .pipe(
          map((uploadProcess) => {
            if (!uploadProcess) {
              return 'Uploading ...';
            }

            if (uploadProcess.status == UploadStatus.Failed) {
              return 'Uploading failed';
            }

            if (uploadProcess.status == UploadStatus.Cancelled) {
              return 'Uploading cancelled';
            }

            return `Uploading ${uploadProcess.progress}% ...`;
          })
        );
    }
  }

  canViewVideo(videoClient: VideoClient) {
    return (
      videoClient.video?.status == VideoStatus.Registered &&
      videoClient.video?.processingStatus !=
        VideoProcessingStatus.VideoProcessingFailed
    );
  }

  getLikesPercentage(VideoClient: VideoClient) {
    return (
      (100 * VideoClient.video!.metrics.likesCount) /
      (VideoClient.video!.metrics.likesCount +
        VideoClient.video!.metrics.dislikesCount)
    );
  }

  addToPlaylist(VideoClient: VideoClient) {
    if (VideoClient.video) {
      this.addToPlaylistDialog.openAddToPlaylistDialog(VideoClient.video.id);
    }
  }
}
