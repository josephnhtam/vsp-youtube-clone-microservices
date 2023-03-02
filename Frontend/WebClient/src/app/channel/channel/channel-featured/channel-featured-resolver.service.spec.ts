import {TestBed} from '@angular/core/testing';

import {ChannelFeaturedResolver} from './channel-featured-resolver.service';

describe('ChannelFeaturedResolverService', () => {
  let service: ChannelFeaturedResolver;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ChannelFeaturedResolver);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
