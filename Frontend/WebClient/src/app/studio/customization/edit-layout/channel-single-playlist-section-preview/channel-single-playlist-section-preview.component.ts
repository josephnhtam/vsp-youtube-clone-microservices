import {Router} from '@angular/router';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {catchError, finalize, tap} from 'rxjs';
import {ChannelHelperService} from 'src/app/core/services/channel-helper.service';
import {HiddenVideo, Playlist, SinglePlaylistSection,} from 'src/app/core/models/channel';
import {EditSinglePlaylistDialogService} from '../edit-single-playlist-dialog/edit-single-playlist-dialog.service';

@Component({
  selector: 'app-channel-single-playlist-section-preview',
  templateUrl: './channel-single-playlist-section-preview.component.html',
  styleUrls: ['./channel-single-playlist-section-preview.component.css'],
})
export class ChannelSinglePlaylistSectionPreviewComponent implements OnInit {
  @Input() section!: SinglePlaylistSection;
  @Input() maxPreviewCount: number = 5;
  @Output() onRemoved = new EventEmitter();

  pending: boolean = false;
  playlist: Playlist | null = null;

  constructor(
    private service: ChannelHelperService,
    private editSinglePlaylistDialogService: EditSinglePlaylistDialogService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadSection();
  }

  loadSection() {
    this.pending = true;

    this.service
      .getPlaylist(this.section.content.playlistId, 1, this.maxPreviewCount)
      .pipe(
        finalize(() => {
          this.pending = false;
        }),

        tap((playlist) => {
          this.playlist = playlist;
        }),

        catchError((error) => {
          console.error(error);
          this.playlist = null;
          throw error;
        })
      )
      .subscribe();
  }

  get totalCount() {
    return this.playlist?.itemsCount ?? 0;
  }

  get videos() {
    return this.playlist?.items.map((i) => i.video) ?? [];
  }

  get title() {
    return this.playlist?.title;
  }

  get hiddenVideoDummy() {
    const hiddenVideo: HiddenVideo = {
      id: '',
      visibility: null,
    };

    return hiddenVideo;
  }

  edit() {
    this.editSinglePlaylistDialogService.openDialog(this.section, (section) => {
      this.section.content.playlistId = section.content.playlistId;
      this.loadSection();
    });
  }
}
