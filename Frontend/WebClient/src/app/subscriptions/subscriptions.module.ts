import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SubscriptionsRoutingModule } from './subscriptions-routing.module';
import { SubscriptionsComponent } from './subscriptions/subscriptions.component';
import { SubscriptionsFeedComponent } from './subscriptions/subscriptions-feed/subscriptions-feed.component';
import { CoreModule } from '../core/core.module';
import { SharedModule } from '../shared/shared.module';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@NgModule({
  declarations: [SubscriptionsComponent, SubscriptionsFeedComponent],
  imports: [
    CommonModule,
    CoreModule,
    SharedModule,
    SubscriptionsRoutingModule,
    MatButtonModule,
    MatProgressSpinnerModule,
  ],
})
export class SubscriptionsModule {}
