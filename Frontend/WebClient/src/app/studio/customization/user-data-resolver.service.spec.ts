import {TestBed} from '@angular/core/testing';

import {UserDataResolver} from './user-data-resolver.service';

describe('DetailedUserProfileResolverService', () => {
  let service: UserDataResolver;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UserDataResolver);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
