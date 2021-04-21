import { Component, OnInit } from '@angular/core';
import { MemberService } from '../_services/member.service';

@Component({
    selector: 'app-users',
    templateUrl: './members.component.html',
    styleUrls: ['./members.component.css']
})
export class MembersComponent implements OnInit {
    members$: any;

    constructor(private memberService: MemberService) { }

    ngOnInit(): void {
        this.members$ = this.memberService.getMembers();
    }

}
