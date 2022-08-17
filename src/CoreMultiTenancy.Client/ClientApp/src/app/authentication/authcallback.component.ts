import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { OidcAuthService } from './oidcauth.service';

@Component({
    selector: "auth-callback",
    template: "./auth-callback.component.html"
})

export class AuthCallBackComponent implements OnInit {
    constructor(private authSvc: OidcAuthService, private router: Router) { }
    ngOnInit(): void {
        this.authSvc.completeSignIn().then((v) => {
            this.router.navigate(["/"]);
        })
    }
}