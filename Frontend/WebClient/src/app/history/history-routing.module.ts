import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from '../auth/guards/auth-guard.service';
import { UserReadyGuard } from '../shared/guards/user-ready-guard.service';
import { HistoryGuard } from './history/history-guard.service';
import { HistoryComponent } from './history/history.component';

const routes: Routes = [
  {
    path: '',
    component: HistoryComponent,
    canActivate: [AuthGuard, UserReadyGuard, HistoryGuard],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class HistoryRoutingModule {}
