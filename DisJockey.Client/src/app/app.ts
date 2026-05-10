import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Nav } from './nav/nav';
import { BottomNav } from './bottom-nav/bottom-nav';
import { AccountService, User } from './core/services/account-service';
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Nav, BottomNav],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  private readonly accountService = inject(AccountService);

  protected readonly currentUser = toSignal<User | null>(
    this.accountService.getCurrentUser(),
    { initialValue: null }
  );
}
