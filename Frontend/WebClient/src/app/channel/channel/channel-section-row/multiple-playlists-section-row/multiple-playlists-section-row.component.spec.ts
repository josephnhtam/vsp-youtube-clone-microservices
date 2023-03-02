import {ComponentFixture, TestBed} from '@angular/core/testing';

import {MultiplePlaylistsSectionRowComponent} from './multiple-playlists-section-row.component';

describe('MultiplePlaylistsSectionRowComponent', () => {
  let component: MultiplePlaylistsSectionRowComponent;
  let fixture: ComponentFixture<MultiplePlaylistsSectionRowComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MultiplePlaylistsSectionRowComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MultiplePlaylistsSectionRowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
