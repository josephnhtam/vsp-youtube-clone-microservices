import {
  EditMultiplePlaylistsDialogService
} from './../edit-multiple-playlists-dialog/edit-multiple-playlists-dialog.service';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {catchError, finalize, tap} from 'rxjs';
import {ChannelHelperService} from 'src/app/core/services/channel-helper.service';
import {MultiplePlaylistsSection, PlaylistInfo,} from 'src/app/core/models/channel';

@Component({
  selector: 'app-channel-multiple-playlists-section-preview',
  templateUrl: './channel-multiple-playlists-section-preview.component.html',
  styleUrls: ['./channel-multiple-playlists-section-preview.component.css'],
})
export class ChannelMultiplePlaylistsSectionPreviewComponent implements OnInit {
  @Input() section!: MultiplePlaylistsSection;
  @Input() maxPreviewCount: number = 5;
  @Output() onRemoved = new EventEmitter();

  pending: boolean = false;
  playlistInfos: PlaylistInfo[] | null = null;

  constructor(
    private service: ChannelHelperService,
    private editMultiplePlaylistsDialogService: EditMultiplePlaylistsDialogService
  ) {}

  ngOnInit(): void {
    this.loadSection();
  }

  loadSection() {
    this.pending = true;

    this.service
      .getPublicPlaylistInfos(this.section.content.playlistIds)
      .pipe(
        finalize(() => {
          this.pending = false;
        }),

        tap((infos) => {
          this.playlistInfos = infos;
        }),

        catchError((error) => {
          console.error(error);

          this.playlistInfos = [];

          throw error;
        })
      )
      .subscribe();
  }

  edit() {
    this.editMultiplePlaylistsDialogService.openDialog(
      this.section,
      (section) => {
        this.section.content.title = section.content.title;
        this.section.content.playlistIds = section.content.playlistIds;
        this.loadSection();
      }
    );
  }

  get title() {
    const title = this.section.content.title.trim();
    if (title != '') {
      return `Multiple playlists: ${title}`;
    } else {
      return 'Multiple playlists';
    }
  }
}
