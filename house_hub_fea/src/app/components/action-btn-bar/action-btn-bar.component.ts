import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { NzButtonModule, NzButtonType } from 'ng-zorro-antd/button';
import { NzSizeLDSType } from 'ng-zorro-antd/core/types';
import { NzIconModule } from 'ng-zorro-antd/icon';

export interface familyHubButtonConfig {
  text: string;
  icon?: string;
  size: NzSizeLDSType;
  type: NzButtonType;
  action: () => void;
  visible?: boolean;
}

@Component({
  selector: 'app-action-btn-bar',
  imports: [CommonModule, NzButtonModule, NzIconModule],
  templateUrl: './action-btn-bar.component.html',
  styleUrl: './action-btn-bar.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ActionBtnBarComponent {
  buttonConfig = input<familyHubButtonConfig[]>([]);
  alignRight = input<boolean>(false);

  constructor() {}
}
