import {HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from "@angular/common/http";
import {delay, finalize, Observable} from "rxjs";
import {Router} from "@angular/router";
import { BusyService } from "../services/busy.service";
import {Injectable} from "@angular/core";

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {
  constructor(private busyService: BusyService) {}
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if(!req.url.includes('emailexists')){
      this.busyService.busy();
    }
    return next.handle(req).pipe(
      delay(700),
      finalize(() => {
        this.busyService.idle()
      })
    );
  }

}
