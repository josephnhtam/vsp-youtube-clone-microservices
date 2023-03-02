import {ComponentFixture, TestBed} from '@angular/core/testing';

import {PlaylistsInSectionComponent} from './playlists-in-section.component';

describe('PlaylistsInSectionComponent', () => {
  let component: PlaylistsInSectionComponent;
  let fixture: ComponentFixture<PlaylistsInSectionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PlaylistsInSectionComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PlaylistsInSectionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
