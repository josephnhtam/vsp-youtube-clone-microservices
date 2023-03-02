import { AuthService } from '../services/auth.service';
import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  Resolve,
  RouterStateSnapshot,
  UrlTree,
} from '@angular/router';
import { map, Observable } from 'rxjs';

@Injectable()
export class AuthInfoResolver implements CanActivate, Resolve<boolean> {
  constructor(private authService: AuthService) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ):
    | boolean
    | UrlTree
    | Observable<boolean | UrlTree>
    | Promise<boolean | UrlTree> {
    return this.restoreAuthInfo().pipe(map((_) => true));
  }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    return this.restoreAuthInfo();
  }

  restoreAuthInfo() {
    console.log('Restoring Auth Info');

    return this.authService.isAuthenticated$.pipe(
      map((isAuthenticated) => {
        if (!isAuthenticated) {
          return this.authService.restoreAuthInfo();
        }

        return isAuthenticated;
      })
    );
  }
}
