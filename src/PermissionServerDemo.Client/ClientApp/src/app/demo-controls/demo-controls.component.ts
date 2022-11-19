import { Component, Inject, OnInit, TemplateRef } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { map, Observable, take } from "rxjs";
import { TenantManagerService } from '../../tenancy/tenantManager.service';
import { AuthorizeService } from "src/api-authorization/authorize.service";
import { Router } from "@angular/router";
import { ToastService } from "../toasts/toasts.service";

@Component({
    selector: 'demo-controls-component',
    templateUrl: './demo-controls.component.html',
    styleUrls: ['./demo-controls.component.css'],
    providers: [TenantManagerService]
})
export class DemoControlsComponent implements OnInit {
    apiUrl: string;
    idpApiUrl: string;
    subId: string | null | undefined;
    tenantId?: string | null;
    permCats: PermissionCategory[] = [];
    model: RoleCreateDto;
    permissions$?: Observable<string[]>;

    constructor(private http: HttpClient,
        private router: Router,
        private authorizeSvc: AuthorizeService,
        private tenantManager: TenantManagerService,
        private modalSvc: NgbModal,
        private toastService: ToastService,
        @Inject('API_URL') apiUrl: string,
        @Inject('IDP_API_URL') idpApiUrl: string) {
        this.apiUrl = apiUrl;
        this.idpApiUrl = idpApiUrl;
        this.model = <RoleCreateDto>({});
    }

    ngOnInit() {
        // it doesn't make sense to react to changing user/tenant information, only for permissions
        this.authorizeSvc.getUser().pipe(map(u => u && u.sub), take(1)).subscribe(s => this.subId = s);
        this.tenantManager.tenantId$.pipe(take(1)).subscribe(tid => this.tenantId = tid);
        this.permissions$ = this.tenantManager.permissions$;

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
        this.http.post<AircraftCreateDto>(`${this.apiUrl}/organizations/${this.tenantId}/aircraft`, ac)
            .subscribe({
                next: (res) => this.toastService.pushSuccess("Aircraft 'N555YS' ssuccessfully created."),
                error: (err) => {
                    if (err.status == 409)
                        this.toastService.pushWarning("The demo aircraft has already been created. Navigate to the Aircraft page to view it.")
                    else if (err.status == 403)
                        this.toastService.pushDanger("User either does not have access to this tenant or lacks permissions.")
                    else
                        console.log(err)
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
        this.http.post<RoleCreateDto>(this.idpApiUrl + `/organizations/${this.tenantId}/roles`, this.model).subscribe({
            next: (res) => {
                this.toastService.pushSuccess(`Role '${this.model.Name}' successfully created.`);
                this.modalSvc.dismissAll();
            },
            error: (err) => console.log(err)
        });
    }

    removeCreateAircraftRole() {
        // role id is hardcoded for demo
        const url = `${this.idpApiUrl}/organizations/${this.tenantId}/users/${this.subId}/roles/75a7570f-3ce5-48ba-9461-80283ed1d94d`
        this.http.delete(url).subscribe({
            next: (res) => {
                this.toastService.pushSuccess("Role successfully removed.");
                // this is a hacky workaround for handling the updates. A more robust updating 
                // system would be needed but is kind of out of scope for this project
                this.tenantManager.updatePermissions();
            },
            error: (err) => {
                if (err.status == 404)
                    this.toastService.pushWarning("You do not currently have this role.")
                else
                    console.log(err);
            }
        });
    }

    revokeAccess() {
        this.http.delete(`${this.idpApiUrl}/organizations/${this.tenantId}/users/${this.subId}`).subscribe({
            next: (res) => this.router.navigate(['/portal']),
            error: (err) => {
                if (err.status == 400)
                    this.toastService.pushWarning("User is the owner of this tenant and cannot have access revoked. Navigate to the portal and select the other tenant.");
                else
                    console.log(err)
            }
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