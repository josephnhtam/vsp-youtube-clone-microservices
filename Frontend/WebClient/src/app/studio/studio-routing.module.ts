import {StudioComponent} from './studio.component';
import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {VideosManagementComponent} from './videos-management/videos-management.component';
import {StudioHubClientConnectorService} from './services/studio-hub-client-connector.service';
import {CustomizationComponent} from './customization/customization.component';
import {UserDataResolver} from './customization/user-data-resolver.service';

const routes: Routes = [
  {
    path: '',
    component: StudioComponent,
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'videos',
      },
      {
        path: 'videos',
        component: VideosManagementComponent,
      },
      {
        path: 'editing',
        component: CustomizationComponent,
        resolve: {
          detailedUserProfile: UserDataResolver,
        },
      },
    ],
    canActivate: [StudioHubClientConnectorService],
    canDeactivate: [StudioHubClientConnectorService],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class StudioRoutingModule {}
