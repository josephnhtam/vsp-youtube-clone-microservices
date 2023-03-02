import {Component, Input, OnInit} from '@angular/core';
import {AbstractControl, FormGroup} from '@angular/forms';
import {map, Observable} from 'rxjs';
import {VideoResource} from 'src/app/video-player/video-player/models';
import {VideoProcessingStatus} from 'src/app/studio/models';
import {VideoClient} from 'src/app/studio/videos-management/models';
import {COMMA, ENTER} from '@angular/cdk/keycodes';
import {MatChipInputEvent} from '@angular/material/chips';

interface Tag {
  keyword: string;
}

@Component({
  selector: 'app-edit-video-details',
  templateUrl: './edit-video-details.component.html',
  styleUrls: ['./edit-video-details.component.css'],
})
export class EditVideoDetailsComponent implements OnInit {
  @Input('videoClient')
  videoClient$!: Observable<VideoClient | undefined>;

  @Input()
  infoFormGroup!: FormGroup;

  addOnBlur = true;
  readonly separatorKeysCodes = [ENTER, COMMA] as const;
  tags: Tag[] = [];

  constructor() {}

  ngOnInit(): void {
    this.tags = (this.infoFormGroup.get('tags')?.value as string)
      .split(',')
      .filter((x) => !!x.trim())
      .map((x) => {
        return { keyword: x };
      });
  }

  hasError(control: AbstractControl | null, error?: string) {
    if (!control?.touched || !control?.invalid) return false;

    if (error) {
      return control.hasError(error);
    } else {
      return true;
    }
  }

  selectThumbnail(thumbnailIndex: number) {
    this.infoFormGroup.get('thumbnailIndex')?.setValue(thumbnailIndex);
    this.infoFormGroup.markAsDirty();
    return false;
  }

  get thumbnailIndex() {
    return this.infoFormGroup.get('thumbnailIndex')?.value ?? 0;
  }

  get videoThumbnails$() {
    return this.videoClient$.pipe(map((vc) => vc?.video?.thumbnails ?? []));
  }

  get areVideoThumbnailsProcessed$() {
    return this.videoThumbnails$.pipe(map((x) => x.length > 0));
  }

  get videoPreviewAvailable$() {
    return this.videoPreview$.pipe(map((x) => x != null));
  }

  get videoPreview$() {
    return this.videoClient$.pipe(
      map((vc) => {
        if (!vc?.video?.videos) return null;

        const videos = [...vc.video.videos].sort((a, b) => a.height - b.height);
        if (videos && videos.length > 0) {
          return videos[0];
        } else {
          return null;
        }
      })
    );
  }

  get videoPreviewResource$(): Observable<VideoResource[]> {
    return this.videoPreview$.pipe(
      map((video) => {
        return [video!];
      })
    );
  }

  getVideoStyle$() {
    return this.videoPreview$.pipe(
      map((video) => {
        if (!video) return '';
        const aspectRatio = video.width / video.height;
        return `width: 100%; padding-top: ${(1 / aspectRatio) * 100}%`;
      })
    );
  }

  get hasOriginalVideoFileName$() {
    return this.originalVideoFileName$.pipe(map((n) => n != null));
  }

  get originalVideoFileName$() {
    return this.videoClient$.pipe(
      map((vc) => vc?.video?.originalVideoFileName ?? null)
    );
  }

  get isVideoUploaded$() {
    return this.videoClient$.pipe(
      map(
        (vc) =>
          vc?.video?.processingStatus !=
          VideoProcessingStatus.WaitingForUserUpload
      )
    );
  }

  get isProcessingFailed$() {
    return this.videoClient$.pipe(
      map(
        (vc) =>
          vc?.video?.processingStatus ==
          VideoProcessingStatus.VideoProcessingFailed
      )
    );
  }

  private updateTags() {
    const tags = this.tags
      .map((x) => x.keyword)
      .filter((x) => !!x)
      .join(',');

    this.infoFormGroup.get('tags')?.setValue(tags);
    this.infoFormGroup.get('tags')?.markAsTouched();
    this.infoFormGroup.get('tags')?.markAsDirty();
  }

  isTagsInputValid() {
    return this.infoFormGroup.get('tags')?.valid;
  }

  add(event: MatChipInputEvent): void {
    const value = (event.value || '').trim();
    if (value) {
      this.tags.push({ keyword: value });
    }
    event.chipInput!.clear();
    this.updateTags();
  }

  remove(tag: Tag): void {
    const index = this.tags.indexOf(tag);

    if (index >= 0) {
      this.tags.splice(index, 1);
    }
    this.updateTags();
  }

  edit(tag: Tag, event: MatChipInputEvent) {
    const value = event.value.trim();

    if (!value) {
      this.remove(tag);
      return;
    }

    const index = this.tags.indexOf(tag);
    if (index > 0) {
      this.tags[index].keyword = value;
    }
    this.updateTags();
  }
}
