import {
  EditMultiplePlaylistsDialogService
} from './edit-multiple-playlists-dialog/edit-multiple-playlists-dialog.service';
import {EditSinglePlaylistDialogService} from './edit-single-playlist-dialog/edit-single-playlist-dialog.service';
import {Store} from '@ngrx/store';
import {Component, OnDestroy, OnInit} from '@angular/core';
import {LayoutUpdate, UserData} from '../models';
import {selectUserData} from '../selectors';
import {Subscription, take, tap} from 'rxjs';
import {
  ChannelSection,
  ChannelSectionType,
  CreatedPlaylistsSection,
  VideosSection,
} from 'src/app/core/models/channel';

@Component({
  selector: 'app-edit-layout',
  templateUrl: './edit-layout.component.html',
  styleUrls: ['./edit-layout.component.css'],
})
export class EditLayoutComponent implements OnInit, OnDestroy {
  userData!: UserData;
  unsubscribedSpotlightVideoId: string | null = null;
  subscribedSpotlightVideoId: string | null = null;
  sections!: ChannelSection[];

  maxSectionsCount = 12;

  private sectionsJson!: string;
  private userDataUpdatedSub?: Subscription;

  constructor(
    private store: Store,
    private editSinglePlaylistDialogService: EditSinglePlaylistDialogService,
    private editMultiplePlaylistsDialogService: EditMultiplePlaylistsDialogService
  ) {}

  ngOnDestroy(): void {
    this.userDataUpdatedSub?.unsubscribe();
  }

  ngOnInit(): void {
    this.userDataUpdatedSub = this.store
      .select(selectUserData)
      .pipe(
        tap(() => {
          this.reset();
        })
      )
      .subscribe();
  }

  isValid() {
    return true;
  }

  reset() {
    this.store
      .select(selectUserData)
      .pipe(
        take(1),
        tap((userData) => {
          if (userData != null) {
            this.syncUiWithUserData(userData);
          }
        })
      )
      .subscribe();

    this.clearDirty();
  }

  syncUiWithUserData(userData: UserData) {
    this.userData = userData;
    this.sectionsJson = JSON.stringify(userData.userChannel.sections);
    this.sections = JSON.parse(this.sectionsJson);
    this.unsubscribedSpotlightVideoId =
      userData.userChannel.unsubscribedSpotlightVideoId;
    this.subscribedSpotlightVideoId =
      userData.userChannel.subscribedSpotlightVideoId;
  }

  isDirty() {
    if (
      this.unsubscribedSpotlightVideoId !=
      this.userData.userChannel.unsubscribedSpotlightVideoId
    ) {
      return true;
    }

    if (
      this.subscribedSpotlightVideoId !=
      this.userData.userChannel.subscribedSpotlightVideoId
    ) {
      return true;
    }

    if (this.sectionsJson != JSON.stringify(this.sections)) {
      return true;
    }

    return false;
  }

  getUpdate(): LayoutUpdate | null {
    if (this.isValid() && this.isDirty()) {
      return {
        unsubscribedSpotlightVideoId: this.unsubscribedSpotlightVideoId,
        subscribedSpotlightVideoId: this.subscribedSpotlightVideoId,
        channelSections: this.sections,
      };
    }

    return null;
  }

  clearDirty() {}

  addVideoSection() {
    const videosSection: VideosSection = {
      id: null,
      type: ChannelSectionType.Videos,
    };

    this.sections.push(videosSection);
  }

  addCreatedPlaylistsSection() {
    const createdPlaylistsSection: CreatedPlaylistsSection = {
      id: null,
      type: ChannelSectionType.CreatedPlaylists,
    };

    this.sections.push(createdPlaylistsSection);
  }

  addSinglePlaylistSection() {
    this.editSinglePlaylistDialogService.openDialog(null, (section) => {
      this.sections.push(section);
    });
  }

  addMultiplePlaylistsSection() {
    this.editMultiplePlaylistsDialogService.openDialog(null, (section) => {
      this.sections.push(section);
    });
  }

  canAddVideosSection() {
    if (this.sections.length > this.maxSectionsCount) {
      return false;
    }

    return !this.sections.some((x) => x.type === ChannelSectionType.Videos);
  }

  canAddCreatedPlaylistsSection() {
    if (this.sections.length > this.maxSectionsCount) {
      return false;
    }

    return !this.sections.some(
      (x) => x.type === ChannelSectionType.CreatedPlaylists
    );
  }

  canAddSinglePlaylistSection() {
    if (this.sections.length > this.maxSectionsCount) {
      return false;
    }

    return true;
  }

  canAddMultiplePlaylistsSection() {
    if (this.sections.length > this.maxSectionsCount) {
      return false;
    }

    return true;
  }
}
