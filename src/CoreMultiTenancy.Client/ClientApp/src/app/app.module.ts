import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { PortalComponent } from './portal/portal.component';
import { ApiAuthorizationModule } from 'src/api-authorization/api-authorization.module';
import { AuthorizeInterceptor } from 'src/api-authorization/authorize.interceptor';
import { AuthorizeGuard } from 'src/api-authorization/authorize.guard';
import { TenantGuard } from '../tenancy/tenant.guard';
import { AircraftComponent } from './aircraft/aircraft.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    PortalComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ApiAuthorizationModule,
    RouterModule.forRoot([
      { path: '', component: PortalComponent, canActivate: [AuthorizeGuard] },
      { path: 'portal', component: PortalComponent, canActivate: [AuthorizeGuard] },
      { path: 'aircraft', component: AircraftComponent, canActivate: [AuthorizeGuard, TenantGuard] },
    ]),
    FontAwesomeModule,
    NgbModule,
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthorizeInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
