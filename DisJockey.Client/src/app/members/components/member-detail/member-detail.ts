import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { map, switchMap } from 'rxjs';
import { MemberApi } from '../../services/member-api';
import { MemberCard } from '../member-card/member-card';
import { TrackList } from '../../../tracks/components/track-list/track-list';
import { TrackListType } from '../../../tracks/enums/trackListType';

@Component({
  selector: 'app-member-detail',
  imports: [MemberCard, TrackList],
  templateUrl: './member-detail.html',
  styleUrl: './member-detail.scss',
})
export class MemberDetail {
  private readonly memberApi = inject(MemberApi);

  protected readonly TrackListType = TrackListType;

  protected readonly member = toSignal(
    inject(ActivatedRoute).paramMap.pipe(
      map(params => params.get('discordId')!),
      switchMap(discordId => this.memberApi.getMember(discordId))
    )
  );
}
