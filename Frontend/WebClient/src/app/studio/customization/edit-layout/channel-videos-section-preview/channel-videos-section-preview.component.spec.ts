import {ComponentFixture, TestBed} from '@angular/core/testing';

import {ChannelVideosSectionPreviewComponent} from './channel-videos-section-preview.component';

describe('ChannelVideosSectionPreviewComponent', () => {
  let component: ChannelVideosSectionPreviewComponent;
  let fixture: ComponentFixture<ChannelVideosSectionPreviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChannelVideosSectionPreviewComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChannelVideosSectionPreviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
