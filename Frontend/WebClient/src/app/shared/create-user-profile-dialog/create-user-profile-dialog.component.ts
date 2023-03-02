import {CreateUserProfileDialogService} from './create-user-profile-dialog.service';
import {DialogRef} from '@angular/cdk/dialog';
import {Observable, of, switchMap, take, tap,} from 'rxjs';
import {AuthService} from 'src/app/auth/services/auth.service';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {Component, OnInit} from '@angular/core';

@Component({
  selector: 'app-create-user-profile-dialog',
  templateUrl: './create-user-profile-dialog.component.html',
  styleUrls: ['./create-user-profile-dialog.component.css'],
})
export class CreateUserProfileDialogComponent implements OnInit {
  formGroup: FormGroup;

  thumbnailUrl: string | null = null;
  private thumbnailFile: File | null = null;

  private maxThumbnailSize = 4096;
  private allowedThumbnailMimeTypes = ['image/png', 'image/jpeg'];
  allowedThumbnailExtensions = '.png,.jpg,.jpeg';

  constructor(
    private authService: AuthService,
    private service: CreateUserProfileDialogService,
    private dialog: DialogRef
  ) {
    const fb = new FormBuilder();

    this.formGroup = fb.group({
      displayName: ['', [Validators.required, Validators.maxLength(50)]],
    });
  }

  ngOnInit(): void {
    this.authService.authInfo$
      .pipe(
        take(1),
        tap((authInfo) => {
          if (authInfo) {
            this.formGroup.get('displayName')?.setValue(authInfo.name);
          }
        })
      )
      .subscribe();
  }

  onThumbnailFileSelected(ev: Event) {
    if (ev.target instanceof HTMLInputElement) {
      const file = ev.target?.files?.[0];

      const accept =
        !!file &&
        this.maxThumbnailSize * 1024 > file.size &&
        this.allowedThumbnailMimeTypes.some((x) => x === file.type);

      if (accept) {
        this.setThumbnailToUpload(file);
      }
    }

    return false;
  }

  setThumbnailToUpload(file: File) {
    this.thumbnailUrl = null;

    const fr = new FileReader();
    fr.readAsDataURL(file);
    fr.onload = () => {
      this.thumbnailUrl = fr.result as string;
    };

    this.thumbnailFile = file;
  }

  submit() {
    if (this.formGroup.invalid) return;

    const displayName = this.formGroup.get('displayName')!.value;

    this.formGroup.disable();

    let operation: Observable<boolean>;

    if (this.thumbnailFile == null) {
      operation = this.service.createUserProfile(displayName, null);
    } else {
      operation = this.service.uploadThumbnail(this.thumbnailFile).pipe(
        switchMap((thumbnailToken) => {
          if (!thumbnailToken) return of(false);

          return this.service.createUserProfile(displayName, thumbnailToken);
        })
      );
    }

    operation.subscribe((success) => {
      if (success) {
        this.dialog.close();
      } else {
        this.formGroup.enable();
      }
    });

    return false;
  }
}
