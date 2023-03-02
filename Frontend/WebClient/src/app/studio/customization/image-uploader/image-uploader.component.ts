import {Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild,} from '@angular/core';

@Component({
  selector: 'app-image-uploader',
  templateUrl: './image-uploader.component.html',
  styleUrls: ['./image-uploader.component.css'],
})
export class ImageUploaderComponent implements OnInit {
  @Input() fileAccepted = false;
  @Input() allowedMimeTypes = ['image/png', 'image/jpeg'];
  @Input() acceptedExtensions = '.png,.jpg,.jpeg';
  @Input() maxSize = 4096;

  @Output() fileToUpload = new EventEmitter<File>();
  @Output() fileRemoved = new EventEmitter<void>();

  @ViewChild('fileInput', { static: true })
  fileInput!: ElementRef;

  constructor() {}

  ngOnInit(): void {}

  openFileInput() {
    if (this.fileInput.nativeElement instanceof HTMLInputElement) {
      this.fileInput.nativeElement.click();
    }
  }

  removeFile() {
    this.fileAccepted = false;
    this.fileRemoved.emit();
  }

  onFileSelected(ev: Event) {
    if (ev.target instanceof HTMLInputElement) {
      const file = ev.target?.files?.[0];

      const accept =
        !!file &&
        this.maxSize * 1024 > file.size &&
        this.allowedMimeTypes.some((x) => x === file.type);

      if (accept) {
        this.fileToUpload.emit(file);
      }
    }

    return false;
  }
}
