import { CommonModule } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Input,
  Output,
  inject,
} from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzMessageService } from 'ng-zorro-antd/message';
import { User, UpdateUserRequest } from '../../../data/models/usersModel';

@Component({
  selector: 'app-user-card',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    NzCardModule,
    NzButtonModule,
    NzIconModule,
    NzTagModule,
    NzModalModule,
    NzInputModule,
  ],
  template: `
    <nz-card class="user-card" [nzActions]="cardActions" [nzExtra]="statusTemplate">
      <ng-template #statusTemplate>
        <nz-tag [nzColor]="user.IsActive ? 'green' : 'red'">
          {{ user.IsActive ? 'Active' : 'Inactive' }}
        </nz-tag>
      </ng-template>

      <!-- User Info -->
      <div class="user-info">
        <div class="user-avatar">
          <nz-icon nzType="user" nzTheme="outline"></nz-icon>
        </div>

        <div class="user-details">
          <h3 class="user-name">{{ getFullName() }}</h3>
          <div class="user-meta">
            <div class="meta-item" *ngIf="user.Email">
              <nz-icon nzType="mail" nzTheme="outline"></nz-icon>
              <span>{{ user.Email }}</span>
            </div>
            <div class="meta-item" *ngIf="user.PhoneNumber">
              <nz-icon nzType="phone" nzTheme="outline"></nz-icon>
              <span>{{ user.PhoneNumber }}</span>
            </div>
            <div class="meta-item" *ngIf="user.DateOfBirth">
              <nz-icon nzType="calendar" nzTheme="outline"></nz-icon>
              <span>{{ getFormattedBirthDate() }} ({{ getAge() }} years old)</span>
            </div>
            <div class="meta-item" *ngIf="user.City">
              <nz-icon nzType="environment" nzTheme="outline"></nz-icon>
              <span>{{ user.City }}{{ user.Country ? ', ' + user.Country : '' }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Additional Info -->
      <div class="user-additional" *ngIf="hasAdditionalInfo()">
        <div class="additional-item" *ngIf="user.Gender">
          <span class="label">Gender:</span>
          <span class="value">{{ user.Gender }}</span>
        </div>
        <div class="additional-item" *ngIf="user.Address">
          <span class="label">Address:</span>
          <span class="value">{{ user.Address }}</span>
        </div>
      </div>

      <!-- Card Actions Template -->
      <ng-template #editAction>
        <nz-icon nzType="edit" nzTheme="outline" (click)="openEditModal()"></nz-icon>
      </ng-template>

      <ng-template #deleteAction>
        <nz-icon nzType="delete" nzTheme="outline" (click)="confirmDelete()"></nz-icon>
      </ng-template>

      <ng-template #viewAction>
        <nz-icon nzType="eye" nzTheme="outline" (click)="viewDetails()"></nz-icon>
      </ng-template>
    </nz-card>
  `,
  styleUrls: ['./user-card.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UserCardComponent {
  @Input() user!: User;
  @Output() userUpdate = new EventEmitter<User>();
  @Output() userDelete = new EventEmitter<User>();

  private fb = inject(FormBuilder);
  private modal = inject(NzModalService);
  private message = inject(NzMessageService);

  // Card actions configuration
  get cardActions() {
    return [this.editAction, this.deleteAction, this.viewAction];
  }

  getFullName(): string {
    return `${this.user.FirstName} ${this.user.LastName}`.trim();
  }

  getFormattedBirthDate(): string {
    if (!this.user.DateOfBirth) return 'Not provided';
    const date = new Date(this.user.DateOfBirth);
    return date.toLocaleDateString();
  }

  getAge(): number | string {
    if (!this.user.DateOfBirth) return 'Unknown';

    const birthDate = new Date(this.user.DateOfBirth);
    const today = new Date();
    let age = today.getFullYear() - birthDate.getFullYear();
    const monthDiff = today.getMonth() - birthDate.getMonth();

    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
      age--;
    }

    return age;
  }

  private formatDateForInput(dateString: string | null | undefined): string {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toISOString().split('T')[0];
  }

  hasAdditionalInfo(): boolean {
    return !!(this.user.Gender || this.user.Address);
  }

  openEditModal() {
    const editForm = this.fb.group({
      firstName: [this.user.FirstName],
      lastName: [this.user.LastName],
      email: [this.user.Email],
      phoneNumber: [this.user.PhoneNumber],
      dateOfBirth: [this.formatDateForInput(this.user.DateOfBirth)],
      gender: [this.user.Gender],
      address: [this.user.Address],
      city: [this.user.City],
      postalCode: [this.user.PostalCode],
      country: [this.user.Country],
      isActive: [this.user.IsActive],
    });

    const modal = this.modal.create({
      nzTitle: `Edit User: ${this.getFullName()}`,
      nzContent: this.getEditModalTemplate(editForm),
      nzWidth: 600,
      nzFooter: [
        {
          label: 'Cancel',
          onClick: () => modal.destroy(),
        },
        {
          label: 'Save Changes',
          type: 'primary',
          onClick: () => {
            if (editForm.valid) {
              this.saveUserChanges(editForm.value);
              modal.destroy();
            } else {
              this.message.error('Please fill in all required fields');
            }
          },
        },
      ],
    });
  }

  private getEditModalTemplate(form: any) {
    // This is a simplified approach. In a real app, you'd use a proper component
    return `
      <form style="max-height: 400px; overflow-y: auto;">
        <div style="margin-bottom: 16px;">
          <label>First Name *</label>
          <input nz-input placeholder="First Name" value="${form.get('firstName')?.value || ''}" />
        </div>
        <div style="margin-bottom: 16px;">
          <label>Last Name</label>
          <input nz-input placeholder="Last Name" value="${form.get('lastName')?.value || ''}" />
        </div>
        <div style="margin-bottom: 16px;">
          <label>Email</label>
          <input nz-input placeholder="Email" value="${form.get('email')?.value || ''}" />
        </div>
        <div style="margin-bottom: 16px;">
          <label>Phone Number</label>
          <input nz-input placeholder="Phone Number" value="${
            form.get('phoneNumber')?.value || ''
          }" />
        </div>
        <p style="color: #666; font-size: 12px;">Note: For a complete edit experience, consider implementing a dedicated form component.</p>
      </form>
    `;
  }

  private saveUserChanges(formValue: any) {
    const updates: UpdateUserRequest = {
      firstName: formValue.firstName?.trim() || null,
      lastName: formValue.lastName?.trim() || null,
      email: formValue.email?.trim() || null,
      phoneNumber: formValue.phoneNumber?.trim() || null,
      dateOfBirth: formValue.dateOfBirth || null,
      gender: formValue.gender || null,
      address: formValue.address?.trim() || null,
      city: formValue.city?.trim() || null,
      postalCode: formValue.postalCode?.trim() || null,
      country: formValue.country?.trim() || null,
      isActive: formValue.isActive ?? null,
    };

    // Remove null values
    Object.keys(updates).forEach((key) => {
      if (updates[key as keyof UpdateUserRequest] === null) {
        delete updates[key as keyof UpdateUserRequest];
      }
    });

    this.userUpdate.emit({ ...this.user, ...updates } as User);
  }

  confirmDelete() {
    this.modal.confirm({
      nzTitle: 'Delete User',
      nzContent: `Are you sure you want to delete ${this.getFullName()}? This action cannot be undone.`,
      nzOkText: 'Delete',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzOnOk: () => {
        this.userDelete.emit(this.user);
      },
    });
  }

  viewDetails() {
    this.modal.create({
      nzTitle: `User Details: ${this.getFullName()}`,
      nzContent: this.getDetailsModalTemplate(),
      nzWidth: 500,
      nzFooter: null,
    });
  }

  private getDetailsModalTemplate() {
    const displayEmail = this.user.Email || 'No email provided';
    const displayPhone = this.user.PhoneNumber || 'No phone provided';

    return `
      <div style="line-height: 1.8;">
        <p><strong>Name:</strong> ${this.getFullName()}</p>
        <p><strong>Email:</strong> ${displayEmail}</p>
        <p><strong>Phone:</strong> ${displayPhone}</p>
        <p><strong>Age:</strong> ${this.getAge()} years old</p>
        <p><strong>Status:</strong> ${this.user.IsActive ? 'Active' : 'Inactive'}</p>
        ${this.user.Gender ? `<p><strong>Gender:</strong> ${this.user.Gender}</p>` : ''}
        ${this.user.Address ? `<p><strong>Address:</strong> ${this.user.Address}</p>` : ''}
        ${this.user.City ? `<p><strong>City:</strong> ${this.user.City}</p>` : ''}
        ${this.user.Country ? `<p><strong>Country:</strong> ${this.user.Country}</p>` : ''}
        <p><strong>Created:</strong> ${new Date(this.user.CreatedAt).toLocaleDateString()}</p>
        <p><strong>Updated:</strong> ${new Date(this.user.UpdatedAt).toLocaleDateString()}</p>
      </div>
    `;
  }

  // Template references for card actions
  editAction: any;
  deleteAction: any;
  viewAction: any;
}
