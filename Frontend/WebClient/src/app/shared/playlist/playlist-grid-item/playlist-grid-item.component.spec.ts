import {ComponentFixture, TestBed} from '@angular/core/testing';

import {PlaylistGridItemComponent} from './playlist-grid-item.component';

describe('PlaylistGridItemComponent', () => {
  let component: PlaylistGridItemComponent;
  let fixture: ComponentFixture<PlaylistGridItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PlaylistGridItemComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PlaylistGridItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
