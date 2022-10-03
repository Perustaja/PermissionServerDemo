import { Component, Inject, OnInit } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { map, Observable } from "rxjs";
import { TenantManagerService } from '../../tenancy/tenantManager.service';
import { AuthorizeService } from "src/api-authorization/authorize.service";

@Component({
    selector: 'demo-controls-component',
    templateUrl: './demo-controls.component.html',
    styleUrls: ['./demo-controls.component.css'],
    providers: [TenantManagerService]
})
export class DemoControlsComponent implements OnInit {
    aircraft: Aircraft[] = [];
    apiUrl: string;
    idpApiUrl: string;
    token?: Observable<string | null>;
    subId?: Observable<string | null | undefined>;
    tenantId: string | null;
    permissions: string[];

    constructor(private http: HttpClient,
        private authorizeSvc: AuthorizeService,
        private tenantManager: TenantManagerService,
        @Inject('API_URL') apiUrl: string,
        @Inject('IDP_API_URL') idpApiUrl: string) {
        this.apiUrl = apiUrl;
        this.idpApiUrl = idpApiUrl;
        this.token = authorizeSvc.getAccessToken().pipe(map(t => t));
        this.subId = authorizeSvc.getUser().pipe(map(u => u && u.sub));
        this.permissions = tenantManager.permissions;
        this.tenantId = tenantManager.tenantId;
    }

    ngOnInit() {
    }

    createDemoAircraft() {
        const ac = <Aircraft>({
            RegNumber: "N555YS",
            Model: "Cessna 172S",
            ThumbnailUri: "N555YS.jpg"
        });
        this.http.post<Aircraft>(this.apiUrl + `/organizations/${this.tenantManager.tenantId}/aircraft`, ac)
            .subscribe({
                next: (res) => alert(JSON.stringify(res)),
                error: (err) => {
                    if (err.status == 409)
                        alert("The demo aircraft has already been created. Navigate to the Aircraft page to view it.")
                }
            });
    }
}
export interface Aircraft {
    RegNumber: string,
    Model: string,
    ThumbnailUri: string
}