import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { CheckboxModule } from 'primeng/checkbox';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-primeng-test',
  standalone: true,
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
      <h2>PrimeNG Components Test</h2>

      <p-card
        header="Basic Components"
        [style]="{ width: '100%', margin: '1rem 0' }"
      >
        <div class="grid">
          <div class="col-12 md:col-6">
            <label for="username">Username:</label>
            <input
              pInputText
              id="username"
              [(ngModel)]="username"
              placeholder="Enter username"
              class="w-full"
            />
          </div>

          <div class="col-12 md:col-6">
            <label for="date">Date:</label>
            <p-calendar
              id="date"
              [(ngModel)]="selectedDate"
              dateFormat="mm/dd/yy"
              placeholder="Select date"
              [showIcon]="true"
              class="w-full"
            >
            </p-calendar>
          </div>

          <div class="col-12 md:col-6">
            <label for="city">City:</label>
            <p-dropdown
              [options]="cities"
              [(ngModel)]="selectedCity"
              optionLabel="name"
              placeholder="Select a city"
              class="w-full"
            >
            </p-dropdown>
          </div>

          <div class="col-12 md:col-6">
            <div class="field-checkbox">
              <p-checkbox
                [(ngModel)]="checked"
                [binary]="true"
                id="accept"
              ></p-checkbox>
              <label for="accept">I agree to the terms and conditions</label>
            </div>
          </div>
        </div>

        <div class="flex gap-2 mt-3">
          <p-button
            label="Primary Button"
            (onClick)="showSuccess()"
            [raised]="true"
          >
          </p-button>

          <p-button
            label="Secondary"
            (onClick)="showInfo()"
            severity="secondary"
            [outlined]="true"
          >
          </p-button>

          <p-button
            label="Danger"
            (onClick)="showError()"
            severity="danger"
            icon="pi pi-times"
          >
          </p-button>
        </div>
      </p-card>

      <p-card header="Form Data" [style]="{ width: '100%', margin: '1rem 0' }">
        <div class="grid">
          <div class="col-12">
            <strong>Current Values:</strong>
          </div>
          <div class="col-12">
            <p><strong>Username:</strong> {{ username || 'Not set' }}</p>
            <p><strong>Selected Date:</strong> {{ getFormattedDate() }}</p>
            <p>
              <strong>Selected City:</strong>
              {{ selectedCity?.name || 'Not selected' }}
            </p>
            <p>
              <strong>Checkbox:</strong> {{ checked ? 'Checked' : 'Unchecked' }}
            </p>
          </div>
        </div>
      </p-card>
    </div>

    <p-toast></p-toast>
  `,
  styles: [
    `
      .grid {
        display: grid;
        gap: 1rem;
        grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      }

      .col-12 {
        grid-column: 1 / -1;
      }

      @media (min-width: 768px) {
        .md\\:col-6 {
          grid-column: span 6;
        }
      }

      .field-checkbox {
        display: flex;
        align-items: center;
        gap: 0.5rem;
      }

      .w-full {
        width: 100%;
      }

      .flex {
        display: flex;
      }

      .gap-2 {
        gap: 0.5rem;
      }

      .mt-3 {
        margin-top: 1rem;
      }

      .p-4 {
        padding: 1.5rem;
      }
    `,
  ],
})
export class PrimengTestComponent {
  username: string = '';
  selectedDate: Date | null = null;
  selectedCity: any = null;
  checked: boolean = false;

  cities = [
    { name: 'New York', code: 'NY' },
    { name: 'Rome', code: 'RM' },
    { name: 'London', code: 'LDN' },
    { name: 'Istanbul', code: 'IST' },
    { name: 'Paris', code: 'PRS' },
  ];

  constructor(private messageService: MessageService) {}

  showSuccess() {
    this.messageService.add({
      severity: 'success',
      summary: 'Success',
      detail: 'Primary button clicked!',
    });
  }

  showInfo() {
    this.messageService.add({
      severity: 'info',
      summary: 'Info',
      detail: 'Secondary button clicked!',
    });
  }

  showError() {
    this.messageService.add({
      severity: 'error',
      summary: 'Error',
      detail: 'Danger button clicked!',
    });
  }

  getFormattedDate(): string {
    if (!this.selectedDate) {
      return 'Not selected';
    }
    return this.selectedDate.toLocaleDateString();
  }
}
