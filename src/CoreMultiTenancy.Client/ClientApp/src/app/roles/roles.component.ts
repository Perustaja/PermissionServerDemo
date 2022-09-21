import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { TenantManagerService } from '../../tenancy/tenantManager.service';

@Component({
    selector: 'app-roles',
    templateUrl: './roles.component.html',
    styleUrls: ['./roles.component.css']
})
export class RolesComponent implements OnInit {
    idpApiUrl: string;
    allPermissions: Permission[] = [];
    roles: Role[] = [];

    constructor(private http: HttpClient,
        private tenantManager: TenantManagerService,
        @Inject('IDP_API_URL') idpApiUrl: string) {
        this.idpApiUrl = idpApiUrl;
    }

    ngOnInit() {
        this.http.get<Permission[]>(this.idpApiUrl + `/permissions`)
        .subscribe({
            next: (res) => this.allPermissions = res,
            error: (e) => console.log(e)
        })

        this.http.get<Role[]>(this.idpApiUrl + `/organizations/${this.tenantManager.tenantId}/roles`)
        .subscribe({
            next: (res) => this.roles = res,
            error: (e) => console.log(e)
        })
    }
}

export interface Role {
    name: string,
    description: string,
    isGlobal: boolean,
    permissions: Permission[]
}

export interface Permission {
    id: number,
    name: string,
    description: string,
    category: string
}