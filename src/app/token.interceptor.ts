import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpHeaders
} from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class TokenInterceptor implements HttpInterceptor {

  constructor() {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    console.log('Interceptor is working');
    const token=localStorage.getItem('token');
    console.log("tokenHttpInterceptor",token);

    const modifiedRequest=request.clone({
      headers:new HttpHeaders(
        {
          'Authorization':'Bearer '+token
      }
    )
  })
  console.log("request",modifiedRequest);
    return next.handle(modifiedRequest);
  }
}
