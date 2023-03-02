import {TestBed} from '@angular/core/testing';

import {UserProfileResolver} from './user-profile-resolver.service';

describe('UserProfileResolverService', () => {
  let service: UserProfileResolver;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UserProfileResolver);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
