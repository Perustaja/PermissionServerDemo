import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { TenantManagerService } from './tenantManager.service';

@Injectable({
  providedIn: 'root'
})
export class TenantGuard implements CanActivate {
  constructor(private tenantManager: TenantManagerService, private router: Router) {
  }
  canActivate(
    _next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    return this.tenantManager.isTenantSet$
      .pipe(tap(isAuthenticated => this.handlePortalRedirection(isAuthenticated, state)));
  }

  private handlePortalRedirection(isAuthenticated: boolean, state: RouterStateSnapshot) {
    if (!isAuthenticated)
      this.router.navigate(['/portal'])
  }
}
