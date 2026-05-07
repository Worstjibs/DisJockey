import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Nav } from './nav/nav';
import { BottomNav } from './bottom-nav/bottom-nav';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Nav, BottomNav],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
}
