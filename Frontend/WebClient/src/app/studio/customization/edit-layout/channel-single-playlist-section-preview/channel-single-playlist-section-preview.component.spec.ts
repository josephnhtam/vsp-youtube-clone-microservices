import {ComponentFixture, TestBed} from '@angular/core/testing';

import {ChannelSinglePlaylistSectionPreviewComponent} from './channel-single-playlist-section-preview.component';

describe('ChannelSinglePlaylistSectionPreviewComponent', () => {
  let component: ChannelSinglePlaylistSectionPreviewComponent;
  let fixture: ComponentFixture<ChannelSinglePlaylistSectionPreviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChannelSinglePlaylistSectionPreviewComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChannelSinglePlaylistSectionPreviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
