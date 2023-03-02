import {map} from 'rxjs';
import {Store} from '@ngrx/store';
import {Component, Input, OnInit} from '@angular/core';
import {selectHistoryRecords} from '../../../core/selectors/history';

@Component({
  selector: 'app-user-watch-history',
  templateUrl: './user-watch-history.component.html',
  styleUrls: ['./user-watch-history.component.css'],
})
export class UserWatchHistoryComponent implements OnInit {
  @Input()
  dateTime!: number;

  dateString!: string;

  from!: Date;
  to!: Date;

  constructor(private store: Store) {}

  ngOnInit(): void {
    this.from = new Date(this.dateTime);

    this.to = new Date(this.from);
    this.to.setDate(this.to.getDate() + 1);

    this.dateString = `${
      monthNames[this.from.getMonth()]
    } ${this.from.getDate()}, ${this.from.getFullYear()}`;
  }

  get historyRecords$() {
    const fromTime = this.from.getTime();
    const toTime = this.to.getTime();

    return this.store.select(selectHistoryRecords).pipe(
      map((x) => {
        return x.filter((r) => {
          const time = r.parsedDate.getTime();
          return time >= fromTime && time < toTime;
        });
      })
    );
  }
}

const monthNames = [
  'Jan',
  'Feb',
  'Mar',
  'Apr',
  'May',
  'Jun',
  'Jul',
  'Aug',
  'Sep',
  'Oct',
  'Nov',
  'Dec',
];
