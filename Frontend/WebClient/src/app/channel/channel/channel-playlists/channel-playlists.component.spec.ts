import {ComponentFixture, TestBed} from '@angular/core/testing';

import {ChannelPlaylistsComponent} from './channel-playlists.component';

describe('ChannelPlaylistsComponent', () => {
  let component: ChannelPlaylistsComponent;
  let fixture: ComponentFixture<ChannelPlaylistsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChannelPlaylistsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChannelPlaylistsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
