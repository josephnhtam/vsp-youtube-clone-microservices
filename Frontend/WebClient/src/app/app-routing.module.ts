import { SignoutCallbackComponent } from './auth/signout-callback/signout-callback.component';
import { SigninCallbackComponent } from './auth/signin-callback/signin-callback.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './auth/guards/auth-guard.service';
import { AuthInfoResolver } from './auth/guards/auth-info-resolver.service';
import { UserProfileResolver } from './shared/guards/user-profile-resolver.service';
import { UserReadyGuard } from './shared/guards/user-ready-guard.service';

const routes: Routes = [
  {
    path: '',
    children: [
      {
        path: '',
        canActivate: [AuthInfoResolver, UserProfileResolver],
        children: [
          {
            path: '',
            loadChildren: () =>
              import('./main/main.module').then((m) => m.MainModule),
          },
          {
            path: 'studio',
            canActivate: [AuthGuard, UserReadyGuard],
            loadChildren: () =>
              import('./studio/studio.module').then((m) => m.StudioModule),
          },
        ],
      },
      {
        path: 'signin-callback',
        component: SigninCallbackComponent,
      },
      {
        path: 'signout-callback',
        component: SignoutCallbackComponent,
      },
      {
        path: '**',
        redirectTo: '/',
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
