import { Injectable } from '@angular/core';
import {NgxSpinnerService} from "ngx-spinner";

@Injectable({
  providedIn: 'root'
})
export class BusyService {
  busyRequestCount = 0;


  constructor(private spinnerService: NgxSpinnerService) { }

  private spinnerOptions = {
    type: 'pacman',
    bdColor: 'rgba(255,255,255,0.7)',
    color: '#333333',
  };
  busy(){
    this.busyRequestCount++;
    this.spinnerService.show(undefined,  this.spinnerOptions);
  }

  idle(): void {
    this.busyRequestCount--;
    if(this.busyRequestCount <= 0){
      this.busyRequestCount = 0;
      this.spinnerService.hide();
    }
  }
}
