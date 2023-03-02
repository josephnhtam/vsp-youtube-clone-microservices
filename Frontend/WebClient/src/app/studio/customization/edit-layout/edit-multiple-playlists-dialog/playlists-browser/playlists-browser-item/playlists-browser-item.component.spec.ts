import {ComponentFixture, TestBed} from '@angular/core/testing';

import {PlaylistsBrowserItemComponent} from './playlists-browser-item.component';

describe('PlaylistsBrowserItemComponent', () => {
  let component: PlaylistsBrowserItemComponent;
  let fixture: ComponentFixture<PlaylistsBrowserItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PlaylistsBrowserItemComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PlaylistsBrowserItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
