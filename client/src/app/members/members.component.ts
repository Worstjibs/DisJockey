import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { Member } from '../_models/member';
import { MemberService } from '../_services/member.service';

@Component({
    selector: 'app-users',
    templateUrl: './members.component.html',
    styleUrls: ['./members.component.css']
})
export class MembersComponent implements OnInit {
    members$: Observable<Member[]>;

    constructor(private memberService: MemberService) { }

    ngOnInit(): void {
        this.members$ = this.memberService.getMembers();

        console.log(this.members$);
    }

}
