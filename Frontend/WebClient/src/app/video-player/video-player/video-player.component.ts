import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnDestroy,
  OnInit,
  Output,
  SimpleChanges,
  ViewChild,
  ViewEncapsulation,
} from '@angular/core';
import {VgApiService, VgMediaDirective, VgPlayerComponent,} from '@videogular/ngx-videogular/core';
import {Subscription, timer} from 'rxjs';
import {VideoResource} from './models';

@Component({
  selector: 'app-video-player',
  templateUrl: './video-player.component.html',
  styleUrls: ['./video-player.component.css'],
  encapsulation: ViewEncapsulation.None,
})
export class VideoPlayerComponent implements OnInit, OnChanges, OnDestroy {
  @Input()
  thumbnailUrl?: string;

  @Input()
  videoResources!: VideoResource[];

  @Input()
  showQualitySelector: boolean = true;

  @Input()
  inlineProgressBar: boolean = true;

  @ViewChild('videoPlayer', { static: true })
  player!: VgPlayerComponent;

  @ViewChild(VgMediaDirective, { static: true })
  vgMedia!: VgMediaDirective;

  @Input()
  tryAutoPlay = true;

  @Input()
  autoRefresh = true;

  @Input()
  loop: boolean = false;

  @Output()
  played = new EventEmitter<void>();

  @Output()
  paused = new EventEmitter<void>();

  @Output()
  ended = new EventEmitter<void>();

  selectedHeight!: number;
  api?: VgApiService;
  lastTime?: number;
  autoPlay: boolean = false;
  switchingVideoResource = false;
  isPlaying = false;

  private subscriptions: Subscription[] = [];
  private bufferCheckSub?: Subscription;

  constructor() {}

  ngOnDestroy(): void {
    this.stopBufferCheck();

    for (const sub of this.subscriptions) {
      sub.unsubscribe();
    }

    this.subscriptions = [];
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.vgMedia.vgMedia) {
      this.vgMedia.vgMedia.loop = this.loop;
    }

    if (changes['videoResources'] != null) {
      if (this.autoRefresh) {
        this.refresh();
      }
    }
  }

  ngOnInit(): void {
    this.selectDefaultHeight();
  }

  refresh(): void {
    this.selectDefaultHeight();

    if (this.api?.isPlayerReady) {
      this.selectVideoResource(this.selectedHeight, this.tryAutoPlay, false);
    }
  }

  private selectDefaultHeight() {
    const sortedVideoResources = [...this.videoResources].sort(
      (a, b) => b.height - a.height
    );

    let preferredHeight = 720;

    const preferredHeightString = localStorage.getItem('preferred_height');
    if (preferredHeightString && !Number.isNaN(Number(preferredHeightString))) {
      preferredHeight = Number(preferredHeightString);
    }

    this.selectedHeight =
      sortedVideoResources.find((x) => x.height <= preferredHeight)?.height ??
      this.videoResources[0].height;
  }

  onPlayerReady(api: VgApiService) {
    this.api = this.player.api;
    this.vgMedia.vgMedia.loop = this.loop;

    const preferredVolume = Number(
      localStorage.getItem('preferred_volume') ?? '0.5'
    );

    if (!Number.isNaN(preferredVolume)) {
      this.api.volume = preferredVolume;
    }

    const subscriptions = this.api.getDefaultMedia().subscriptions;
    this.subscriptions.push(
      subscriptions.loadedMetadata.subscribe(this.onLoadedMetadata.bind(this))
    );
    this.subscriptions.push(
      subscriptions.volumeChange.subscribe(this.onVolumedChanged.bind(this))
    );
    this.subscriptions.push(
      subscriptions.canPlay.subscribe(this.onCanPlay.bind(this))
    );
    this.subscriptions.push(
      subscriptions.play.subscribe(this.onPlay.bind(this))
    );
    this.subscriptions.push(
      subscriptions.pause.subscribe(this.onPause.bind(this))
    );
    this.subscriptions.push(
      subscriptions.ended.subscribe(this.onEnded.bind(this))
    );
    this.subscriptions.push(
      subscriptions.seeked.subscribe(this.onSeeked.bind(this))
    );

    this.selectVideoResource(this.selectedHeight, this.tryAutoPlay, false);
  }

  onLoadedMetadata() {
    if (!this.api) return;

    if (this.lastTime) {
      this.api.seekTime(this.lastTime);
    }
  }

  onVolumedChanged() {
    if (this.api) {
      localStorage.setItem('preferred_volume', this.api.volume.toString());
    }
  }

  onSeeked() {
    if (this.bufferCheckSub) {
      // fix the bufferCheck when looping
      if (this.vgMedia.currentTime < this.vgMedia.lastPlayPos) {
        this.vgMedia.currentPlayPos = this.vgMedia.currentTime;
        this.vgMedia.lastPlayPos = this.vgMedia.currentTime;
        this.startBufferCheck();
      }
    }
  }

  startBufferCheck() {
    // videogular failed to stopBufferCheck subscription in some case.
    // override its bufferCheck lifetime management here
    this.vgMedia.stopBufferCheck();
    this.stopBufferCheck();

    this.bufferCheckSub = timer(
      this.vgMedia.checkInterval,
      this.vgMedia.checkInterval
    ).subscribe(() => {
      this.vgMedia.bufferCheck();
    });
  }

  stopBufferCheck() {
    if (this.bufferCheckSub) {
      this.bufferCheckSub?.unsubscribe();
      this.bufferCheckSub = undefined;
    }
  }

  onPlay() {
    this.isPlaying = true;
    this.startBufferCheck();
    this.played.next();
  }

  onPause() {
    this.isPlaying = false;
    this.stopBufferCheck();
    this.paused.next();
  }

  onEnded() {
    this.ended.next();
  }

  onCanPlay() {
    if (!this.api) return;

    if (this.lastTime) {
      this.api.seekTime(this.lastTime);
      this.lastTime = undefined;
    }

    this.switchingVideoResource = false;
  }

  onHeightSelected(height: number) {
    this.selectVideoResource(
      height,
      !this.vgMedia.vgMedia.paused && !this.vgMedia.vgMedia.ended,
      true
    );
    localStorage.setItem('preferred_height', height.toString());
  }

  selectVideoResource(
    height: number,
    autoPlay: boolean,
    rememberCurrentTime: boolean
  ) {
    if (!this.api) return;

    this.selectedHeight = height;

    let url = this.videoResources.find(
      (x) => x.height == this.selectedHeight
    )!.url;

    if (rememberCurrentTime) {
      this.lastTime = this.api.currentTime;
      url += `#t=${this.api.currentTime}`;
    }

    this.autoPlay = autoPlay;
    this.vgMedia.vgMedia.autoplay = autoPlay;
    this.vgMedia.vgMedia.src = url;

    this.switchingVideoResource = true;
  }
}
