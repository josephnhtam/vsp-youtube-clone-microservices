import {ComponentFixture, TestBed} from '@angular/core/testing';

import {SignoutCallbackComponent} from './signout-callback.component';

describe('SignoutCallbackComponent', () => {
  let component: SignoutCallbackComponent;
  let fixture: ComponentFixture<SignoutCallbackComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SignoutCallbackComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SignoutCallbackComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
