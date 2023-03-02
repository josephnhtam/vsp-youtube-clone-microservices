import { ConfirmDialogService } from '../../../shared/confirm-dialog/confirm-dialog.service';
import { AuthService } from '../../../auth/services/auth.service';
import { concatLatestFrom } from '@ngrx/effects';
import { Router } from '@angular/router';
import { PlaylistVisibility } from '../../../core/models/library';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { selectAvailablePlaylistItems } from '../../../core/selectors/playlist';
import { map, Observable, of, take, tap } from 'rxjs';
import { Store } from '@ngrx/store';
import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  Input,
  OnInit,
  ViewChild,
} from '@angular/core';
import { PlaylistManagementAction } from 'src/app/core/actions';
import {
  selectPlaylistManagementState,
  selectPlaylistState,
} from 'src/app/core/selectors';
import * as moment from 'moment';
import { environment } from 'src/environments/environment';

// @ts-ignore
import ColorThief from '@laverdet/lokesh-colorthief';
import { getChannelLink } from 'src/app/core/services/utilities';

@Component({
  selector: 'app-playlist-header',
  templateUrl: './playlist-header.component.html',
  styleUrls: ['./playlist-header.component.css'],
})
export class PlaylistHeaderComponent implements OnInit {
  @Input()
  playlistId!: string;

  @ViewChild('ThumbnailImage')
  imageEl!: ElementRef;

  @ViewChild('titleEl')
  titleEl!: ElementRef;

  @ViewChild('descEl')
  descEl!: ElementRef;

  backgroundStyle: { [key: string]: any } | null = null;
  gradientBackgroundStyle: { [key: string]: any } | null = null;
  editingTitle = false;
  editingDesc = false;

  editTitleFormGroup!: FormGroup;
  editDescFormGroup!: FormGroup;

  constructor(
    private store: Store,
    private changeDetector: ChangeDetectorRef,
    private router: Router,
    private authService: AuthService,
    private confirmDialog: ConfirmDialogService
  ) {}

  ngOnInit() {}

  get state$() {
    return this.store.select(selectPlaylistState);
  }

  get isPlaylistRemovable$() {
    return this.state$
      .pipe(map((x) => x.playlistInfo?.visibility != null))
      .pipe(
        concatLatestFrom(() => this.isReadonly$),
        map(([isEditable, isReadonly]) => isEditable && !isReadonly)
      );
  }

  get isMine$() {
    return this.creatorProfile$.pipe(
      concatLatestFrom(() => this.authService.authInfo$),
      map(([creatorProfile, authInfo]) => {
        return creatorProfile?.id == authInfo?.sub;
      })
    );
  }

  get canEdit$() {
    return this.isMine$.pipe(
      map((isMine) => {
        if (!isMine) return false;

        switch (this.playlistId.toUpperCase()) {
          case 'WL':
            return false;
          case 'LL':
            return false;
          case 'DL':
            return false;
          default:
            return !this.isVideosList;
        }
      })
    );
  }

  get hasAction() {
    switch (this.playlistId.toUpperCase()) {
      case 'WL':
        return false;
      case 'LL':
        return false;
      case 'DL':
        return false;
      default:
        return !this.isVideosList;
    }
  }

  get isVideosList() {
    return this.playlistId.toUpperCase().startsWith('VIDEOS-');
  }

  get isReadonly$() {
    if (this.isVideosList) {
      return of(true);
    }

    return this.isMine$.pipe(map((x) => !x));
  }

  get title$() {
    switch (this.playlistId.toUpperCase()) {
      case 'WL':
        return of('Watch later');
      case 'LL':
        return of('Liked videos');
      case 'DL':
        return of('Disliked videos');
    }

    if (this.isVideosList) {
      return of('Videos');
    }

    return this.state$.pipe(map((x) => x.playlistInfo?.title ?? undefined));
  }

  get updateDate$() {
    return this.state$.pipe(
      map((x) => moment(x.playlistInfo?.updateDate).format('ll'))
    );
  }

  get description$() {
    return this.state$.pipe(map((x) => x.playlistInfo?.description));
  }

  get visibility$() {
    return this.state$.pipe(
      map((x) => x.playlistInfo?.visibility),
      map((visiblity) => {
        switch (visiblity ?? PlaylistVisibility.Private) {
          case PlaylistVisibility.Private:
            return 'Private';
          case PlaylistVisibility.Unlisted:
            return 'Unlisted';
          case PlaylistVisibility.Public:
            return 'Public';
        }
      })
    );
  }

  get videosCount$() {
    return this.state$.pipe(map((x) => x.videosCount));
  }

  get creatorProfile$() {
    return this.state$.pipe(map((x) => x.playlistInfo?.creatorProfile));
  }

  get featuredVideo$() {
    return this.store
      .select(selectAvailablePlaylistItems)
      .pipe(map((x) => (x.length > 0 ? x[0] : null)));
  }

  get featuredVideoThumbnailUrl$() {
    return this.featuredVideo$.pipe(
      map(
        (x) =>
          (x?.video as any).thumbnailUrl ||
          environment.assetSetup.noThumbnailIconUrl
      ),
      tap((x) => {
        if (!x) {
          this.backgroundStyle = null;
          this.gradientBackgroundStyle = null;
        }
      })
    );
  }

  async getColor(): Promise<any> {
    const colorThief = new ColorThief();
    return await colorThief.getPalette(this.imageEl.nativeElement, 2)[0];
  }

