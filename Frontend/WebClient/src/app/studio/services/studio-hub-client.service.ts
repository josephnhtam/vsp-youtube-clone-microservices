import { Store } from '@ngrx/store';
import * as SignalR from '@microsoft/signalr';
import {
  BehaviorSubject,
  from,
  lastValueFrom,
  map,
  of,
  switchMap,
  take,
} from 'rxjs';
import { AuthService } from '../../auth/services/auth.service';
import { environment } from 'src/environments/environment';
import { Injectable, OnDestroy } from '@angular/core';
import {
  HubClient,
  HubClientOptions,
  HubClientState,
} from '../../core/signalr/hub-client';
import { VideosManagementHubAction } from '../videos-management/actions';
import { ProcessedVideo, Video, VideoThumbnail } from '../models';

@Injectable()
export class StudioHubClientService implements OnDestroy {
  private hubClientSubject = new BehaviorSubject<HubClient | null>(null);

  constructor(private authService: AuthService, private store: Store) {}

  ngOnDestroy(): void {
    if (this.hubClientSubject.getValue() != null) {
      this.stop();
    }
  }

  get isStarted$() {
    return this.hubClientSubject.asObservable();
  }

  get state$() {
    return this.hubClientSubject.pipe(
      switchMap((client) => {
        if (client != null) {
          return client.state$;
        } else {
          return of(HubClientState.Disconnected);
        }
      })
    );
  }

  start() {
    return from(this.doStart());
  }

  stop() {
    return from(this.doStop());
  }

  private async doStart() {
    let hubClient = this.hubClientSubject.getValue();

    if (hubClient != null) {
      throw new Error('Already started');
    }

    const options: HubClientOptions = {
      hubUrl: environment.appSetup.apiUrl + '/Hubs/VideoManager',
      httpConnectionOptions: this.createHttpConnectionOptions(),
      configureBuilder: this.configureBuilder.bind(this),
    };

    hubClient = new HubClient(options);
    this.registerEventHandlers(hubClient);
    this.hubClientSubject.next(hubClient);
    await hubClient.start();
  }

  private async doStop() {
    const hubClient = this.hubClientSubject.getValue();

    if (hubClient == null) {
      throw new Error('Not started');
    }

    this.hubClientSubject.next(null);
    await hubClient.stop();
  }

  protected createHttpConnectionOptions(): SignalR.IHttpConnectionOptions {
    return {
      accessTokenFactory: this.getAccessToken.bind(this),
      transport:
        SignalR.HttpTransportType.WebSockets |
        SignalR.HttpTransportType.ServerSentEvents |
        SignalR.HttpTransportType.LongPolling,
    };
  }

  protected getAccessToken(): Promise<string> {
    return lastValueFrom(
      this.authService.authInfo$.pipe(
        take(1),
        map((authInfo) => {
          return authInfo?.token.access_token ?? '';
        })
      )
    );
  }

  protected configureBuilder(builder: SignalR.HubConnectionBuilder): void {
    builder
      // .withHubProtocol(new MessagePackHubProtocol())
      .configureLogging(SignalR.LogLevel.Trace);
  }

  protected registerEventHandlers(client: HubClient): void {
    client.on('NotifyVideoUploaded', this.onVideoUploaded.bind(this));
    client.on('NotifyVideoRegistered', this.onVideoRegistered.bind(this));
    client.on(
      'NotifyVideoBeingProcessed',
      this.onVideoBeingProcessed.bind(this)
    );
    client.on(
      'NotifyVideoProcesssingFailed',
      this.onVideoProcessingFailed.bind(this)
    );
    client.on(
      'NotifyVideoProcessingComplete',
      this.onVideoProcessingComplete.bind(this)
    );
    client.on(
      'NotifyProcessedVideoAdded',
      this.onProcessedVideoAdded.bind(this)
    );
    client.on(
      'NotifyVideoThumbnailsAdded',
      this.onVideoThumbnailsAdded.bind(this)
    );
  }

  protected onVideoUploaded(
    videoId: string,
    originalFileName: string,
    videoFileUrl: string
  ) {
    this.store.dispatch(
      VideosManagementHubAction.videoUploaded({
        videoId,
        originalFileName,
        videoFileUrl,
      })
    );
  }

  protected onVideoRegistered(videoId: string) {
    console.log(`Video (${videoId}) registered`);
    // this.store.dispatch(
    //   HubActions.videoRegistered{ videoId })
    // );
  }

  protected onVideoBeingProcessed(videoId: string) {
    this.store.dispatch(
      VideosManagementHubAction.videoBeingProcessed({ videoId })
    );
  }

  protected onVideoProcessingFailed(videoId: string) {
    this.store.dispatch(
      VideosManagementHubAction.videoProcessingFailed({ videoId })
    );
  }

  protected onVideoProcessingComplete(video: Video) {
    console.log('onVideoProcessingComplete', video);
    this.store.dispatch(
      VideosManagementHubAction.videoProcessingComplete({
        video,
      })
    );
  }

  protected onProcessedVideoAdded(videoId: string, video: ProcessedVideo) {
    this.store.dispatch(
      VideosManagementHubAction.processedVideoAdded({ videoId, video })
    );
  }

  protected onVideoThumbnailsAdded(
    videoId: string,
    thumbnails: VideoThumbnail[]
  ) {
    this.store.dispatch(
      VideosManagementHubAction.videoThumbnailsAdded({ videoId, thumbnails })
    );
  }
}
