import { HttpClient } from "@angular/common/http";
import { Component, Inject, OnInit } from "@angular/core";
import { TenantManagerService } from "src/tenancy/tenantManager.service";

@Component({
    selector: 'app-users-component',
    templateUrl: './users.component.html',
    styleUrls: ['./users.component.css'],
    providers: [TenantManagerService]
})
export class UsersComponent implements OnInit {
    idpApiUrl: string;
    users: User[] = [];

    constructor(private http: HttpClient,
        private tenantManager: TenantManagerService,
        @Inject('IDP_API_URL') idpApiUrl: string) {
        this.idpApiUrl = idpApiUrl;
    }

    ngOnInit() {
        this.tenantManager.tenantId$.subscribe(tid => {
            this.http.get<User[]>(`${this.idpApiUrl}/organizations/${tid}/users`).subscribe({
                next: (res) => this.users = res,
                error: (e) => console.log(e)
            });
        })
    }
}

export interface User {
    firstName: string,
    lastName: string,
    userOrganization: UserOrganization,
    roles: Role[]
}

export interface UserOrganization {
    awaitingApproval: boolean,
    blacklisted: boolean,
    dateSubmitted: Date
}

export interface Role {
    name: string,
    description: string,
    isGlobal: boolean
}