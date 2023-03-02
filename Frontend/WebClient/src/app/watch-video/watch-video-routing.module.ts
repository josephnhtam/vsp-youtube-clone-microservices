import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {WatchVideoComponent} from "./watch-video/watch-video.component";
import {VideoResolver} from "./watch-video/video-resolver.service";

const routes: Routes = [{
  path:":id",
  component: WatchVideoComponent,
  runGuardsAndResolvers: 'paramsOrQueryParamsChange',
  resolve: { video: VideoResolver },
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class WatchVideoRoutingModule { }
