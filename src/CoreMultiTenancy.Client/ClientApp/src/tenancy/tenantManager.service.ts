import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { User } from 'oidc-client';
import { map } from "rxjs";
import { AuthorizeService } from "src/api-authorization/authorize.service";

@Injectable({
    providedIn: 'root'
})

export class TenantManagerService {
    private idpApiUrl: string;

    constructor(private http: HttpClient,
        private authorizeSvc: AuthorizeService,
        @Inject('IDP_API_URL') idpApiUrl: string) {
        this.idpApiUrl = idpApiUrl;
    }

    get tenantId(): string | null {
        return localStorage.getItem('tenantId');
    }

    isTenantSet(): boolean {
        return (localStorage.getItem('tenantId')?.length ?? 0) > 0;
    }

    updateTenantSelection(tenantId: string): void {
        localStorage.setItem('tenantId', tenantId);
        this.setPermissionsForTenant(tenantId);
    }

    setPermissionsForTenant(tenantId: string): void {
        this.authorizeSvc.getUser()
            .pipe(map(u => u && u.sub))
            .subscribe(userId =>
                this.http.get<string[]>(this.idpApiUrl + `/users/${userId}/organizations/${tenantId}/quickpermissions`)
                    .subscribe({
                        next: (res) => {
                            console.log(`Permissions updated: ${res}`);
                            localStorage.setItem('permissions', JSON.stringify(res));
                        },
                        error: (e) => console.log(e)
                    })
            );
    }

    getCurrentPermissions(): string[] {
        const perms = localStorage.getItem('permissions');
        if (perms === null)
            throw new Error("Tenant marked as selected but empty permissions in storage.")
        else
            return JSON.parse(perms);
    }
}