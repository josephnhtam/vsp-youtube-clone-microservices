import { PlaylistsPageContent } from '../../../../core/reducers/channel-page';
import { Guid } from 'guid-typescript';
import { ActivatedRoute, Router } from '@angular/router';
import { catchError, finalize, map, of, take, tap } from 'rxjs';
import {
  ChannelSection,
  ChannelSectionType,
  PlaylistInfo,
  UserChannel,
} from 'src/app/core/models/channel';
import { environment } from './../../../../../environments/environment';
import { Store } from '@ngrx/store';
import { HttpClient } from '@angular/common/http';
import {
  Component,
  ElementRef,
  Input,
  OnChanges,
  OnInit,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
import { ChannelPageAction } from 'src/app/core/actions';
import { ChannelPageType } from 'src/app/core/reducers/channel-page';
import {
  selectChannelPage,
  selectHasMoreChannelPageItems,
} from 'src/app/core/selectors/channel-page';
import { concatLatestFrom } from '@ngrx/effects';

@Component({
  selector: 'app-channel-playlists-view',
  templateUrl: './channel-playlists-view.component.html',
  styleUrls: ['./channel-playlists-view.component.css'],
})
export class ChannelPlaylistsViewComponent implements OnInit, OnChanges {
  @Input() channel!: UserChannel;
  @Input() sectionId: string | null = null;
  @Input() pageSize = 30;

  @ViewChild('bottomSection', { static: true })
  bottomSection!: ElementRef;

  private bottomSectionObserver: IntersectionObserver;
  private contextId!: string;
  private loadingSection = false;

  constructor(
    private httpClient: HttpClient,
    private store: Store,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.bottomSectionObserver = new IntersectionObserver(
      this.onObserveBottom.bind(this),
      {
        root: null,
        threshold: 0,
      }
    );
  }

  ngOnInit(): void {
    this.bottomSectionObserver.observe(this.bottomSection.nativeElement);
  }

  ngOnChanges(changes: SimpleChanges) {
    this.loadPlaylists();
  }

  get channelPage$() {
    return this.store.select(
      selectChannelPage(this.channel.id, this.contextId)
    );
  }

  get channelPageContent$() {
    return this.channelPage$.pipe(
      map((page) => page?.content as PlaylistsPageContent)
    );
  }

  get playlists$() {
    return this.channelPageContent$.pipe(
      map((content) => content?.items as PlaylistInfo[])
    );
  }

  get loaded$() {
    if (this.loadingSection) return of(false);
    return this.channelPage$.pipe(map((page) => page?.loaded));
  }

  get pending$() {
    if (this.loadingSection) return of(true);
    return this.channelPage$.pipe(map((page) => page?.pending));
  }

  get hasMoreItems$() {
    return this.store.select(
      selectHasMoreChannelPageItems(this.channel.id, this.contextId)
    );
  }

  onObserveBottom(
    entries: IntersectionObserverEntry[],
    observer: IntersectionObserver
  ) {
    const [entry] = entries;

    if (entry.isIntersecting) {
      this.hasMoreItems$
        .pipe(
          take(1),

          concatLatestFrom(() => this.loaded$),

          tap(([hasMoreItems, loaded]) => {
            if (hasMoreItems && loaded) {
              this.showMoreResults();
            }
          })
        )
        .subscribe();
    }
  }

  showMoreResults() {
    this.store.dispatch(
      ChannelPageAction.loadMoreResults({
        userId: this.channel.id,
        contextId: this.contextId,
      })
    );
  }

  loadPlaylists() {
    this.refreshContextId();

    if (this.sectionId != null) {
      const url =
        environment.appSetup.apiUrl +
        `/api/v1/UserChannels/${this.channel.id}/Section/${this.sectionId}`;

      this.loadingSection = true;
      this.httpClient
        .get<ChannelSection>(url)
        .pipe(
          finalize(() => {
            this.loadingSection = false;
          }),

          catchError((error) => {
            this.failed(error);
            throw error;
          })
        )
        .subscribe((section) => {
          if (section.type == ChannelSectionType.MultiplePLaylists) {
            this.store.dispatch(
              ChannelPageAction.loadChannelPage({
                userId: this.channel.id,
                contextId: this.contextId!,
                pageSize: this.pageSize,
                request: {
                  type: ChannelPageType.MultiplePlaylists,
                  playlistIds: section.content.playlistIds,
                },
              })
            );
          } else {
            this.failed('Invalid section');
          }
        });
    } else {
      this.store.dispatch(
        ChannelPageAction.loadChannelPage({
          userId: this.channel.id,
          contextId: this.contextId,
          pageSize: this.pageSize,
          request: {
            type: ChannelPageType.CreatedPlaylists,
          },
        })
      );
    }
  }

  private failed(error: any) {
    console.error(error);

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: {
        view: 'overview',
      },
      queryParamsHandling: 'merge',
    });
  }

  private refreshContextId() {
    this.contextId = Guid.create().toString();
  }
}
