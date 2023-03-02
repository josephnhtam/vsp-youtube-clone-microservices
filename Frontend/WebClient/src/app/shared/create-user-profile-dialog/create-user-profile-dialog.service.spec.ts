import {TestBed} from '@angular/core/testing';

import {CreateUserProfileDialogService} from './create-user-profile-dialog.service';

describe('CreateUserProfileDialogService', () => {
  let service: CreateUserProfileDialogService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CreateUserProfileDialogService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
