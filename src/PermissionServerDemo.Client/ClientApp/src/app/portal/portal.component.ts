import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { faUserCircle } from '@fortawesome/free-solid-svg-icons'
import { map, takeUntil } from 'rxjs/operators';
import { TenantManagerService } from '../../tenancy/tenantManager.service';
import { AuthorizeService } from '../../api-authorization/authorize.service';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-portal-component',
  templateUrl: './portal.component.html',
  styleUrls: ['./portal.component.css'],
  providers: [TenantManagerService]
})
export class PortalComponent implements OnInit, OnDestroy {
  private ngUnsub = new Subject<void>();
  faUserCircle = faUserCircle;

  idpApiUrl: string;
  idpBaseUrl: string;
  userOrganizations: UserOrganization[] = [];

  constructor(private http: HttpClient,
    private authorizeSvc: AuthorizeService,
    private tenantManager: TenantManagerService,
    private router: Router,
    @Inject('IDP_API_URL') idpApiUrl: string,
    @Inject('IDP_BASE_URL') idpBaseUrl: string) {
    this.idpApiUrl = idpApiUrl;
    this.idpBaseUrl = idpBaseUrl;
  }

  ngOnInit() {
    this.authorizeSvc.getUser()
      .pipe(
        takeUntil(this.ngUnsub),
        map(u => u && u.sub)
      )
      .subscribe(userId =>
        this.http.get<UserOrganization[]>(`${this.idpApiUrl}/users/${userId}/organizations`).subscribe({
          next: (res) => this.userOrganizations = res,
          error: (e) => console.log(e)
        })
      )
  }

  selectTenant(id: string) {
    this.tenantManager.updateTenantSelection(id).then((res) => {
      this.router.navigate(['/aircraft'])
    })
  }

  ngOnDestroy() {
    this.ngUnsub.next();
    this.ngUnsub.complete();
  }
}

interface UserOrganization {
  awaitingApproval: boolean,
  blacklisted: boolean;
  dateSubmitted: Date
  organization: Organization
}

interface Organization {
  id: string,
  logoUri: string,
  title: string,
  isActive: boolean
  organization: Organization
}