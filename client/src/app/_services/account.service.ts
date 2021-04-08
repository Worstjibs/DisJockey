import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { CurrentUser } from '../_models/currentUser';

@Injectable({
    providedIn: 'root'
})
export class AccountService {
    baseUrl = environment.apiUrl;
    private currentUserSource = new ReplaySubject<CurrentUser>(1);
    currentUser$ = this.currentUserSource.asObservable();

    constructor(private http: HttpClient, private router: Router) { }

    // login(model: any) {
    //     return this.http.post(this.baseUrl + 'account/login', model).pipe(
    //         map((response: any) => {
    //             const user = response;

    //             if (user) {
    //                 localStorage.setItem('user', JSON.stringify(user));
    //                 this.currentUserSource.next(user);
    //             }
    //         })
    //     );
    // }

    getClaims() {
        const user: CurrentUser = JSON.parse(localStorage.getItem('user'));

        if (user) {
            this.currentUserSource.next(user);
        } else {
            this.http.get(this.baseUrl + 'account/claims').subscribe((response: any) => {
                if (response) {
                    localStorage.setItem('user', JSON.stringify(response));                
                    this.currentUserSource.next(response);
                }
            });
        }
    }

    logout() {
        localStorage.removeItem('user');
        this.currentUserSource.next(null);

        window.location.href = this.baseUrl + 'account/logout';
    }
}
