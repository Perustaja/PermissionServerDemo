import { Component, Inject, OnInit } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { TenantManagerService } from '../../tenancy/tenantManager.service';
import { AuthorizeService } from "src/api-authorization/authorize.service";
import { map, Observable } from "rxjs";

@Component({
    selector: 'demo-controls-component',
    templateUrl: './demo-controls.component.html',
    styleUrls: ['./demo-controls.component.css'],
    providers: [TenantManagerService]
})
export class DemoControlsComponent implements OnInit {
    aircraft: Aircraft[] = [];
    apiUrl: string;
    apiBaseUrl: string;
    token?: Observable<string | null>;
    subId?: Observable<string | null | undefined>;
    tenantId: string | null;
    permissions: string[];

    constructor(private http: HttpClient,
        private authorizeSvc: AuthorizeService,
        private tenantManager: TenantManagerService,
        @Inject('API_URL') apiUrl: string,
        @Inject('API_BASE_URL') apiBaseUrl: string) {
        this.apiUrl = apiUrl;
        this.apiBaseUrl = apiBaseUrl;
        this.token = authorizeSvc.getAccessToken().pipe(map(t => t));
        this.subId = authorizeSvc.getUser().pipe(map(u => u && u.sub));
        this.permissions = tenantManager.permissions;
        this.tenantId = tenantManager.tenantId;
    }

    ngOnInit() {
    }
}
export interface Aircraft {
    regNumber: string,
    model: string,
    thumbnailUri: string,
    isGrounded: boolean
}