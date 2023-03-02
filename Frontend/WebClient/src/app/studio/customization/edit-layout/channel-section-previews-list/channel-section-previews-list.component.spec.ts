import {ComponentFixture, TestBed} from '@angular/core/testing';

import {ChannelSectionPreviewsListComponent} from './channel-section-previews-list.component';

describe('ChannelSectionPreviewsListComponent', () => {
  let component: ChannelSectionPreviewsListComponent;
  let fixture: ComponentFixture<ChannelSectionPreviewsListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChannelSectionPreviewsListComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChannelSectionPreviewsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
