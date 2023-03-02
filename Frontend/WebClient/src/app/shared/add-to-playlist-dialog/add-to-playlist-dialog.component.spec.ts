import {ComponentFixture, TestBed} from '@angular/core/testing';

import {AddToPlaylistDialogComponent} from './add-to-playlist-dialog.component';

describe('AddToPlaylistDialogComponent', () => {
  let component: AddToPlaylistDialogComponent;
  let fixture: ComponentFixture<AddToPlaylistDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddToPlaylistDialogComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddToPlaylistDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
