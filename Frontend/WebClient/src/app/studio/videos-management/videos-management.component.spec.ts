import {ComponentFixture, TestBed} from '@angular/core/testing';

import {VideosManagementComponent} from './videos-management.component';

describe('VideosManagementComponent', () => {
  let component: VideosManagementComponent;
  let fixture: ComponentFixture<VideosManagementComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ VideosManagementComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(VideosManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
