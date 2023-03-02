import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {DialogRef} from '@angular/cdk/dialog';
import {Component, OnInit} from '@angular/core';
import {ChannelHelperService} from 'src/app/core/services/channel-helper.service';
import {OnCompleteEdit} from './edit-multiple-playlists-dialog.service';
import {ChannelSectionType, MultiplePlaylistsSection, SimplePlaylistInfo,} from 'src/app/core/models/channel';

@Component({
  selector: 'app-edit-multiple-playlists-dialog',
  templateUrl: './edit-multiple-playlists-dialog.component.html',
  styleUrls: ['./edit-multiple-playlists-dialog.component.css'],
})
export class EditMultiplePlaylistsDialogComponent implements OnInit {
  formGroup: FormGroup;

  playlistsInSection: SimplePlaylistInfo[] | null = null;

  private multiplePlaylistsSection!: MultiplePlaylistsSection | null;
  private onCompleteEdit: OnCompleteEdit | null = null;

  constructor(
    private dialogRef: DialogRef,
    private service: ChannelHelperService
  ) {
    const fb = new FormBuilder();

    this.formGroup = fb.group({
      title: ['', [Validators.maxLength(50)]],
    });
  }

  ngOnInit(): void {}

  initialize(
    multiplePlaylistsSection: MultiplePlaylistsSection | null,
    onCompleteEdit: OnCompleteEdit
  ) {
    this.multiplePlaylistsSection = multiplePlaylistsSection;
    this.onCompleteEdit = onCompleteEdit;

    if (!!multiplePlaylistsSection) {
      this.formGroup
        .get('title')
        ?.setValue(multiplePlaylistsSection.content.title);

      this.service
        .getPublicSimplePlaylistInfos(
          multiplePlaylistsSection.content.playlistIds ?? []
        )
        .subscribe((playlistInfos) => {
          this.playlistsInSection = playlistInfos;
        });
    } else {
      this.playlistsInSection = [];
    }
  }

  valid() {
    return (this.formGroup.valid && this.playlistsInSection?.length) ?? 0 > 0;
  }

  close() {
    this.dialogRef.close();
  }

  done() {
    if (!this.valid) {
      return;
    }

    if (this.multiplePlaylistsSection != null) {
      this.multiplePlaylistsSection.content.title =
        this.formGroup.value['title'] ?? '';
      this.multiplePlaylistsSection.content.playlistIds =
        this.playlistsInSection!.map((x) => x.id);
    } else {
      this.multiplePlaylistsSection = {
        type: ChannelSectionType.MultiplePLaylists,
        id: null,
        content: {
          title: this.formGroup.value['title'] ?? '',
          playlistIds: this.playlistsInSection!.map((x) => x.id),
        },
      };
    }

    if (this.onCompleteEdit) {
      this.onCompleteEdit(this.multiplePlaylistsSection);
    }

    this.close();
  }
}
