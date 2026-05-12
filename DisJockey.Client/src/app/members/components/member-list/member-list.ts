import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { MemberApi } from '../../services/member-api';
import { MemberCard } from '../member-card/member-card';

@Component({
  selector: 'app-member-list',
  imports: [RouterLink, MemberCard],
  templateUrl: './member-list.html',
  styleUrl: './member-list.scss',
})
export class MemberList {
  private readonly memberApi = inject(MemberApi);

  protected readonly members = toSignal(this.memberApi.getMembers(), { initialValue: [] });
}
