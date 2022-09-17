import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faPlaneDeparture, faUsers, faSitemap, faCog, faBuilding } from '@fortawesome/free-solid-svg-icons'

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnInit {
  faPlaneDeparture = faPlaneDeparture;
  faUsers = faUsers;
  faSitemap = faSitemap;
  faCog = faCog;
  faBuilding = faBuilding;

  constructor(private route: ActivatedRoute) {
    route.url
  }

  ngOnInit() {

  }
}
