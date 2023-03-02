import {Component, Input, OnInit} from '@angular/core';
import {FormGroup} from '@angular/forms';
import {Observable} from 'rxjs';
import {VideoClient} from 'src/app/studio/videos-management/models';

@Component({
  selector: 'app-edit-video-visibility',
  templateUrl: './edit-video-visibility.component.html',
  styleUrls: ['./edit-video-visibility.component.css'],
})
export class EditVideoVisibilityComponent implements OnInit {
  @Input('videoClient')
  videoClient$!: Observable<VideoClient | undefined>;

  @Input()
  visibilityFormGroup!: FormGroup;

  constructor() {}

  ngOnInit(): void {}

  set visibility(value) {
    this.visibilityFormGroup.get('visibility')?.setValue(value);
    this.visibilityFormGroup.markAsDirty();
  }

  get visibility() {
    return Number(this.visibilityFormGroup.get('visibility')?.value ?? 0);
  }
}
