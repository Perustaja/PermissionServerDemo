import { Component, HostListener, Inject, OnDestroy, OnInit } from '@angular/core';
import { EventManager } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { faPlaneDeparture, faUsers, faSitemap, faCog, faBuilding, faUserCircle } from '@fortawesome/free-solid-svg-icons'
import { fromEvent, Observable, Subject, Subscription } from 'rxjs';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent {
  faUserCircle = faUserCircle;
  faPlaneDeparture = faPlaneDeparture;
  faUsers = faUsers;
  faSitemap = faSitemap;
  faCog = faCog;
  faBuilding = faBuilding;
  idpBaseUrl: string;

  sidenavMode: string;
  showMobileLinks: boolean;

  constructor(private route: ActivatedRoute,
    @Inject('IDP_BASE_URL') idpBaseUrl: string) {
    this.idpBaseUrl = idpBaseUrl;
    this.sidenavMode = (window.innerWidth > 767) ? 'side' : 'over';
    this.showMobileLinks = false;
  }

  toggleMobileNavLinks() {
    this.showMobileLinks = !this.showMobileLinks;
  }

  @HostListener('window:resize', ['$event'])
  onResize(event: { target: { innerWidth: any; }; }) {
    if (event.target.innerWidth > 767) {
      this.sidenavMode = 'side';
      this.showMobileLinks = false;
    }
    else {
      this.sidenavMode = 'over';
    }
  }
}
