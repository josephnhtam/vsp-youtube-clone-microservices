import {ComponentFixture, TestBed} from '@angular/core/testing';

import {SinglePlaylistSectionRowComponent} from './single-playlist-section-row.component';

describe('SinglePlaylistSectionRowComponent', () => {
  let component: SinglePlaylistSectionRowComponent;
  let fixture: ComponentFixture<SinglePlaylistSectionRowComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SinglePlaylistSectionRowComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SinglePlaylistSectionRowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
