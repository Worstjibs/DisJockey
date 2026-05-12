import { Component, input } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Member } from '../../models/member';

@Component({
  selector: 'app-member-card',
  imports: [DatePipe],
  templateUrl: './member-card.html',
  styleUrl: './member-card.scss',
})
export class MemberCard {
  member = input<Member | null>(null);
}
