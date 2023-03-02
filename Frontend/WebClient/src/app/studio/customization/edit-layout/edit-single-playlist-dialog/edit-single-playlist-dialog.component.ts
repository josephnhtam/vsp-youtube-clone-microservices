import {concatMap, filter, take} from 'rxjs';
import {AuthService} from 'src/app/auth/services/auth.service';
import {ChannelHelperService} from 'src/app/core/services/channel-helper.service';
import {DialogRef} from '@angular/cdk/dialog';
import {OnCompleteEdit} from './edit-single-playlist-dialog.service';
import {Component, OnInit} from '@angular/core';
import {ChannelSectionType, PlaylistInfo, SinglePlaylistSection,} from 'src/app/core/models/channel';

@Component({
  selector: 'app-edit-single-playlist-dialog',
  templateUrl: './edit-single-playlist-dialog.component.html',
  styleUrls: ['./edit-single-playlist-dialog.component.css'],
})
export class EditSinglePlaylistDialogComponent implements OnInit {
  private singlePlaylistSection: SinglePlaylistSection | null = null;
  private onCompleteEdit: OnCompleteEdit | null = null;

  playlistInfos: PlaylistInfo[] = [];

  constructor(
    private dialogRef: DialogRef,
    private service: ChannelHelperService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.authService.authInfo$
      .pipe(
        take(1),
        filter((authInfo) => !!authInfo),
        concatMap((authInfo) => {
          return this.service.getCreatedPlaylistInfos(authInfo!.sub);
        })
      )
      .subscribe((response) => {
        this.playlistInfos = response.infos;
      });
  }

  initialize(
    singlePlaylistSection: SinglePlaylistSection | null,
    onCompleteEdit: OnCompleteEdit
  ) {
    this.singlePlaylistSection = singlePlaylistSection;
    this.onCompleteEdit = onCompleteEdit;
  }

  close() {
    this.dialogRef.close();
  }

  select(playlistInfo: PlaylistInfo) {
    if (this.singlePlaylistSection != null) {
      this.singlePlaylistSection.content = {
        playlistId: playlistInfo.id,
      };
    } else {
      this.singlePlaylistSection = {
        type: ChannelSectionType.SinglePlaylist,
        id: null,
        content: {
          playlistId: playlistInfo.id,
        },
      };
    }

    if (this.onCompleteEdit) {
      this.onCompleteEdit(this.singlePlaylistSection);
    }

    this.close();
  }
}
