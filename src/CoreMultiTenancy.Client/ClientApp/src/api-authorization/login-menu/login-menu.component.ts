import { Component, Inject, OnInit } from '@angular/core';
import { faUserCircle } from '@fortawesome/free-solid-svg-icons';
import { map, Observable } from 'rxjs';
import { AuthorizeService } from '../authorize.service';

@Component({
  selector: 'app-login-menu',
  templateUrl: './login-menu.component.html',
  styleUrls: ['./login-menu.component.css']
})
export class LoginMenuComponent implements OnInit {
  faUserCircle = faUserCircle;
  idpBaseUrl: string;
  userName?: Observable<string | null | undefined>;

  constructor(private authorizeSvc: AuthorizeService,
    @Inject('IDP_BASE_URL') idpBaseUrl: string) {
    this.idpBaseUrl = idpBaseUrl;
  }

  ngOnInit() {
    this.userName = this.authorizeSvc.getUser().pipe(map(u => u && u.name));
  }
}
