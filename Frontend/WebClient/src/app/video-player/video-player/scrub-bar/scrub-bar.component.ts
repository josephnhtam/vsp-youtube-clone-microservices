import {
  Component,
  ElementRef,
  HostBinding,
  HostListener,
  Input,
  OnDestroy,
  OnInit,
  ViewChild,
  ViewEncapsulation,
} from '@angular/core';
import {Subscription} from 'rxjs';
import {VgApiService, VgControlsHiddenService, VgStates,} from '@videogular/ngx-videogular/core';

@Component({
  selector: 'app-scrub-bar',
  encapsulation: ViewEncapsulation.None,
  templateUrl: './scrub-bar.component.html',
  styleUrls: ['./scrub-bar.component.css'],
})
export class ScrubBarComponent implements OnInit, OnDestroy {
  @HostBinding('class.hide') hideScrubBar = false;

  @Input() vgFor: string = '';
  @Input() vgSlider = true;

  @ViewChild('scrubBar', { static: true })
  scrubBarEl!: ElementRef;

  elem: HTMLElement;
  target: any;
  isSeeking = false;
  wasPlaying = false;

  subscriptions: Subscription[] = [];

  constructor(
    ref: ElementRef,
    public API: VgApiService,
    vgControlsHiddenState: VgControlsHiddenService
  ) {
    this.elem = ref.nativeElement;
    this.subscriptions.push(
      vgControlsHiddenState.isHidden.subscribe((hide) =>
        this.onHideScrubBar(hide)
      )
    );
  }

  ngOnInit() {
    if (this.API.isPlayerReady) {
      this.onPlayerReady();
    } else {
      this.subscriptions.push(
        this.API.playerReadyEvent.subscribe(() => this.onPlayerReady())
      );
    }
  }

  onPlayerReady() {
    this.target = this.API.getMediaById(this.vgFor);
  }

  protected seekStart() {
    if (this.target.canPlay) {
      this.isSeeking = true;
      if (this.target.state === VgStates.VG_PLAYING) {
        this.wasPlaying = true;
      }
      this.target.pause();
    }
  }

  protected seekMove(pos: number) {
    if (this.isSeeking) {
      const offset =
        pos - this.scrubBarEl.nativeElement.getBoundingClientRect().left;
      const percentage = Math.max(
        Math.min((offset * 100) / this.elem.scrollWidth, 99.9),
        0
      );
      this.target.time.current = (percentage * this.target.time.total) / 100;
      this.target.seekTime(percentage, true);
    }
  }

  protected seekEnd(pos: number | false) {
    this.isSeeking = false;
    if (this.target.canPlay) {
      if (pos !== false) {
        const offset =
          pos - this.scrubBarEl.nativeElement.getBoundingClientRect().left;
        const percentage = Math.max(
          Math.min((offset * 100) / this.elem.scrollWidth, 99.9),
          0
        );
        this.target.seekTime(percentage, true);
      }
      if (this.wasPlaying) {
        this.wasPlaying = false;
        this.target.play();
      }
    }
  }

  protected touchEnd() {
    this.isSeeking = false;
    if (this.wasPlaying) {
      this.wasPlaying = false;
      this.target.play();
    }
  }

  protected getTouchOffset(event: any) {
    let offsetLeft = 0;
    let element: any = event.target;
    while (element) {
      offsetLeft += element.offsetLeft;
      element = element.offsetParent;
    }
    return event.touches[0].pageX - offsetLeft;
  }

  @HostListener('mousedown', ['$event'])
  onMouseDownScrubBar($event: any) {
    if (this.target) {
      if (!this.target.isLive) {
        if (!this.vgSlider) {
          this.seekEnd($event.clientX);
        } else {
          this.seekStart();
        }
      }
    }
  }

  @HostListener('document:mousemove', ['$event'])
  onMouseMoveScrubBar($event: any) {
    if (this.target) {
      if (!this.target.isLive && this.vgSlider && this.isSeeking) {
        this.seekMove($event.clientX);
      }
    }
  }

  @HostListener('document:mouseup', ['$event'])
  onMouseUpScrubBar($event: any) {
    if (this.target) {
      if (!this.target.isLive && this.vgSlider && this.isSeeking) {
        this.seekEnd($event.clientX);
      }
    }
  }

  @HostListener('touchstart', ['$event'])
  onTouchStartScrubBar(_$event: any) {
    if (this.target) {
      if (!this.target.isLive) {
        if (!this.vgSlider) {
          this.seekEnd(false);
        } else {
          this.seekStart();
        }
      }
    }
  }

  @HostListener('document:touchmove', ['$event'])
  onTouchMoveScrubBar($event: any) {
    if (this.target) {
      if (!this.target.isLive && this.vgSlider && this.isSeeking) {
        this.seekMove(this.getTouchOffset($event));
      }
    }
  }
  // @ts-ignore
  @HostListener('document:touchcancel', ['$event']) onTouchCancelScrubBar(
    _$event: any
  ) {
    if (this.target) {
      if (!this.target.isLive && this.vgSlider && this.isSeeking) {
        this.touchEnd();
      }
    }
  }
  // @ts-ignore
  @HostListener('document:touchend', ['$event']) onTouchEndScrubBar(
    _$event: any
  ) {
    if (this.target) {
      if (!this.target.isLive && this.vgSlider && this.isSeeking) {
        this.touchEnd();
      }
    }
  }

  @HostListener('keydown', ['$event'])
  arrowAdjustVolume(event: KeyboardEvent) {
    if (this.target) {
      if (event.keyCode === 38 || event.keyCode === 39) {
        event.preventDefault();
        this.target.seekTime((this.target.time.current + 5000) / 1000, false);
      } else if (event.keyCode === 37 || event.keyCode === 40) {
        event.preventDefault();
        this.target.seekTime((this.target.time.current - 5000) / 1000, false);
      }
    }
  }

  getPercentage() {
    return this.target
      ? Math.round((this.target.time.current * 100) / this.target.time.total) +
          '%'
      : '0%';
  }

  onHideScrubBar(hide: boolean) {
    this.hideScrubBar = hide;
  }

  ngOnDestroy() {
    this.subscriptions.forEach((s) => s.unsubscribe());
  }
}
