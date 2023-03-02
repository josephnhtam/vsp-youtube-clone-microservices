import {ComponentFixture, TestBed} from '@angular/core/testing';

import {PlaylistsBrowserComponent} from './playlists-browser.component';

describe('PlaylistsBrowserComponent', () => {
  let component: PlaylistsBrowserComponent;
  let fixture: ComponentFixture<PlaylistsBrowserComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PlaylistsBrowserComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PlaylistsBrowserComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
