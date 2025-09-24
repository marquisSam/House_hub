import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { NzMenuModule } from 'ng-zorro-antd/menu';

@Component({
  selector: 'app-home-layout',
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    NzLayoutModule,
    NzIconModule,
    NzMenuModule,
  ],
  templateUrl: './home-layout.component.html',
  styleUrl: './home-layout.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomeLayoutComponent {}
