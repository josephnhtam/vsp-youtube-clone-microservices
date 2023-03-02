import {DetailedVideoStatus, GetDetailedVideoStatus, VideoVisibility,} from './../../models/index';
import {Actions, concatLatestFrom, ofType} from '@ngrx/effects';
import {selectUploadProcess} from '../../uploader/selectors';
import {Store} from '@ngrx/store';
import {Component, Input, OnDestroy, OnInit, ViewChild, ViewEncapsulation,} from '@angular/core';
import {filter, map, Observable, of, Subscription, switchMap, take,} from 'rxjs';
import {VideoProcessingStatus, VideoThumbnailStatus} from '../../models';
import {VideoClient} from '../../videos-management/models';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {EditVideoDialogComponent} from '../edit-video-dialog.component';
import {VideoInfoUpdateRequest} from '../../videos-management/actions/videos-management';
import {VideosManagementAction, VideosManagementApiAction,} from '../../videos-management/actions';
import {MatStepper} from '@angular/material/stepper';
import {UploadStatus} from '../../uploader/models';

@Component({
  selector: 'app-edit-video',
  templateUrl: './edit-video.component.html',
  styleUrls: ['./edit-video.component.css'],
  encapsulation: ViewEncapsulation.None,
})
export class EditVideoComponent implements OnInit, OnDestroy {
  @Input('videoClient')
  videoClient$!: Observable<VideoClient | undefined>;

  @ViewChild('stepper', { static: true })
  stepper!: MatStepper;

  infoFormGroup!: FormGroup;
  visibilityFormGroup!: FormGroup;

  private videoClientSub?: Subscription;

  constructor(
    private store: Store,
    private actions$: Actions,
    private dialog: EditVideoDialogComponent
  ) {}

  ngOnDestroy(): void {
    this.videoClientSub?.unsubscribe();
  }

  ngOnInit(): void {
    this.subscribeToVideoClient();
    this.createFormGroups();
  }

  get videoProcessingStatus$() {
    return this.videoClient$.pipe(
      map(
        (vc) =>
          vc?.video?.processingStatus ??
          VideoProcessingStatus.WaitingForUserUpload
      )
    );
  }

  get thumbnailsProcessingStatus$() {
    return this.videoClient$.pipe(
      map((vc) => vc?.video?.thumbnailStatus ?? VideoThumbnailStatus.Waiting)
    );
  }

  get isVideoBeingUploaded$() {
    return this.videoProcessingStatus$.pipe(
      map((status) => status == VideoProcessingStatus.VideoBeingProcessed)
    );
  }

  get isVideoUploaded$() {
    return this.videoProcessingStatus$.pipe(
      map(
        (status) =>
          status != VideoProcessingStatus.WaitingForUserUpload &&
          status != VideoProcessingStatus.VideoProcessingFailed
      )
    );
  }

  get isVideoSdBeingProcessed$() {
    return this.videoProcessingStatus$.pipe(
      concatLatestFrom(() => this.thumbnailsProcessingStatus$),

      map(
        ([videoStatus, thumbnailStatus]) =>
          videoStatus > VideoProcessingStatus.WaitingForUserUpload &&
          videoStatus < VideoProcessingStatus.VideoProcessed &&
          thumbnailStatus == VideoThumbnailStatus.Processed
      )
    );
  }

  get isVideoSdProcessed$() {
    return this.videoClient$.pipe(map((vc) => vc?.video?.videos.length != 0));
  }

