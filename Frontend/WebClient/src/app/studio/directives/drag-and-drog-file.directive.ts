import {Directive, EventEmitter, HostListener, Output} from '@angular/core';

@Directive({
  selector: '[appDragAndDrogFile]',
})
export class DragAndDrogFileDirective {
  @Output() fileDragOver = new EventEmitter<boolean>();
  @Output() fileDropped = new EventEmitter<FileList>();

  private draggingOver = 0;

  @HostListener('dragenter', ['$event'])
  private onDragEnter(event: Event) {
    event.preventDefault();
    event.stopPropagation();

    this.draggingOver++;
    this.fileDragOver.emit(true);
  }

  @HostListener('dragleave', ['$event'])
  private onDragLeave(event: Event) {
    event.preventDefault();
    event.stopPropagation();

    this.draggingOver--;
    if (this.draggingOver === 0) {
      this.fileDragOver.emit(false);
    }
  }

  @HostListener('dragover', ['$event'])
  private onDragOver(event: Event) {
    event.preventDefault();
    event.stopPropagation();
  }

  @HostListener('drop', ['$event'])
  private onDrop(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();

    const files = event.dataTransfer?.files;

    if (files && files.length > 0) {
      this.fileDropped.emit(files);
    } else {
      this.fileDragOver.emit(false);
    }
  }
}
