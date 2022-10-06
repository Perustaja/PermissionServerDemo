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
    private tenantIdSubject = new BehaviorSubject<string | null>(localStorage.getItem('tenantId'));
    private permissionsSubject = new BehaviorSubject<string[]>(JSON.parse(localStorage.getItem('permissions') ?? ''));
    isTenantSet$ = this.isTenantSetSubject.asObservable();
    tenantId$ = this.tenantIdSubject.asObservable();
    permissions$ = this.permissionsSubject.asObservable();

    constructor(private http: HttpClient,
        private authorizeSvc: AuthorizeService,
        @Inject('IDP_API_URL') idpApiUrl: string) {
        this.idpApiUrl = idpApiUrl;
    }

    updateTenantSelection(tenantId: string) {
        localStorage.setItem('tenantId', tenantId);
        this.tenantIdSubject.next(tenantId);
        this.isTenantSetSubject.next(true);
        this.setPermissionsForTenant(tenantId);
    }
    
    // this is a hacky workaround for handling the updates. A more robust updating 
    // system would be needed but is kind of out of scope for this project
    updatePermissions() {
        this.setPermissionsForTenant(localStorage.getItem('tenantId') ?? '')
    }

    private setPermissionsForTenant(tenantId: string) {
        this.authorizeSvc.getUser()
            .pipe(map(u => u && u.sub))
            .subscribe(userId =>
                this.http.get<string[]>(this.idpApiUrl + `/users/${userId}/organizations/${tenantId}/permissions`)
                    .subscribe({
                        next: (res) => {
                            console.log(`User permissions updated: ${res}`);
                            localStorage.setItem('permissions', JSON.stringify(res));
                            this.permissionsSubject.next(res)
                        },
                        error: (e) => console.log(e)
                    })
            );
    }
}