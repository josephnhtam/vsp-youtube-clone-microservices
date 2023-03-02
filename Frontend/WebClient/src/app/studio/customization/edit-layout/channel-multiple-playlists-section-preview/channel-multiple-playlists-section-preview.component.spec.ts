import {ComponentFixture, TestBed} from '@angular/core/testing';

import {ChannelMultiplePlaylistsSectionPreviewComponent} from './channel-multiple-playlists-section-preview.component';

describe('ChannelMultiplePlaylistsSectionPreviewComponent', () => {
  let component: ChannelMultiplePlaylistsSectionPreviewComponent;
  let fixture: ComponentFixture<ChannelMultiplePlaylistsSectionPreviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChannelMultiplePlaylistsSectionPreviewComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChannelMultiplePlaylistsSectionPreviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
