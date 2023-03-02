import { LibraryComponent } from './library/library.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from '../auth/guards/auth-guard.service';
import { UserReadyGuard } from '../shared/guards/user-ready-guard.service';

const routes: Routes = [
  {
    path: '',
    component: LibraryComponent,
    canActivate: [AuthGuard, UserReadyGuard],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class LibraryRoutingModule {}
