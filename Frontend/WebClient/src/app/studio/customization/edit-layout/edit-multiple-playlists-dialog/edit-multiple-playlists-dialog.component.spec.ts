import {ComponentFixture, TestBed} from '@angular/core/testing';

import {EditMultiplePlaylistsDialogComponent} from './edit-multiple-playlists-dialog.component';

describe('EditMultiplePlaylistsDialogComponent', () => {
  let component: EditMultiplePlaylistsDialogComponent;
  let fixture: ComponentFixture<EditMultiplePlaylistsDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditMultiplePlaylistsDialogComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditMultiplePlaylistsDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
