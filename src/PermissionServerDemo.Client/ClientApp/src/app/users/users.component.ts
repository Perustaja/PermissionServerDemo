import { HttpClient } from "@angular/common/http";
import { Component, Inject, OnInit, OnDestroy } from '@angular/core';
import { Subject, takeUntil } from "rxjs";
import { TenantManagerService } from "src/tenancy/tenantManager.service";

@Component({
    selector: 'app-users-component',
    templateUrl: './users.component.html',
    styleUrls: ['./users.component.css'],
    providers: [TenantManagerService]
})
export class UsersComponent implements OnInit, OnDestroy {
    private ngUnsub = new Subject<void>();
    idpApiUrl: string;
    users: User[] = [];

    constructor(private http: HttpClient,
        private tenantManager: TenantManagerService,
        @Inject('IDP_API_URL') idpApiUrl: string) {
        this.idpApiUrl = idpApiUrl;
    }

    ngOnInit() {
        this.tenantManager.tenantId$
            .pipe(takeUntil(this.ngUnsub))
            .subscribe(tid => {
                this.http.get<User[]>(`${this.idpApiUrl}/organizations/${tid}/users`).subscribe({
                    next: (res) => this.users = res,
                    error: (e) => console.log(e)
                });
            })
    }

    ngOnDestroy() {
        this.ngUnsub.next();
        this.ngUnsub.complete();
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