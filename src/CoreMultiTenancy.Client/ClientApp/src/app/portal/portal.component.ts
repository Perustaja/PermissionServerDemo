import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { Router } from '@angular/router';
import { faUserCircle } from '@fortawesome/free-solid-svg-icons'
import { TenantStorageService } from '../../tenancy/tenantStorage.service';

@Component({
  selector: 'app-portal-component',
  templateUrl: './portal.component.html',
  styleUrls: ['./portal.component.css'],
  providers: [TenantStorageService]
})
export class PortalComponent {
  faUserCircle = faUserCircle;
  idpUrl: string;
  public userOrganizations: UserOrganization[] = [];

  constructor(private http: HttpClient, 
    private tenantStorage: TenantStorageService, 
    private router: Router,
    @Inject('IDP_BASE_URL') idpUrl: string) {
    this.idpUrl = idpUrl;
    this.http.get<UserOrganization[]>(idpUrl + `users/organizations`).subscribe({
      next: (res) => this.userOrganizations = res,
      error: (e) => console.log(e)
    });
  }
  
  selectTenant(id: string) {
    this.tenantStorage.tenantId = id;
    // update permissions cache for controls
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
