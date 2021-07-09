import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';

@Injectable({
    providedIn: 'root'
})
export class MemberService {
    baseUrl = environment.apiUrl;
    members$: any = [];

    constructor(private http: HttpClient) { }

    getMembers(): Observable<Member[]> {
        return this.http.get<Member[]>(this.baseUrl + "members").pipe(
            map(members => {
                this.members$ = members;
                return members;
            })
        );
    }

    getMember(discordId: string): Observable<Member> {
        return this.http.get<Member>(this.baseUrl + "members/" + discordId);
	}
}
