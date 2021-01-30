import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

@Injectable({
    providedIn: 'root'
})
export class AccountService {
    baseUrl = environment.apiUrl;
    private currentUserSource = new ReplaySubject<any>(1);
    currentUser$ = this.currentUserSource.asObservable();

    constructor(private http: HttpClient) { }

    login(model: any) {
        return this.http.post(this.baseUrl + 'account/login', model).pipe(
            map((response: any) => {
                const user = response;

                if (user) {
                    localStorage.setItem('user', JSON.stringify(user));
                    this.currentUserSource.next(user);
                }
            })
        );
    }
}
