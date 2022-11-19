import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from '@angular/core';
import { BehaviorSubject, map, Observable } from "rxjs";
import { AuthorizeService } from "src/api-authorization/authorize.service";

@Injectable({
    providedIn: 'root'
})

export class TenantManagerService {
    private idpApiUrl: string;
    private isTenantSetSubject = new BehaviorSubject<boolean>((localStorage.getItem('tenantId')?.length ?? 0) > 0);
    private isTenantShadowSubject = new BehaviorSubject<boolean>(Boolean(JSON.parse(localStorage.getItem('isTenantShadow') ?? 'false')));
    private tenantIdSubject = new BehaviorSubject<string | null>(localStorage.getItem('tenantId'));
    private permissionsSubject = new BehaviorSubject<string[]>(JSON.parse(localStorage.getItem('permissions') ?? '[]'));
    private userId: string | null | undefined;
    isTenantSet$ = this.isTenantSetSubject.asObservable();
    isTenantShadow$ = this.isTenantShadowSubject.asObservable();
    tenantId$ = this.tenantIdSubject.asObservable();
    permissions$ = this.permissionsSubject.asObservable();

    constructor(private http: HttpClient,
        private authorizeSvc: AuthorizeService,
        @Inject('IDP_API_URL') idpApiUrl: string) {
        this.idpApiUrl = idpApiUrl;
        authorizeSvc.getUser()
            .pipe(map(u => u && u.sub))
            .subscribe(sub => this.userId = sub);
    }

    updateTenantSelection(tenantId: string): Promise<boolean> {
        localStorage.setItem('tenantId', tenantId);
        this.tenantIdSubject.next(tenantId);
        this.isTenantSetSubject.next(true);
        return this.setPermissionsForTenant(tenantId);
    }

    // this is a hacky workaround for handling the updates. A more robust updating 
    // system would be needed but is kind of out of scope for this project
    updatePermissions(): Promise<boolean> {
        return this.setPermissionsForTenant(localStorage.getItem('tenantId') ?? '');
    }

    private setPermissionsForTenant(tenantId: string): Promise<boolean> {
        return new Promise((resolve, reject) => {
            this.http.get<PermResult>(this.idpApiUrl + `/users/${this.userId}/organizations/${tenantId}/permissions`)
                .subscribe({
                    next: (res) => {
                        console.log(`User permissions updated: ${res}`);
                        localStorage.setItem('permissions', JSON.stringify(res.permissions));
                        localStorage.setItem('isTenantShadow', JSON.stringify(res.isTenantShadow));
                        this.permissionsSubject.next(res.permissions);
                        this.isTenantShadowSubject.next(res.isTenantShadow);
                        resolve(true);
                    },
                    error: (e) => {
                        console.log(e);
                        reject(false);
                    }
                })
        })
    }
}
export interface PermResult {
    permissions: string[],
    isTenantShadow: boolean,
}