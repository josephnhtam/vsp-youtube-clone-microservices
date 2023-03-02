import {ComponentFixture, TestBed} from '@angular/core/testing';

import {PlaylistPreviewComponent} from './playlist-preview.component';

describe('PlaylistPreviewComponent', () => {
  let component: PlaylistPreviewComponent;
  let fixture: ComponentFixture<PlaylistPreviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PlaylistPreviewComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PlaylistPreviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
