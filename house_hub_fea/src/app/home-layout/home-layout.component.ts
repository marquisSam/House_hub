import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-home-layout',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './home-layout.component.html',
  styleUrl: './home-layout.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomeLayoutComponent {}
