import {ChannelPlaylistsComponent} from './channel-playlists/channel-playlists.component';
import {ChannelVideosComponent} from './channel-videos/channel-videos.component';
import {ChannelFeaturedComponent} from './channel-featured/channel-featured.component';
import {filter, map, Observable, tap} from 'rxjs';
import {ActivatedRoute, NavigationEnd, Router} from '@angular/router';
import {Component, ElementRef, OnInit, Type, ViewChild,} from '@angular/core';
import {ChannelData} from 'src/app/core/models/channel';
import {ChannelAboutComponent} from './channel-about/channel-about.component';

@Component({
  selector: 'app-channel',
  templateUrl: './channel.component.html',
  styleUrls: ['./channel.component.css'],
})
export class ChannelComponent implements OnInit {
  channel$!: Observable<ChannelData>;
  navs!: ChannelNavigation[];
  activeLink?: string;

  bannerOffsetStyle: string = '';

  @ViewChild('container', { static: true }) containerEl!: ElementRef;

  @ViewChild('navbar', { static: true, read: ElementRef })
  navbarEl!: ElementRef;

  @ViewChild('info', { static: true, read: ElementRef })
  infoEl!: ElementRef;

  constructor(private router: Router, private route: ActivatedRoute) {
    this.router.events
      .pipe(
        filter(
          (event): event is NavigationEnd => event instanceof NavigationEnd
        ),

        tap((event: NavigationEnd) => {
          this.refreshActiveLink();
        })
      )
      .subscribe();

    this.refreshLinks();
  }

  get bannerUrl$() {
    return this.channel$.pipe(map((x) => x.userChannelInfo.bannerUrl));
  }

  get thumbnailUrl$() {
    return this.channel$.pipe(map((x) => x.userProfile.thumbnailUrl));
  }

  get displayName$() {
    return this.channel$.pipe(map((x) => x.userProfile.displayName));
  }

  onScroll() {
    this.updateBannerOffset();
  }

  updateBannerOffset() {
    const containerElTop = this.containerEl.nativeElement.scrollTop * 0.5;
    this.bannerOffsetStyle = `transform: translateY(${containerElTop}px)`;
  }

  resetScroll() {
    const scrollTop = this.containerEl.nativeElement.scrollTop;
    const infoBottom = this.infoEl.nativeElement.getBoundingClientRect().bottom;
    const navbarHeight =
      this.navbarEl.nativeElement.getBoundingClientRect().height;

    const navbarPos = scrollTop + infoBottom - navbarHeight;

    this.containerEl.nativeElement.scrollTop = Math.min(
      this.containerEl.nativeElement.scrollTop,
      navbarPos
    );
    this.updateBannerOffset();
  }

  ngOnInit(): void {
    this.channel$ = this.route.data.pipe(
      map((data) => data['channelData'] as ChannelData)
    );
  }

  refreshActiveLink() {
    const currentNav = this.navs.find((nav) => {
      return this.route.children.some((x) => x.component === nav.component);
    });

    if (!!currentNav) {
      this.activeLink = currentNav.link;
    }
  }

  refreshLinks() {
    this.navs = [];

    this.navs.push({
      label: 'HOME',
      link: 'featured',
      component: ChannelFeaturedComponent,
    });

    this.navs.push({
      label: 'VIDEOS',
      link: 'videos',
      component: ChannelVideosComponent,
    });

    this.navs.push({
      label: 'PLAYLISTS',
      link: 'playlists',
      component: ChannelPlaylistsComponent,
    });

    this.navs.push({
      label: 'ABOUT',
      link: 'about',
      component: ChannelAboutComponent,
    });
  }

  navigate(nav: ChannelNavigation) {
    this.activeLink = nav.link;
    this.router.navigate([nav.link], {
      relativeTo: this.route,
    });
  }
}

interface ChannelNavigation {
  label: string;
  link: string;
  component: Type<any>;
}
