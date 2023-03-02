import {ComponentFixture, TestBed} from '@angular/core/testing';

import {ChannelPlaylistsViewComponent} from './channel-playlists-view.component';

describe('ChannelPlaylistsViewComponent', () => {
  let component: ChannelPlaylistsViewComponent;
  let fixture: ComponentFixture<ChannelPlaylistsViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChannelPlaylistsViewComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChannelPlaylistsViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
