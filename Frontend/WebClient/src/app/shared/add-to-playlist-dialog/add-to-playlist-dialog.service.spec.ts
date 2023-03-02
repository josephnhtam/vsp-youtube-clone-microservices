import {TestBed} from '@angular/core/testing';

import {AddToPlaylistDialogService} from './add-to-playlist-dialog.service';

describe('AddToPlaylistDialogService', () => {
  let service: AddToPlaylistDialogService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AddToPlaylistDialogService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
