import { Component, computed, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AccountService, User } from '../core/services/account-service';
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-nav',
  imports: [RouterModule],
  templateUrl: './nav.html',
  styleUrl: './nav.scss',
})
export class Nav {
  accountService = inject(AccountService);

  protected readonly currentUser = toSignal<User | null>(
    this.accountService.getCurrentUser(),
    {
      initialValue: null
    }
  );
}
