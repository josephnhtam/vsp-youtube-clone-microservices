import {TestBed} from '@angular/core/testing';

import {ChannelResolver} from './channel-resolver.service';

describe('ChannelResolverService', () => {
  let service: ChannelResolver;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ChannelResolver);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
