import {
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';

@Component({
  selector: 'app-upload-video',
  templateUrl: './upload-video.component.html',
  styleUrls: ['./upload-video.component.css'],
})
export class UploadVideoComponent implements OnInit {
  @Input() fileAccepted = false;
  @Input() allowedMimeTypes = [
    'video/mp4',
    'video/quicktime',
    'video/x-quicktime',
    'video/mpeg',
    'video/webm',
    'video/3gpp',
    'video/3gpp2',
    'video/x-flv',
    'video/x-matroska',
    'video/x-ms-wmv',
    'video/x-msvideo',
    'video/ogg',
    'application/vnd.rn-realmedia-vbr',
  ];
  @Input() acceptedExtensions =
    '.mp4,.mov,.webm,.flv,.mkv,.wmv,.ogg,.rmvb,.mpg,.mpeg,.3gp';
  @Output() fileToUpload = new EventEmitter<File[]>();

  @ViewChild('fileInput', { static: true })
  fileInput!: ElementRef;

  dragOver = false;

  constructor() {}

  ngOnInit(): void {}

  openFileInput() {
    if (this.fileAccepted) return;

    if (this.fileInput.nativeElement instanceof HTMLInputElement) {
      this.fileInput.nativeElement.click();
    }
  }

  onFileDragOver(dragOver: boolean) {
    this.dragOver = dragOver;
  }

  onFileDropped(fileList: FileList) {
    if (this.fileAccepted) {
      return;
    }

    const files = this.getFiles(fileList);

    if (files.length > 0) {
      this.fileToUpload.emit(files);
    }
  }

  private getFiles(fileList: FileList | null) {
    const files: File[] = [];

    if (!!fileList) {
      for (let i = 0; i < fileList.length; i++) {
        const file = fileList.item(i);

        if (!!file && this.allowedMimeTypes.some((x) => x === file.type)) {
          files.push(file);
        }
      }
    }

    return files;
  }

  onFileSelected(ev: Event) {
    if (!this.fileAccepted) {
      if (ev.target instanceof HTMLInputElement) {
        const files = this.getFiles(ev.target?.files);

        if (files.length > 0) {
          this.fileToUpload.emit(files);
        }
      }
    }

    return false;
  }
}
