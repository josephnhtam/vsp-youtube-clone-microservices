import {selectVideoClientByContextId, selectVideoClientById} from '../videos-management/selectors';
import {environment} from 'src/environments/environment';
import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {MatDialogRef} from '@angular/material/dialog';
import {Store} from '@ngrx/store';
import * as VideosManagement from '../videos-management/actions';
import {VideosManagementAction} from '../videos-management/actions';
import {VideoClient} from '../videos-management/models';
import {Video, VideoProcessingStatus} from '../models';
import {StudioHubClientService} from '../services/studio-hub-client.service';
import {HubClientState} from 'src/app/core/signalr/hub-client';
import {EMPTY, finalize, interval, map, Observable, of, Subscription, switchMap, take, tap,} from 'rxjs';
import {EditVideoComponent} from './edit-video/edit-video.component';

@Component({
  selector: 'app-edit-video-dialog',
  templateUrl: './edit-video-dialog.component.html',
  styleUrls: ['./edit-video-dialog.component.css'],
})
export class EditVideoDialogComponent implements OnInit, OnDestroy {
  @ViewChild(EditVideoComponent)
  editVideoComp?: EditVideoComponent;

  file?: File;
  videoClient$?: Observable<VideoClient | undefined>;

  private videoClientSub?: Subscription;
  private clientStateChangeSub?: Subscription;
  private refresher?: Subscription;
  private static contextId = 0;

  constructor(
    private store: Store,
    private studioHubClient: StudioHubClientService,
    public dialogRef?: MatDialogRef<EditVideoDialogComponent>
  ) {}

  ngOnInit(): void {
    this.createHubClientFallback();
  }

  private createHubClientFallback() {
    this.clientStateChangeSub = this.studioHubClient.state$
      .pipe(
        finalize(() => {
          this.stopVideoRefresher();
        }),

        tap((state) => {
          if (state == HubClientState.Connected) {
            this.stopVideoRefresher();
          } else {
            this.startVideoRefresher();
          }
        })
      )
      .subscribe();
  }

  private startVideoRefresher() {
    if (this.refresher) return;

    this.refresher = interval(
      environment.studioSetup.videoRefreshIntervalSeconds * 1000
    )
      .pipe(
        switchMap(() => {
          if (this.videoClient$) {
            return this.videoClient$.pipe(take(1));
          } else {
            return EMPTY;
          }
        }),

        tap((videoClient) => {
          if (
            videoClient &&
            videoClient.video &&
            videoClient.video.processingStatus <
              VideoProcessingStatus.VideoProcessed
          ) {
            this.store.dispatch(
              VideosManagementAction.getVideo({
                videoId: videoClient.video.id,
              })
            );
          }
        })
      )
      .subscribe();
  }

  private stopVideoRefresher() {
    if (!this.refresher) return;

    this.refresher.unsubscribe();
    this.refresher = undefined;
  }

  ngOnDestroy(): void {
    this.unsubscribeToVideoClient();
    this.clientStateChangeSub?.unsubscribe();
  }

  close() {
    if (this.editVideoComp && this.editVideoComp.needSave()) {
      this.editVideoComp
        .save$()
        .pipe(take(1))
        .subscribe((success) => {
          if (success) {
            this.closeWithoutSave();
          }
        });
    } else {
      this.closeWithoutSave();
    }
  }

  closeWithoutSave() {
    this.dialogRef?.close();
  }

  uploadFile(files: File[]) {
    if (files.length == 0) return;

    if (files.length == 1) {
      this.uploadSingleFile(files[0]);
    } else {
      for (const file of files) {
        this.createVideoAndUploadFile(file);
      }
      this.close();
    }
  }

  uploadSingleFile(file: File) {
    this.file = file;

    if (this.videoClient$) {
      this.videoClient$.pipe(take(1)).subscribe((vc) => {
        if (vc?.video) {
          this.getVideoUploadTokenAndUploadFile(vc.video, file);
        } else {
          this.createVideoAndUploadFile(file);
        }
      });
    } else {
      this.createVideoAndUploadFile(file);
    }
  }

  private getVideoUploadTokenAndUploadFile(video: Video, file: File) {
    this.store.dispatch(
      VideosManagement.VideosManagementAction.getVideoUploadToken({
        videoId: video.id,
        videoFile: file,
      })
    );
  }

  private createVideoAndUploadFile(file: File) {
    let title = this.createTitle(file);

    const contextId = ++EditVideoDialogComponent.contextId;

    this.subscribeToVideoClientByContextId(contextId);

    this.store.dispatch(
      VideosManagement.VideosManagementAction.createVideo({
        title,
        description: '',
        videoFile: file,
        contextId: contextId,
      })
    );
  }

  private createTitle(file: File) {
    let title = file.name;
    const dotPos = title?.lastIndexOf('.');
    if (dotPos && dotPos > 0) {
      title = title?.substring(0, dotPos);
    }
    title = title?.trim();
    return title;
  }

  subscribeToVideoClient(videoId: string) {
    this.unsubscribeToVideoClient();

    this.videoClient$ = this.store.select(selectVideoClientById(videoId));

    this.videoClientSub = this.videoClient$.subscribe({
      next: this.onVideoClientChanged.bind(this),
    });
  }

  private subscribeToVideoClientByContextId(contextId: number) {
    this.unsubscribeToVideoClient();

    this.videoClient$ = this.store.select(
      selectVideoClientByContextId(contextId)
    );

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

    if (videoClient.processError != null) {
      // todo: show error message
      this.close();
    }
  }

  get dialogTitle$() {
    const defaultTitle = 'Upload videos';

    if (this.videoClient$) {
      return this.videoClient$.pipe(
        map((vc) => {
          if (vc?.video) {
            return vc.video.title;
          } else {
            return defaultTitle;
          }
        })
      );
    } else {
      return of(defaultTitle);
    }
  }

  get readyToEdit$() {
    if (!this.videoClient$) return of(false);

    return this.videoClient$.pipe(
      map((videoClient) => {
        if (!videoClient?.video) {
          return false;
        }

        return (
          videoClient.videoUploadToken != null ||
          videoClient.video.processingStatus !==
            VideoProcessingStatus.WaitingForUserUpload
        );
      })
    );
  }
}
