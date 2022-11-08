import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { MatButtonModule } from '@angular/material/button'
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatExpansionModule, MatExpansionPanel } from '@angular/material/expansion'

import { AppComponent } from './app.component';
import { SidebarComponent } from './sidebar/sidebar.component';
import { PortalComponent } from './portal/portal.component';
import { ApiAuthorizationModule } from 'src/api-authorization/api-authorization.module';
import { AuthorizeInterceptor } from 'src/api-authorization/authorize.interceptor';
import { AuthorizeGuard } from 'src/api-authorization/authorize.guard';
import { TenantGuard } from '../tenancy/tenant.guard';
import { AircraftComponent } from './aircraft/aircraft.component';
import { UsersComponent } from './users/users.component';
import { RolesComponent } from './roles/roles.component';
import { DemoControlsComponent } from './demo-controls/demo-controls.component';
import { ToastsComponent } from './toasts/toasts.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

@NgModule({
  declarations: [
    AppComponent,
    ToastsComponent,
    SidebarComponent,
    PortalComponent,
    AircraftComponent,
    UsersComponent,
    RolesComponent,
    DemoControlsComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ApiAuthorizationModule,
    RouterModule.forRoot([
      { path: '', redirectTo: '/portal', pathMatch: 'full'},
      { path: 'portal', component: PortalComponent, canActivate: [AuthorizeGuard] },
      { path: 'aircraft', component: AircraftComponent, canActivate: [AuthorizeGuard, TenantGuard] },
      { path: 'users', component: UsersComponent, canActivate: [AuthorizeGuard, TenantGuard] },
      { path: 'roles', component: RolesComponent, canActivate: [AuthorizeGuard, TenantGuard] },
      { path: 'demo-controls', component: DemoControlsComponent, canActivate: [AuthorizeGuard, TenantGuard] },
    ]),
    FontAwesomeModule,
    MatButtonModule,
    MatToolbarModule,
    MatSidenavModule,
    MatExpansionModule,
    MatIconModule,
    MatDividerModule,
    NgbModule,
    BrowserAnimationsModule,
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthorizeInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
