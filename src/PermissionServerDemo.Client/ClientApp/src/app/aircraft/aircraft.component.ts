import { Component, Inject, OnInit } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { TenantManagerService } from '../../tenancy/tenantManager.service';

@Component({
    selector: 'app-aircraft-component',
    templateUrl: './aircraft.component.html',
    styleUrls: ['./aircraft.component.css'],
    providers: [TenantManagerService]
})
export class AircraftComponent implements OnInit {
    aircraft: Aircraft[] = [];
    apiUrl: string;
    apiBaseUrl: string;

    constructor(private http: HttpClient,
        private tenantManager: TenantManagerService,
        @Inject('API_URL') apiUrl: string,
        @Inject('API_BASE_URL') apiBaseUrl: string) {
        this.apiUrl = apiUrl;
        this.apiBaseUrl = apiBaseUrl;
    }

    ngOnInit() {
        this.tenantManager.tenantId$.subscribe(tid => {
            this.http.get<Aircraft[]>(`${this.apiUrl}/organizations/${tid}/aircraft`).subscribe({
                next: (res) => this.aircraft = res,
                error: (e) => console.log(e)
            });
        })
    }
}

export interface Aircraft {
    regNumber: string,
    model: string,
    thumbnailUri: string,
    isGrounded: boolean
}