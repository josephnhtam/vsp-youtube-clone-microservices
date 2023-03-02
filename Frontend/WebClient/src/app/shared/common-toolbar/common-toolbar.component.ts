import {AuthService} from './../../auth/services/auth.service';
import {CommonToolbarService} from './common-toolbar.service';
import {Component, EventEmitter, HostListener, Input, OnInit, Output, ViewChild,} from '@angular/core';
import {MatMenuTrigger} from '@angular/material/menu';

@Component({
  selector: 'app-common-toolbar',
  templateUrl: './common-toolbar.component.html',
  styleUrls: ['./common-toolbar.component.css'],
})
export class CommonToolbarComponent implements OnInit {
  @Input() forceHideSidebar = false;

  @Output() onToggleMenu = new EventEmitter();

  @ViewChild('accountMenuTrigger', { static: true })
  accountMenuTrigger!: MatMenuTrigger;

  private _miniSidebar = false;

  get isSidebarHidden() {
    return this.sm || this.forceHideSidebar || this.service.shouldHideSidebar;
  }

  get miniSidebar() {
    return this.showMiniSidebar || this._miniSidebar;
  }

  get showMiniSidebar() {
    return this.lg;
  }

  private lg = false;
  private sm = false;

  constructor(
    private service: CommonToolbarService,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    this.checkSize();
  }

  toggleMiniGuide() {
    this._miniSidebar = !this._miniSidebar;
    return false;
  }

  toggleMenu() {
    this.onToggleMenu.emit();
  }

  @HostListener('window:resize')
  checkSize() {
    this.lg = window.innerWidth <= 992;
    this.sm = window.innerWidth <= 576;
  }
}
