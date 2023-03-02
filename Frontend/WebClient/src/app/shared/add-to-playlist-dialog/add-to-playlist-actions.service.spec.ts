import {TestBed} from '@angular/core/testing';

import {AddToPlaylistActionService} from './add-to-playlist-actions.service';

describe('AddToPlaylistActionService', () => {
  let service: AddToPlaylistActionService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AddToPlaylistActionService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
