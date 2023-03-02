import {TestBed} from '@angular/core/testing';

import {EditVideoDialogService} from './edit-video-dialog.service';

describe('EditVideoDialogService', () => {
  let service: EditVideoDialogService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EditVideoDialogService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
