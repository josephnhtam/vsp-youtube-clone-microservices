import {TestBed} from '@angular/core/testing';

import {ChannelSubscriptionService} from './channel-subscription.service';

describe('VideoSubscriptionService', () => {
  let service: ChannelSubscriptionService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ChannelSubscriptionService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
