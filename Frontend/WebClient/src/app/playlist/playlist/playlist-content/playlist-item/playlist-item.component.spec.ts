import {ComponentFixture, TestBed} from '@angular/core/testing';

import {PlaylistItemComponent} from './playlist-item.component';

describe('PlaylistItemComponent', () => {
  let component: PlaylistItemComponent;
  let fixture: ComponentFixture<PlaylistItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PlaylistItemComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PlaylistItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
