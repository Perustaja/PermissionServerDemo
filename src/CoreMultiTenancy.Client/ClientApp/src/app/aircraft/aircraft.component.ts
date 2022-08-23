import { Component, Inject } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { TenantStorageService } from '../../tenancy/tenantStorage.service';

@Component({
    selector: 'app-aircraft-component',
    templateUrl: './aircraft.component.html'
})

export class AircraftComponent {
    public aircraft: Aircraft[] = [];
    constructor(private http: HttpClient, 
    private tenantStorage: TenantStorageService,
    @Inject('API_BASE_URL') apiUrl: string) {
        this.http.get<Aircraft[]>(apiUrl + `organizations/${tenantStorage.tenantId}/aircraft`).subscribe({
            next: (res) => this.aircraft = res, // ideally logic for a tenantless, new user
            error: (e) => console.log(e)
          });
    }
}

export interface Aircraft {
    RegNumber: string,
    ThumbnailUri: string,
    IsGrounded: boolean
}