import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { Member } from '../_models/member';
import { MemberService } from '../_services/member.service';

@Component({
    selector: 'app-users',
    templateUrl: './members.component.html',
    styleUrls: ['./members.component.css']
})
export class MembersComponent {
    members$: Observable<Member[]>;

    constructor(private memberService: MemberService) {
        this.members$ = this.memberService.getMembers();
    }
}
