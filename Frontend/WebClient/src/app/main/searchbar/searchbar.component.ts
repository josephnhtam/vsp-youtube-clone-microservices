import {NgForm} from '@angular/forms';
import {Router} from '@angular/router';
import {Component, OnInit} from '@angular/core';
import {SearchParams} from '../../core/models/search';

@Component({
  selector: 'app-searchbar',
  templateUrl: './searchbar.component.html',
  styleUrls: ['./searchbar.component.css'],
})
export class SearchbarComponent implements OnInit {
  constructor(private router: Router) {}

  ngOnInit(): void {}

  search(form: NgForm) {
    const params: SearchParams = {
      query: form.value['query'].trim(),
    };

    if (params.query == '') {
      return false;
    }

    this.router.navigate(['/', 'search'], {
      queryParams: params,
    });

    return false;
  }
}
