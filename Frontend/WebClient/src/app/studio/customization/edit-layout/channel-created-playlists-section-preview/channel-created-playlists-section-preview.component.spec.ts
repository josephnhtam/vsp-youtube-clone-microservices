import {ComponentFixture, TestBed} from '@angular/core/testing';

import {ChannelCreatedPlaylistsSectionPreviewComponent} from './channel-created-playlists-section-preview.component';

describe('ChannelCreatedPlaylistsSectionPreviewComponent', () => {
  let component: ChannelCreatedPlaylistsSectionPreviewComponent;
  let fixture: ComponentFixture<ChannelCreatedPlaylistsSectionPreviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChannelCreatedPlaylistsSectionPreviewComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChannelCreatedPlaylistsSectionPreviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
