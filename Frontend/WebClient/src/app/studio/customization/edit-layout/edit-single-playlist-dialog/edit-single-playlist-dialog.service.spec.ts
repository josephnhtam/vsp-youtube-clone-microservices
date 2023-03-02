import {TestBed} from '@angular/core/testing';

import {EditSinglePlaylistDialogService} from './edit-single-playlist-dialog.service';

describe('EditSinglePlaylistDialogService', () => {
  let service: EditSinglePlaylistDialogService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EditSinglePlaylistDialogService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