  get statusText$(): Observable<string> {
    return this.videoClient$.pipe(
      switchMap((videoClient) => {
        if (!videoClient) {
          return of('');
        }

        const detailedStatus = GetDetailedVideoStatus(videoClient?.video);

        switch (detailedStatus) {
          case DetailedVideoStatus.WaitingForUserUpload:
            return this.getUploadProgressStatusText$(videoClient);

          case DetailedVideoStatus.VideoUploaded:
            return of('Upload complete ... Processing will begin shortly.');

          case DetailedVideoStatus.ProcessingThumbnail:
            return of('Processing thumbnails.');

          case DetailedVideoStatus.ProcessingSD:
            return of('Processing SD.');

          case DetailedVideoStatus.ProcessingHD:
            return of('Processing HD.');

          case DetailedVideoStatus.ProcessingComplete:
            return of('Processing complete.');

          case DetailedVideoStatus.ProcessingFailed:
            return of('Processing failed.');

          case DetailedVideoStatus.RegistrationFailed:
            return of('Processing failed');
        }
      })
    );
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

  private subscribeToVideoClient() {
    this.unsubscribeToVideoClient();

    this.videoClientSub = this.videoClient$.subscribe({
      next: this.onVideoClientChanged.bind(this),
    });
  }

  private unsubscribeToVideoClient() {
    if (this.videoClientSub) {
      this.videoClientSub.unsubscribe();
      this.videoClientSub = undefined;
    }
  }

  private onVideoClientChanged(videoClient: VideoClient | undefined) {
    if (!videoClient) return;
  }

  private createFormGroups() {
    const fb = new FormBuilder();

    this.videoClient$.pipe(take(1)).subscribe((videoClient) => {
      if (!videoClient?.video) {
        this.dialog.close();
        throw new Error('Video not found');
      }

      const title = videoClient.video.title;

      const description = videoClient.video.description;

      const tags = videoClient.video.tags;

      let thumbnailId = videoClient.video.thumbnailId;

      const visibility = Number(videoClient.video.visibility);

      let thumbnailIndex =
        videoClient?.video?.thumbnails.findIndex(
          (x) => x.imageFileId === thumbnailId
        ) ?? 0;

      if (thumbnailIndex < 0) thumbnailIndex = 0;

      this.infoFormGroup = fb.group({
        title: [title, [Validators.required, Validators.maxLength(100)]],
        description: [description, Validators.maxLength(5000)],
        tags: [tags, [Validators.maxLength(500)]],
        thumbnailIndex: [thumbnailIndex],
      });

      this.visibilityFormGroup = fb.group({
        visibility: [visibility, [Validators.required]],
      });
    });
  }

  needSave(): boolean {
    let videoClient: VideoClient | undefined;
    this.videoClient$.pipe(take(1)).subscribe((vc) => {
      videoClient = vc;
    });
    if (!videoClient?.video) return false;

    if (!this.infoFormGroup.valid) {
      return false;
    }

    if (this.infoFormGroup.dirty) {
      return true;
    }

    if (!this.visibilityFormGroup.valid) {
      return false;
    }

    if (this.visibilityFormGroup.dirty) {
      return true;
    }

    return false;
  }

  save() {
    if (!this.infoFormGroup.valid || !this.visibilityFormGroup.valid) {
      return;
    }

    this.save$()
      .pipe(take(1))
      .subscribe((success) => {
        if (success) {
          this.dialog.closeWithoutSave();
        }
      });
  }

  save$(): Observable<boolean> {
    let videoClient: VideoClient | undefined;
    this.videoClient$.pipe(take(1)).subscribe((vc) => {
      videoClient = vc;
    });

    if (!videoClient?.video) return of(false);

    let infoUpdateRequest: VideoInfoUpdateRequest = {
      videoId: videoClient.video.id,
    };

    if (this.infoFormGroup.valid) {
      infoUpdateRequest.setBasicInfo = {
        title: this.infoFormGroup.get('title')?.value,
        description: this.infoFormGroup.get('description')?.value,
        tags: this.infoFormGroup.get('tags')?.value,
      };

      if (videoClient.video.thumbnails.length > 0) {
        const thumbnailIndex =
          this.infoFormGroup.get('thumbnailIndex')?.value ?? 0;

        infoUpdateRequest.setBasicInfo.thumbnailId =
          videoClient.video.thumbnails[thumbnailIndex].imageFileId;
      }
    }

    if (this.visibilityFormGroup.valid) {
      infoUpdateRequest.setVisibilityInfo = {
        visibility:
          (this.visibilityFormGroup.get('visibility')
            ?.value as VideoVisibility) ?? VideoVisibility.Private,
      };
    }

    this.store.dispatch(
      VideosManagementAction.setVideoInfo({ request: infoUpdateRequest })
    );

    return this.actions$.pipe(
      ofType(
        VideosManagementApiAction.videoInfoUpdated,
        VideosManagementApiAction.failedToUpdateVideoInfo
      ),
      filter(({ videoId }) => videoId == videoClient?.video?.id),
      take(1),
      map((action) => {
        return action.type === VideosManagementApiAction.videoInfoUpdated.type;
      })
    );
  }
}
