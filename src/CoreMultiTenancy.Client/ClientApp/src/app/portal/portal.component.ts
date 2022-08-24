import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { Router } from '@angular/router';
import { TenantStorageService } from '../../tenancy/tenantStorage.service';

@Component({
  selector: 'app-portal-component',
  templateUrl: './portal.component.html',
  providers: [TenantStorageService]
})
export class PortalComponent {
  public userOrganizations: UserOrganization[] = [];

  constructor(private http: HttpClient, 
    private tenantStorage: TenantStorageService, 
    private router: Router,
    @Inject('IDP_BASE_URL') idpUrl: string) {
    this.http.get<UserOrganization[]>(idpUrl + `users/organizations`).subscribe({
      next: (res) => this.userOrganizations = res, // ideally logic for a tenantless, new user
      error: (e) => console.log(e)
    });
  }
  
  selectTenant(id: string) {
    this.tenantStorage.tenantId = id;
    this.router.navigate(['/aircraft'])
  }
}

interface UserOrganization {
  Id: string,
  LogoUri: string,
  Title: string,
  IsActive: boolean
  Organization: Organization
}

interface Organization {
  AwaitingApproval: boolean,
  Blacklisted: boolean;
  DateSubmitted: Date
}
