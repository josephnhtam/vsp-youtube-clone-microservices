import {ComponentFixture, TestBed} from '@angular/core/testing';

import {SigninCallbackComponent} from './signin-callback.component';

describe('SigninCallbackComponent', () => {
  let component: SigninCallbackComponent;
  let fixture: ComponentFixture<SigninCallbackComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SigninCallbackComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SigninCallbackComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
