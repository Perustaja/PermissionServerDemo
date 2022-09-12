import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { faUserCircle } from '@fortawesome/free-solid-svg-icons'
import { map, tap } from 'rxjs/operators';
import { TenantStorageService } from '../../tenancy/tenantStorage.service';
import { AuthorizeService } from '../../api-authorization/authorize.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-portal-component',
  templateUrl: './portal.component.html',
  styleUrls: ['./portal.component.css'],
  providers: [TenantStorageService]
})
export class PortalComponent implements OnInit{
  faUserCircle = faUserCircle;
  idpApiUrl: string;
  idpBaseUrl: string;
  userOrganizations: UserOrganization[] = [];

  constructor(private http: HttpClient, 
    private authorizeSvc: AuthorizeService,
    private tenantStorage: TenantStorageService, 
    private router: Router,
    @Inject('IDP_API_URL') idpApiUrl: string,
    @Inject('IDP_BASE_URL') idpBaseUrl: string) {
    this.idpApiUrl = idpApiUrl;
    this.idpBaseUrl = idpBaseUrl;
  }

  ngOnInit() {
    this.authorizeSvc.getUser()
      .pipe(map(u => u && u.sub))
      .subscribe(userId => 
        this.http.get<UserOrganization[]>(this.idpApiUrl + `/users/${userId}/organizations`).subscribe({
          next: (res) => this.userOrganizations = res,
          error: (e) => console.log(e)})
      )
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
