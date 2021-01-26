import { Component, OnInit } from '@angular/core';
import { MemberService } from '../_services/member.service';

@Component({
    selector: 'app-users',
    templateUrl: './users.component.html',
    styleUrls: ['./users.component.css']
})
export class UsersComponent implements OnInit {
    members$: any;

    constructor(private memberService: MemberService) { }

    ngOnInit(): void {
        this.members$ = this.memberService.getMembers();
    }

}
