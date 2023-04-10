import { AuthService } from 'src/app/auth/services/auth.service';
import { CreateUserProfileDialogComponent } from './create-user-profile-dialog.component';
import { Injectable } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { HttpClient, HttpEventType } from '@angular/common/http';
import { Actions, ofType } from '@ngrx/effects';
import {
  catchError,
  filter,
  map,
  Observable,
  of,
  switchMap,
  take,
  tap,
} from 'rxjs';
import { environment } from 'src/environments/environment';
import { UsersAction, UsersApiAction } from '../../core/actions';
import { Store } from '@ngrx/store';

@Injectable()
export class CreateUserProfileDialogService {
  dialogRef: MatDialogRef<CreateUserProfileDialogComponent> | null = null;

  constructor(
    private dialog: MatDialog,
    private store: Store,
    private actions$: Actions,
    private httpClient: HttpClient,
    private authService: AuthService
  ) {}

  openCreateUserProfileDialog() {
    if (!this.dialogRef) {
      const dialogRef = this.dialog.open(CreateUserProfileDialogComponent, {
        width: '90dvw',
        height: '90dvh',
        maxWidth: '638px',
        maxHeight: '450px',
        disableClose: true,
        autoFocus: false,
        restoreFocus: false,
        panelClass: 'create-user-profile-dialog',
      });

      this.dialogRef = dialogRef;

      dialogRef.afterClosed().subscribe(() => {
        if (this.dialogRef == dialogRef) {
          this.dialogRef = null;
        }
      });
    }
  }

  uploadThumbnail(file: File): Observable<string | null> {
    const url =
      environment.appSetup.apiUrl + '/api/v1/Users/Thumbnail/UploadToken';

    return this.httpClient.get<{ uploadToken: string }>(url).pipe(
      switchMap(({ uploadToken }) => {
        const url = environment.appSetup.storageUrl + '/api/v1/ImageStorage';

        const formData = new FormData();
        formData.append('file', file);

        const params = {
          token: uploadToken,
        };

        return this.httpClient
          .post(url, formData, {
            params,
            observe: 'events',
          })
          .pipe(
            filter((response) => response.type === HttpEventType.Response),

            map((response) => {
              if (response.type === HttpEventType.Response) {
                console.log(response.body);
                return (response.body as any).token;
              }
            }),

            catchError((error) => {
              console.error(error);
              this.showErrorMsg('Failed to upload thumbnail.');
              return of(null);
            })
          );
      }),

      catchError((error) => {
        console.error(error);
        this.showErrorMsg('Failed to upload thumbnail.');
        return of(null);
      })
    );
  }

  getUserProfile() {
    this.authService.authInfo$
      .pipe(
        take(1),

        tap((authInfo) => {
          if (!authInfo) return;
          this.store.dispatch(
            UsersAction.getUserProfile({ userId: authInfo.sub })
          );
        })
      )
      .subscribe();
  }

  createUserProfile(
    displayName: string,
    thumbnailToken: string | null
  ): Observable<boolean> {
    this.store.dispatch(
      UsersAction.createUser({ displayName, thumbnailToken })
    );

    return this.actions$.pipe(
      ofType(UsersApiAction.userCreated, UsersApiAction.failedToCreateUser),

      map((action) => {
        if (action.type === UsersApiAction.userCreated.type) {
          this.getUserProfile();
          // this.dialog.close();
          return true;
        } else {
          if (action.error.status === 409) {
            // User profile already created
            this.getUserProfile();
            // this.dialog.close();
            return true;
          } else if (action.error.status === 400) {
            // Invalid display name or handle
            this.showErrorMsg('Invalid request.');
            return false;
          } else {
            // Maybe the user profile manager service is down.
            this.showErrorMsg('Something went wrong. Please try again later.');
            return false;
          }
        }
      })
    );
  }

  showErrorMsg(msg: string) {
    console.log(msg);
  }
}
