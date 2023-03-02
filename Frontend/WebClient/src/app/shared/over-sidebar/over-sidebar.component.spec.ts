import {ComponentFixture, TestBed} from '@angular/core/testing';

import {OverSidebarComponent} from './over-sidebar.component';

describe('OverSidebarComponent', () => {
  let component: OverSidebarComponent;
  let fixture: ComponentFixture<OverSidebarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ OverSidebarComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OverSidebarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
