import { Injectable } from '@angular/core';
import {
    HttpRequest,
    HttpHandler,
    HttpEvent,
    HttpInterceptor
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { catchError } from 'rxjs/operators';
import { NavigationExtras, Router } from '@angular/router';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

    constructor(private toastr: ToastrService, private router: Router) { }

    intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
        return next.handle(request).pipe(
            catchError(error => {
                switch(error.status) {
                    case 400:
                        this.toastr.error(error.error);
                        break;

                    case 404:
                        this.router.navigateByUrl('/not-found');
                        break;
                        
                    case 500:
                        console.log(error);
                        const navigationsExtras: NavigationExtras = { state: { error: error.error } }
                        this.router.navigateByUrl('/server-error', navigationsExtras);
                        break;

                    default:
                        this.toastr.error('Something went wrong');
                        break;
                }

                return throwError(error);
            })
        );
    }
}
