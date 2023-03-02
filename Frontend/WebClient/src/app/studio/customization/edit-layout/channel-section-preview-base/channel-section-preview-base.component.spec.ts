import {ComponentFixture, TestBed} from '@angular/core/testing';

import {ChannelSectionPreviewBaseComponent} from './channel-section-preview-base.component';

describe('ChannelSectionPreviewBaseComponent', () => {
  let component: ChannelSectionPreviewBaseComponent;
  let fixture: ComponentFixture<ChannelSectionPreviewBaseComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChannelSectionPreviewBaseComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChannelSectionPreviewBaseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
