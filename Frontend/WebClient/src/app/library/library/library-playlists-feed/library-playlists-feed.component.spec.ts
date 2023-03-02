import {ComponentFixture, TestBed} from '@angular/core/testing';

import {LibraryPlaylistsFeedComponent} from './library-playlists-feed.component';

describe('LibraryPlaylistsFeedComponent', () => {
  let component: LibraryPlaylistsFeedComponent;
  let fixture: ComponentFixture<LibraryPlaylistsFeedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LibraryPlaylistsFeedComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LibraryPlaylistsFeedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
