import { Component, Inject, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TenantManagerService } from '../../tenancy/tenantManager.service';
import { combineLatestWith, Subject, takeUntil } from "rxjs";

@Component({
    selector: 'app-aircraft-component',
    templateUrl: './aircraft.component.html',
    styleUrls: ['./aircraft.component.css'],
    providers: [TenantManagerService]
})
export class AircraftComponent implements OnInit, OnDestroy {
    ngUnsub = new Subject<void>();
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
        this.tenantManager.tenantId$.pipe(
            takeUntil(this.ngUnsub),
        )
        .subscribe(tid => {
                this.http.get<Aircraft[]>(`${this.apiUrl}/organizations/${tid}/aircraft/`).subscribe({
                next: (res) => this.aircraft = res,
                error: (e) => console.log(e)
            });
        })
    }

    ngOnDestroy() {
        this.ngUnsub.next();
        this.ngUnsub.complete();
    }
}

export interface Aircraft {
    regNumber: string,
    model: string,
    thumbnailUri: string,
    isGrounded: boolean
}