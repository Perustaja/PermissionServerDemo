import { Component, Inject } from '@angular/core';
import { faUserCircle } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-login-menu',
  templateUrl: './login-menu.component.html',
  styleUrls: ['./login-menu.component.css']
})
export class LoginMenuComponent{
  faUserCircle = faUserCircle;
  idpBaseUrl: string;

  constructor(@Inject('IDP_BASE_URL') idpBaseUrl: string) {
    this.idpBaseUrl = idpBaseUrl;
  }
}
