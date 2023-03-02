import {TestBed} from '@angular/core/testing';

import {ChannelPlaylistsResolver} from './channel-playlists-resolver.service';

describe('ChannelPlaylistsResolverService', () => {
  let service: ChannelPlaylistsResolver;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ChannelPlaylistsResolver);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
