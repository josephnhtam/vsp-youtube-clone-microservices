import {ComponentFixture, TestBed} from '@angular/core/testing';

import {CreatedPlaylistsSectionRowComponent} from './created-playlists-section-row.component';

describe('CreatedPlaylistsSectionRowComponent', () => {
  let component: CreatedPlaylistsSectionRowComponent;
  let fixture: ComponentFixture<CreatedPlaylistsSectionRowComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreatedPlaylistsSectionRowComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreatedPlaylistsSectionRowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
