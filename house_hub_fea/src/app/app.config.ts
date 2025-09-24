import { provideHttpClient } from '@angular/common/http';
import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideRouter } from '@angular/router';
import { provideNzIcons } from 'ng-zorro-antd/icon';
import { providePrimeNG } from 'primeng/config';
import { TodoStore } from './data';

import { routes } from './app.routes';
import Aura from '@primeng/themes/aura';

// Import ng-zorro-antd icons
import {
  BellOutline,
  CalendarOutline,
  CheckCircleOutline,
  CheckOutline,
  ClockCircleOutline,
  HomeOutline,
  LogoutOutline,
  MenuOutline,
  ScheduleOutline,
  SettingOutline,
  TeamOutline,
  UnorderedListOutline,
  UserOutline,
} from '@ant-design/icons-angular/icons';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(),
    provideAnimations(), // Required for animations
    providePrimeNG({
      theme: {
        preset: Aura,
      },
    }),
    provideNzIcons([
      MenuOutline,
      BellOutline,
      UserOutline,
      SettingOutline,
      LogoutOutline,
      HomeOutline,
      CheckCircleOutline,
      UnorderedListOutline,
      CalendarOutline,
      CheckOutline,
      ClockCircleOutline,
      TeamOutline,
      ScheduleOutline,
    ]),
    TodoStore,
  ],
};