  async thumbnailLoaded() {
    let color = (await this.getColor()) as number[];

    const maxMagnitude = 255;

    const magnitude = Math.sqrt(
      color[0] * color[0] + color[1] * color[1] + color[2] * color[2]
    );

    if (magnitude > maxMagnitude) {
      color[0] *= maxMagnitude / magnitude;
      color[1] *= maxMagnitude / magnitude;
      color[2] *= maxMagnitude / magnitude;
    }

    const colorStr = `rgba(${color[0]}, ${color[1]}, ${color[2]})`;

    color[0] *= 0.75;
    color[1] *= 0.75;
    color[2] *= 0.75;

    const colorStr08 = `rgba(${color[0]}, ${color[1]}, ${color[2]}, 0.8)`;
    const colorStr03 = `rgba(${color[0]}, ${color[1]}, ${color[2]}, 0.3)`;

    this.backgroundStyle = {
      'background-color': colorStr,
    };

    this.gradientBackgroundStyle = {
      background: `linear-gradient(to bottom, ${colorStr08} 0%, ${colorStr03} 33%, ${colorStr08} 100%)`,
    };
  }

  get channelLink$() {
    return this.creatorProfile$.pipe(
      map((creatorProfile) => getChannelLink(creatorProfile))
    );
  }

  editTitle() {
    this.editingTitle = true;

    this.title$.pipe(take(1)).subscribe({
      next: (title) => {
        const fb = new FormBuilder();
        this.editTitleFormGroup = fb.group({
          title: fb.control(title, [
            Validators.required,
            Validators.maxLength(50),
          ]),
        });
      },
    });

    this.changeDetector.detectChanges();
    this.titleEl.nativeElement?.focus();
  }

  cancelEditTitle() {
    this.editingTitle = false;
  }

  saveTitle() {
    if (this.editTitleFormGroup.invalid) return;

    this.store.dispatch(
      PlaylistManagementAction.updatePlaylist({
        playlistId: this.playlistId,
        title: this.editTitleFormGroup.get('title')?.value,
      })
    );

    this.cancelEditTitle();
  }

  editDesc() {
    this.editingDesc = true;

    this.description$.pipe(take(1)).subscribe({
      next: (description) => {
        const fb = new FormBuilder();
        this.editDescFormGroup = fb.group({
          description: fb.control(description, [Validators.maxLength(5000)]),
        });
      },
    });

    this.changeDetector.detectChanges();
    this.descEl.nativeElement?.focus();
  }

  cancelEditDesc() {
    this.editingDesc = false;
  }

  saveDesc() {
    if (this.editDescFormGroup.invalid) return;

    this.store.dispatch(
      PlaylistManagementAction.updatePlaylist({
        playlistId: this.playlistId,
        description: this.editDescFormGroup.get('description')?.value,
      })
    );

    this.cancelEditDesc();
  }

  saveVisibility(visibility: PlaylistVisibility) {
    this.store.dispatch(
      PlaylistManagementAction.updatePlaylist({
        playlistId: this.playlistId,
        visibility,
      })
    );
  }

  deletePlaylist() {
    this.title$.pipe(take(1)).subscribe((title) => {
      this.confirmDialog.openConfirmDialog(
        'Delete playlist',
        `<div>Are you sure you want to delete <strong>${title}</strong></div>
         <div class="note">Note: Deleting playlists is a permanent action and cannot be undone.</div>`,
        null,
        this.doDeletePlaylist.bind(this),
        null,
        'Delete',
        'Cancel'
      );
    });
  }

  doDeletePlaylist() {
    this.store.dispatch(
      PlaylistManagementAction.removePlaylist({
        playlistId: this.playlistId,
      })
    );
  }

  playAll() {
    this.store
      .select(selectAvailablePlaylistItems)
      .pipe(
        take(1),

        tap((items) => {
          if (items.length == 0) return;

          const videoId = items[0].video.id;
          this.router.navigate(['/watch', videoId], {
            queryParams: {
              list: this.playlistId,
            },
          });
        })
      )
      .subscribe();
  }

  playShuffle() {
    this.store
      .select(selectAvailablePlaylistItems)
      .pipe(
        take(1),

        tap((items) => {
          if (items.length == 0) return;

          const videoId =
            items[Math.floor(Math.random() * items.length)].video.id;

          this.router.navigate(['/watch', videoId], {
            queryParams: {
              list: this.playlistId,
            },
            state: {
              shuffle: true,
            },
          });
        })
      )
      .subscribe();
  }

  createPlaylistRef() {
    if (this.playlistId == null) return;

    this.title$.pipe(take(1)).subscribe((title) => {
      this.store.dispatch(
        PlaylistManagementAction.createPlaylistRef({
          playlistId: this.playlistId!,
          title: title,
        })
      );
    });
  }

  removePlaylistRef() {
    if (this.playlistId == null) return;

    this.title$.pipe(take(1)).subscribe((title) => {
      this.store.dispatch(
        PlaylistManagementAction.removePlaylistRef({
          playlistId: this.playlistId!,
        })
      );
    });
  }

  get hasPlaylistRef$(): Observable<boolean> {
    return this.store
      .select(selectPlaylistManagementState)
      .pipe(
        map((x) => !!this.playlistId && x.entities[this.playlistId] != null)
      );
  }
}
