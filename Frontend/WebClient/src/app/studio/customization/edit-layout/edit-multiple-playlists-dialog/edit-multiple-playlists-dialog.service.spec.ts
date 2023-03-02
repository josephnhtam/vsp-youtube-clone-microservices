import {TestBed} from '@angular/core/testing';

import {EditMultiplePlaylistsDialogService} from './edit-multiple-playlists-dialog.service';

describe('EditMultiplePlaylistsDialogService', () => {
  let service: EditMultiplePlaylistsDialogService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EditMultiplePlaylistsDialogService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
