import {ComponentFixture, TestBed} from '@angular/core/testing';

import {ChannelPlaylistsViewSelectorComponent} from './channel-playlists-view-selector.component';

describe('ChannelPlaylistsViewSelectorComponent', () => {
  let component: ChannelPlaylistsViewSelectorComponent;
  let fixture: ComponentFixture<ChannelPlaylistsViewSelectorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChannelPlaylistsViewSelectorComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChannelPlaylistsViewSelectorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
