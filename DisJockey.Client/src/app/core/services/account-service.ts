import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private readonly httpClient = inject(HttpClient);

  getCurrentUser(): Observable<User | null> {
    return this.httpClient.get<User | null>('/account/userinfo')
  }
}

export interface User {
  discordId: string;
  avatarUrl: string;
  username: string;
}