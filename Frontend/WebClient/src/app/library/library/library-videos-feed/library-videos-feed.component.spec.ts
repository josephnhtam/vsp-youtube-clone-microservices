import {ComponentFixture, TestBed} from '@angular/core/testing';

import {LibraryVideosFeedComponent} from './library-videos-feed.component';

describe('LibraryVideosFeedComponent', () => {
  let component: LibraryVideosFeedComponent;
  let fixture: ComponentFixture<LibraryVideosFeedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LibraryVideosFeedComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LibraryVideosFeedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
