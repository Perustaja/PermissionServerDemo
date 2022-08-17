import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { OidcAuthService } from "./oidcauth.service";

@Injectable({
    providedIn: "root"
})
export class AuthInterceptor implements HttpInterceptor {

    constructor(private oidcAuthService: OidcAuthService) {
    }

    intercept(req: HttpRequest<any>, next: HttpHandler)
        : Observable<HttpEvent<any>> {
        var requestPath = req.url;
        let auth_header = "";
        if (this.oidcAuthService.isLoggedIn()) {
            auth_header = this.oidcAuthService.getAuthorizationHeaderValue();
        }
        req = req.clone({
            setHeaders: {
                "Authorization": auth_header
            }
        });

        return next.handle(req);
    }
}