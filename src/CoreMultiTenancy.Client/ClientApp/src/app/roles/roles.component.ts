import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { TenantManagerService } from '../../tenancy/tenantManager.service';

@Component({
    selector: 'app-roles',
    templateUrl: './roles.component.html',
    styleUrls: ['./roles.component.css'],
    encapsulation: ViewEncapsulation.None,
})
export class RolesComponent implements OnInit {
    idpApiUrl: string;
    userRoles: Role[] = [];
    globalRoles: Role[] = [];
    permCats: PermissionCategory[] = [];

    constructor(private http: HttpClient,
        private tenantManager: TenantManagerService,
        @Inject('IDP_API_URL') idpApiUrl: string) {
        this.idpApiUrl = idpApiUrl;
    }

    ngOnInit() {
        this.http.get<PermissionCategory[]>(this.idpApiUrl + `/permissionCategories`)
            .subscribe({
                next: (res) => this.permCats = res,
                error: (e) => console.log(e)
            })

        this.http.get<Role[]>(this.idpApiUrl + `/organizations/${this.tenantManager.tenantId}/roles`)
            .subscribe({
                next: (res) => res.forEach(r => r.isGlobal ? this.globalRoles.push(r) : this.userRoles.push(r)),
                error: (e) => console.log(e)
            })
    }

    roleHasPermission(role: Role, perm: Permission): boolean {
        return role.permissions.some(p => p.id == perm.id);
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
    description: string
}

export interface PermissionCategory {
    id: number,
    name: string,
    permissions: Permission[]
}