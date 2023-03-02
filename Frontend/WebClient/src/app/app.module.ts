import { RefreshTokenInterceptor } from './auth/http-interceptors/refresh-token-interceptor.service';
import { SharedModule } from './shared/shared.module';
import { CoreModule } from './core/core.module';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SigninCallbackComponent } from './auth/signin-callback/signin-callback.component';
import { SignoutCallbackComponent } from './auth/signout-callback/signout-callback.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';
import { environment } from '../environments/environment';
import { AuthInterceptor } from './auth/http-interceptors/auth-interceptor.service';
import { FailedResponseInterceptor } from './shared/http-interceptors/failed-response-interceptor.service';
import {AuthInfoResolver} from "./auth/guards/auth-info-resolver.service";

@NgModule({
  declarations: [
    AppComponent,
    SigninCallbackComponent,
    SignoutCallbackComponent,
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    AppRoutingModule,
    CoreModule,
    SharedModule,
    StoreModule.forRoot({}),
    EffectsModule.forRoot(),
    StoreDevtoolsModule.instrument({
      maxAge: 25,
      logOnly: environment.production,
    }),
  ],
  providers: [
    AuthInfoResolver,
    {
      provide: HTTP_INTERCEPTORS,
      multi: true,
      useClass: RefreshTokenInterceptor,
    },
    {
      provide: HTTP_INTERCEPTORS,
      multi: true,
      useClass: FailedResponseInterceptor,
    },
    { provide: HTTP_INTERCEPTORS, multi: true, useClass: AuthInterceptor },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
