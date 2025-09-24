import { Component } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';

// PrimeNG Imports
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { CheckboxModule } from 'primeng/checkbox';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';

interface City {
  name: string;
  code: string;
}

@Component({
  selector: 'app-primeng-test',
  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    CardModule,
    InputTextModule,
    CalendarModule,
    DropdownModule,
    CheckboxModule,
    ToastModule,
  ],
  providers: [MessageService],
  template: `
    <div class="p-4">
      <p-toast></p-toast>

      <h2>PrimeNG v19 Test Components</h2>

      <div class="grid">
        <!-- Buttons Card -->
        <div class="col-12 md:col-6 lg:col-4">
          <p-card header="Buttons" subheader="Button component variants">
            <div class="flex flex-wrap gap-2">
              <p-button
                label="Primary"
                (onClick)="showSuccess()"
                [loading]="loading"
              >
              </p-button>

              <p-button
                label="Secondary"
                severity="secondary"
                (onClick)="showInfo()"
              >
              </p-button>

              <p-button
                label="Success"
                severity="success"
                icon="pi pi-check"
                (onClick)="showSuccess()"
              >
              </p-button>

              <p-button
                label="Warning"
                severity="warn"
                icon="pi pi-exclamation-triangle"
              >
              </p-button>
            </div>
          </p-card>
        </div>

        <!-- Form Components Card -->
        <div class="col-12 md:col-6 lg:col-4">
          <p-card header="Form Components" subheader="Input controls">
            <div class="flex flex-column gap-3">
              <div>
                <label for="username" class="block text-sm font-medium mb-2"
                  >Username</label
                >
                <input
                  pInputText
                  id="username"
                  [(ngModel)]="username"
                  placeholder="Enter username"
                  class="w-full"
                />
              </div>

              <div>
                <label for="email" class="block text-sm font-medium mb-2"
                  >Email</label
                >
                <input
                  pInputText
                  id="email"
                  [(ngModel)]="email"
                  placeholder="Enter email"
                  class="w-full"
                />
              </div>

              <div>
                <label for="birthdate" class="block text-sm font-medium mb-2"
                  >Birth Date</label
                >
                <p-calendar
                  [(ngModel)]="birthDate"
                  placeholder="Select date"
                  [showIcon]="true"
                  class="w-full"
                >
                </p-calendar>
              </div>
            </div>
          </p-card>
        </div>

        <!-- Dropdown and Checkbox Card -->
        <div class="col-12 md:col-6 lg:col-4">
          <p-card
            header="Selection Components"
            subheader="Dropdown and checkboxes"
          >
            <div class="flex flex-column gap-3">
              <div>
                <label for="city" class="block text-sm font-medium mb-2"
                  >City</label
                >
                <p-dropdown
                  [options]="cities"
                  [(ngModel)]="selectedCity"
                  optionLabel="name"
                  placeholder="Select a City"
                  class="w-full"
                >
                </p-dropdown>
              </div>

              <div class="flex align-items-center">
                <p-checkbox
                  [(ngModel)]="acceptTerms"
                  [binary]="true"
                  id="terms"
                >
                </p-checkbox>
                <label for="terms" class="ml-2"
                  >I accept the terms and conditions</label
                >
              </div>

              <div class="flex align-items-center">
                <p-checkbox
                  [(ngModel)]="subscribeNewsletter"
                  [binary]="true"
                  id="newsletter"
                >
                </p-checkbox>
                <label for="newsletter" class="ml-2"
                  >Subscribe to newsletter</label
                >
              </div>
            </div>
          </p-card>
        </div>

        <!-- Data Display Card -->
        <div class="col-12">
          <p-card header="Form Data" subheader="Current form values">
            <div class="grid">
              <div class="col-12 md:col-6">
                <h4>Input Values:</h4>
                <ul>
                  <li>
                    <strong>Username:</strong> {{ username || 'Not set' }}
                  </li>
                  <li><strong>Email:</strong> {{ email || 'Not set' }}</li>
                  <li>
                    <strong>Birth Date:</strong>
                    {{ getFormattedDate(birthDate) }}
                  </li>
                  <li>
                    <strong>Selected City:</strong>
                    {{ selectedCity?.name || 'Not selected' }}
                  </li>
                </ul>
              </div>
              <div class="col-12 md:col-6">
                <h4>Checkbox States:</h4>
                <ul>
                  <li>
                    <strong>Accept Terms:</strong>
                    {{ acceptTerms ? 'Yes' : 'No' }}
                  </li>
                  <li>
                    <strong>Newsletter:</strong>
                    {{ subscribeNewsletter ? 'Yes' : 'No' }}
                  </li>
                </ul>
              </div>
            </div>
          </p-card>
        </div>
      </div>
    </div>
  `,
  styles: [
    `
      :host {
        display: block;
      }

      .grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
        gap: 1rem;
        margin-top: 1rem;
      }

      .col-12 {
        grid-column: 1 / -1;
      }

      @media (min-width: 768px) {
        .grid {
          grid-template-columns: repeat(12, 1fr);
        }

        .md\\:col-6 {
          grid-column: span 6;
        }

        .col-12 {
          grid-column: span 12;
        }
      }

      @media (min-width: 1024px) {
        .lg\\:col-4 {
          grid-column: span 4;
        }
      }
    `,
  ],
})
export class PrimengTestComponent {
  username: string = '';
  email: string = '';
  birthDate: Date | undefined;
  selectedCity: City | undefined;
  acceptTerms: boolean = false;
  subscribeNewsletter: boolean = false;
  loading: boolean = false;

  cities: City[] = [
    { name: 'New York', code: 'NY' },
    { name: 'Rome', code: 'RM' },
    { name: 'London', code: 'LDN' },
    { name: 'Istanbul', code: 'IST' },
    { name: 'Paris', code: 'PRS' },
  ];

  constructor(private messageService: MessageService) {}

  getFormattedDate(date: Date | undefined): string {
    if (!date) return 'Not set';
    return date.toLocaleDateString();
  }

  showSuccess() {
    this.messageService.add({
      severity: 'success',
      summary: 'Success',
      detail: 'PrimeNG is working perfectly!',
    });
  }

  showInfo() {
    this.messageService.add({
      severity: 'info',
      summary: 'Info',
      detail: 'This is an info message from PrimeNG Toast',
    });
  }
}
