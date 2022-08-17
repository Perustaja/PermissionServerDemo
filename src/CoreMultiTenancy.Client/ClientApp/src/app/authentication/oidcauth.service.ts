import { Injectable } from "@angular/core"
import { CanActivate } from "@angular/router";
import { User, UserManager, UserManagerSettings } from "oidc-client"
import { Observable, Subject } from "rxjs";

@Injectable({
    providedIn: "root"
})
export class OidcAuthService implements CanActivate {
    private config: UserManagerSettings = {
        authority: "https://localhost:5100",
        client_id: "testclient",
        redirect_uri: "https://localhost:5001/signin-oidc",
        post_logout_redirect_uri: "https://localhost:5001/signout-callback-oidc",
        response_type: "id_token",
        scope: "openid profile testapi",
        filterProtocolClaims: true,
        loadUserInfo: true
    };

    private manager = new UserManager(this.config);
    private user: User = null;
    private subject = new Subject<boolean>();

    constructor() {
        this.manager.getUser().then(u => {
            this.user = u;
        });
    }

    getUserLoggedInEvents(): Observable<boolean> {
        return this.subject.asObservable();
    }

    isLoggedIn(): boolean {
        return this.user != null && !this.user.expired;
    }

    async signIn(): Promise<void> {
        return this.manager.signinRedirect();
    }

    async completeSignIn(): Promise<void> {
        return this.manager.signinRedirectCallback().then(u => {
            this.user = u;
            this.subject.next(this.isLoggedIn());
        });
    }

    async signOut(): Promise<void> {
        return this.manager.signoutRedirect();
    }

    async completeSignOut(): Promise<void> {
        return this.manager.signoutRedirectCallback().then(u => {
            this.user = null;
            this.subject.next(this.isLoggedIn());
        })
    }

    getAuthorizationHeaderValue(): string {
        return `Bearer ${this.user.id_token}`;
    }

    canActivate(): boolean {
        if (this.isLoggedIn())
            return true;
        else {
            this.signIn();
            return false;
        }
    }
}