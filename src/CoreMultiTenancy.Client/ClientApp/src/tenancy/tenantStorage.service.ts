import { Injectable } from "@angular/core";

@Injectable({
    providedIn: 'root'
})

export class TenantStorageService {
    private _tenantId: string = '';
    public isTenantSet: boolean = this. _tenantId.length > 0;

    get tenantId(): string {
        return this._tenantId;
    }

    set tenantId(id: string) {
        this._tenantId = id;
    }
}