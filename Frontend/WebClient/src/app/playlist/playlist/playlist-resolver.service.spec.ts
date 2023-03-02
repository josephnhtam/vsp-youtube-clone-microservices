import {TestBed} from '@angular/core/testing';

import {PlaylistResolver} from './playlist-resolver.service';

describe('PlaylistResolverService', () => {
  let service: PlaylistResolver;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PlaylistResolver);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
