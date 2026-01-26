import {Component, OnDestroy, OnInit} from '@angular/core';
import {Router} from "@angular/router";
import {BreadcrumbService} from "xng-breadcrumb";
import {Observable} from "rxjs";

@Component({
  selector: 'app-section-header',
  templateUrl: './section-header.component.html',
  styleUrls: ['./section-header.component.scss']
})
export class SectionHeaderComponent implements OnInit{

  breadcrumb$: Observable<any[]>;

  constructor(private bcService: BreadcrumbService) { }

  ngOnInit(): void {
        this.breadcrumb$ = this.bcService.breadcrumbs$;
    }
}
