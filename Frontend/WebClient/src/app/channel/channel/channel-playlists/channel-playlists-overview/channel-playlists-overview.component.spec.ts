import {ComponentFixture, TestBed} from '@angular/core/testing';

import {ChannelPlaylistsOverviewComponent} from './channel-playlists-overview.component';

describe('ChannelPlaylistsOverviewComponent', () => {
  let component: ChannelPlaylistsOverviewComponent;
  let fixture: ComponentFixture<ChannelPlaylistsOverviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChannelPlaylistsOverviewComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChannelPlaylistsOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
