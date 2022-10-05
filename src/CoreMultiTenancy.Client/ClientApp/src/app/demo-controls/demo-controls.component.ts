import { Component, Inject, OnInit, TemplateRef } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { map, take } from "rxjs";
import { TenantManagerService } from '../../tenancy/tenantManager.service';
import { AuthorizeService } from "src/api-authorization/authorize.service";
import { Router } from "@angular/router";

@Component({
    selector: 'demo-controls-component',
    templateUrl: './demo-controls.component.html',
    styleUrls: ['./demo-controls.component.css'],
    providers: [TenantManagerService]
})
export class DemoControlsComponent implements OnInit {
    apiUrl: string;
    idpApiUrl: string;
    token?: string | null;
    subId?: string | null | undefined;
    tenantId: string | null;
    permissions: string[];
    permCats: PermissionCategory[] = [];
    model: RoleCreateDto;

    constructor(private http: HttpClient,
        private router: Router,
        private authorizeSvc: AuthorizeService,
        private tenantManager: TenantManagerService,
        private modalSvc: NgbModal,
        @Inject('API_URL') apiUrl: string,
        @Inject('IDP_API_URL') idpApiUrl: string) {
        this.apiUrl = apiUrl;
        this.idpApiUrl = idpApiUrl;

        this.permissions = tenantManager.permissions;
        this.tenantId = tenantManager.tenantId;
        this.model = <RoleCreateDto>({});
    }

    ngOnInit() {
        this.authorizeSvc.getAccessToken().pipe(take(1)).subscribe(t => this.token = t);
        this.authorizeSvc.getUser().pipe(map(u => u && u.sub), take(1)).subscribe(s => this.subId = s);
        this.http.get<PermissionCategory[]>(this.idpApiUrl + `/permissionCategories`)
            .subscribe({
                next: (res) => this.permCats = res,
                error: (e) => console.log(e)
            })
    }

    createDemoAircraft() {
        const ac = <AircraftCreateDto>({
            RegNumber: "N555YS",
            Model: "Cessna 172S",
            ThumbnailUri: "N555YS.jpg"
        });
        this.http.post<AircraftCreateDto>(`${this.apiUrl}/organizations/${this.tenantManager.tenantId}/aircraft`, ac)
            .subscribe({
                next: (res) => alert(JSON.stringify(res)),
                error: (err) => {
                    if (err.status == 409)
                        alert("The demo aircraft has already been created. Navigate to the Aircraft page to view it.")
                }
            });
    }

    openRoleModal(content: TemplateRef<any>) {
        this.model = <RoleCreateDto>({});
        this.modalSvc.open(content, { ariaLabelledBy: 'modal-basic-title' });
    }

    onSubmitRoleModal(test: any) {
        this.model.Permissions = [];
        document.querySelectorAll(".form-check-input:checked")?.forEach(c => this.model.Permissions.push(c.id));
        this.http.post<RoleCreateDto>(this.idpApiUrl + `/organizations/${this.tenantManager.tenantId}/roles`, this.model).subscribe({
            next: (res) => {
                alert(JSON.stringify(res));
                this.modalSvc.dismissAll();
            },
            error: (err) => alert(JSON.stringify(err))
        });
    }

    removeCreateAircraftRole() {
        // role id is hardcoded for demo
        const url = `${this.idpApiUrl}/organizations/${this.tenantManager.tenantId}/users/${this.subId}/roles/75a7570f-3ce5-48ba-9461-80283ed1d94d`
        this.http.delete(url).subscribe({
            next: (res) => {
                alert(JSON.stringify(res));
                this.tenantManager.permissionsAffected();
            },
            error: (err) => alert(JSON.stringify(err))
        });
    }

    revokeAccess() {
        const url = `${this.idpApiUrl}/organizations/${this.tenantManager.tenantId}/users/${this.subId}`
        this.http.delete(url).subscribe({
            next: (res) => {
                alert(JSON.stringify(res));
                this.router.navigate(['/portal']);
            },
            error: (err) => alert(JSON.stringify(err))
        });
    }
}
export interface AircraftCreateDto {
    RegNumber: string,
    Model: string,
    ThumbnailUri: string
}
export interface Permission {
    id: string,
    name: string,
    description: string
}
export interface PermissionCategory {
    id: string,
    name: string,
    permissions: Permission[]
}
export interface RoleCreateDto {
    Name: string,
    Description?: string,
    Permissions: string[]
}