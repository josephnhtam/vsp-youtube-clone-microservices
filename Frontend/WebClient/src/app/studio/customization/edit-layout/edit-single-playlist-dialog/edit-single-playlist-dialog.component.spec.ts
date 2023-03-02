import {ComponentFixture, TestBed} from '@angular/core/testing';

import {EditSinglePlaylistDialogComponent} from './edit-single-playlist-dialog.component';

describe('EditSinglePlaylistDialogComponent', () => {
  let component: EditSinglePlaylistDialogComponent;
  let fixture: ComponentFixture<EditSinglePlaylistDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditSinglePlaylistDialogComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditSinglePlaylistDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
