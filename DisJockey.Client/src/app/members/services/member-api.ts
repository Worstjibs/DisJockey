import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Member } from '../models/member';

@Injectable({
  providedIn: 'root',
})
export class MemberApi {
  private readonly baseUrl = 'api/members';

  private readonly http = inject(HttpClient);

  getMembers(): Observable<Member[]> {
    return this.http.get<Member[]>(this.baseUrl);
  }

  getMember(discordId: string): Observable<Member> {
    return this.http.get<Member>(`${this.baseUrl}/${discordId}`);
  }
}
