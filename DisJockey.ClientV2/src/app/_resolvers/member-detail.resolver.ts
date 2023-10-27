import { Injectable } from '@angular/core';
import {
	Router, Resolve,
	RouterStateSnapshot,
	ActivatedRouteSnapshot
} from '@angular/router';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { Member } from '../_models/member';
import { MemberService } from '../_services/member.service';

@Injectable({
	providedIn: 'root'
})
export class MemberDetailResolver implements Resolve<Member> {

	constructor(private readonly memberService: MemberService) {

	}

	resolve(route: ActivatedRouteSnapshot): Observable<Member> {
		return this.memberService.getMember(route.paramMap.get('discordId'));
	}
}
