// User model interfaces for the frontend
// Based on the backend User entity and request/response DTOs

export interface User {
  Id: string; // Guid from backend
  FirstName: string;
  LastName: string;
  Email?: string | null;
  PhoneNumber?: string | null;
  DateOfBirth?: string | null; // ISO date string
  Gender?: string | null;
  Address?: string | null;
  City?: string | null;
  PostalCode?: string | null;
  Country?: string | null;
  IsActive: boolean;
  CreatedAt: string; // ISO date string
  UpdatedAt: string; // ISO date string
}

export interface CreateUserRequest {
  firstName: string;
  lastName?: string | null;
  email?: string | null;
  phoneNumber?: string | null;
  dateOfBirth?: string | null; // ISO date string
  gender?: string | null;
  address?: string | null;
  city?: string | null;
  postalCode?: string | null;
  country?: string | null;
}

export interface UpdateUserRequest {
  firstName?: string | null;
  lastName?: string | null;
  email?: string | null;
  phoneNumber?: string | null;
  dateOfBirth?: string | null; // ISO date string
  gender?: string | null;
  address?: string | null;
  city?: string | null;
  postalCode?: string | null;
  country?: string | null;
  isActive?: boolean | null;
}

// OData response wrapper for single user
export interface UserResponse {
  '@odata.context': string;
  value: User;
}

// OData response wrapper for user collections
export interface UsersResponse {
  '@odata.context': string;
  value: User[];
  '@odata.count'?: number;
  '@odata.nextLink'?: string;
}

// Utility types for user operations
export interface UserUpdate {
  id: string;
  updates: Partial<User>;
}

// User display helpers
export interface UserDisplayInfo {
  fullName: string;
  displayEmail: string;
  displayPhone: string;
  age?: number;
  isAdult: boolean;
}
