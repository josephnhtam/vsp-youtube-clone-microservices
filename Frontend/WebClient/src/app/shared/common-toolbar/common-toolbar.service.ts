import {Injectable} from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CommonToolbarService {
  private hideSidebarRequest = 0;

  get shouldHideSidebar() {
    return this.hideSidebarRequest > 0;
  }

  startHideSidebar() {
    this.hideSidebarRequest++;
  }

  endHideSidebar() {
    this.hideSidebarRequest--;
  }
}
