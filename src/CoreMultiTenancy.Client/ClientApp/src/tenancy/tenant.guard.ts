import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable } from 'rxjs';
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
        if (!this.tenantManager.isTenantSet)
            this.router.navigate(['/portal']);
        return true;
  }
}
