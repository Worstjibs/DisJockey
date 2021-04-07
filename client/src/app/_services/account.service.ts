import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { ClaimsIdentity } from '../_models/claimsIdentity';
import { CurrentUser } from '../_models/currentUser';

@Injectable({
    providedIn: 'root'
})
export class AccountService {
    baseUrl = environment.apiUrl;
    private currentUserSource = new ReplaySubject<CurrentUser>(1);
    currentUser$ = this.currentUserSource.asObservable();

    constructor(private http: HttpClient) { }

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
        this.http.get(this.baseUrl + 'account/claims').subscribe((response: ClaimsIdentity) => {
            // const claimsIdentity: ClaimsIdentity = {
            //     claims: response.claims,
            //     isAuthenticated: response.isAuthenticated,
            //     name: response.name
            // };
            let claims = response.claims;

            if (claims.length > 0) {
                const discordId: number = claims.find(claim => claim.type === "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").value || "";
                const username: string = claims.find(claim => claim.type === "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name").value || 0;
                const avatarUrl: string = claims.find(claim => claim.type === "urn:discord:avatar:url").value || "";

                const user: CurrentUser = {
                    avatarUrl,
                    discordId,
                    username
                }

                console.log(user);

                this.currentUserSource.next(user);
            }

            

            
        });
    }
}
