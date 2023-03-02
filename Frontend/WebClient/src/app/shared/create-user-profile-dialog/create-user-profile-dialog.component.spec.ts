import {ComponentFixture, TestBed} from '@angular/core/testing';

import {CreateUserProfileDialogComponent} from './create-user-profile-dialog.component';

describe('CreateUserProfileDialogComponent', () => {
  let component: CreateUserProfileDialogComponent;
  let fixture: ComponentFixture<CreateUserProfileDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateUserProfileDialogComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateUserProfileDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
