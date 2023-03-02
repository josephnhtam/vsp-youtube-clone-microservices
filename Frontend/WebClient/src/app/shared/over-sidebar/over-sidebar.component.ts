import {NavigationEnd, Router} from '@angular/router';
import {Component, EventEmitter, OnInit, Output} from '@angular/core';

@Component({
  selector: 'app-over-sidebar',
  templateUrl: './over-sidebar.component.html',
  styleUrls: ['./over-sidebar.component.css'],
})
export class OverSidebarComponent implements OnInit {
  @Output() onClose = new EventEmitter();

  constructor(private router: Router) {}

  ngOnInit(): void {
    this.router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        this.close();
      }
    });
  }

  close() {
    this.onClose.emit();
  }
}
