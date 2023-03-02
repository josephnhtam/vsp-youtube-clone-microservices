import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PlaylistResolver } from './playlist/playlist-resolver.service';
import { PlaylistComponent } from './playlist/playlist.component';

const routes: Routes = [
  {
    path: '',
    component: PlaylistComponent,
    runGuardsAndResolvers: 'paramsOrQueryParamsChange',
    resolve: { playlistId: PlaylistResolver },
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class PlaylistRoutingModule {}
