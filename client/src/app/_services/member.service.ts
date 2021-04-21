import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

@Injectable({
    providedIn: 'root'
})
export class MemberService {
    baseUrl = environment.apiUrl;
    members$: any = [];

    constructor(private http: HttpClient) { }

    getMembers() {
        return this.http.get(this.baseUrl + "members").pipe(
            map(members => {
                this.members$ = members;
                return members;
            })
        );
    }
}
