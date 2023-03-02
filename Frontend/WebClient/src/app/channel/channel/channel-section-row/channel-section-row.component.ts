import { PlaylistInfo, Video } from 'src/app/core/models/channel';
import { SwiperComponent } from 'swiper/angular';
import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  HostListener,
  Input,
  OnInit,
  ViewChild,
  ViewEncapsulation,
} from '@angular/core';

@Component({
  selector: 'app-channel-section-row',
  templateUrl: './channel-section-row.component.html',
  styleUrls: [
    './channel-section-row.component.scss',
    '../../../../../node_modules/swiper/swiper-bundle.min.css',
  ],
  encapsulation: ViewEncapsulation.None,
})
export class ChannelSectionRowComponent implements OnInit, AfterViewInit {
  @Input() items: (Video | PlaylistInfo)[] = [];
  @Input() hideUser: boolean = false;
  @Input() hideUpdateDate: boolean = false;
  @Input() hideVisibility: boolean = false;

  @ViewChild('swiper', { static: true })
  swiperComp!: SwiperComponent;

  slideCount: number = 6;
  isBeginning: boolean = true;
  isEnd: boolean = true;

  constructor(private changeDector: ChangeDetectorRef) {}

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.isBeginning = this.swiperComp.swiperRef.isBeginning;
      this.isEnd = this.swiperComp.swiperRef.isEnd;
    }, 0);
  }

  ngOnInit(): void {
    this.onResize();
  }

  swipePrev() {
    this.swiperComp.swiperRef.slidePrev();
  }

  swipeNext() {
    this.swiperComp.swiperRef.slideNext;
    this.swiperComp.swiperRef.slideNext();
  }

  transitionStart() {
    this.isBeginning = this.swiperComp.swiperRef.isBeginning;
    this.isEnd = this.swiperComp.swiperRef.isEnd;
    this.changeDector.detectChanges();
  }

  @HostListener('window:resize')
  onResize() {
    const width = window.innerWidth;

    if (width > 1400) {
      this.slideCount = 6;
    } else if (width > 1200) {
      this.slideCount = 5;
    } else if (width > 992) {
      this.slideCount = 4;
    } else if (width > 768) {
      this.slideCount = 3;
    } else if (width > 576) {
      this.slideCount = 2;
    } else {
      this.slideCount = 1;
    }

    if (!!this.swiperComp.swiperRef) {
      this.isBeginning = this.swiperComp.swiperRef.isBeginning;
      this.isEnd = this.swiperComp.swiperRef.isEnd;
    }
  }

  isPlaylist(item: Video | PlaylistInfo): item is PlaylistInfo {
    return 'itemsCount' in item;
  }
}
