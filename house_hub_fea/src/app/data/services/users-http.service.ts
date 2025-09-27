import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  User,
  CreateUserRequest,
  UpdateUserRequest,
  UsersResponse,
  UserResponse,
} from '../models/usersModel';

@Injectable({
  providedIn: 'root',
})
export class UsersService {
  private http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:5001'; // Updated to match Docker backend port

  constructor() {}

  /**
   * Get all users with optional OData query parameters
   * @param queryParams Optional OData query string (e.g., '$filter=isActive eq true&$orderby=firstName')
   */
  getUsers(queryParams?: string): Observable<UsersResponse> {
    const url = queryParams
      ? `${this.baseUrl}/odata/Users?${queryParams}`
      : `${this.baseUrl}/odata/Users`;

    return this.http.get<UsersResponse>(url);
  }

  /**
   * Get a specific user by ID
   * @param id User ID (GUID)
   */
  getUserById(id: string): Observable<UserResponse> {
    return this.http.get<UserResponse>(`${this.baseUrl}/odata/Users(${id})`);
  }

  /**
   * Get user by email address
   * @param email User email
   */
  getUserByEmail(email: string): Observable<User> {
    return this.http.get<User>(
      `${this.baseUrl}/odata/Users/GetByEmail?email=${encodeURIComponent(email)}`
    );
  }

  /**
   * Create a new user
   * @param userData User data to create
   */
  createUser(userData: CreateUserRequest): Observable<User> {
    return this.http.post<User>(`${this.baseUrl}/odata/Users`, userData);
  }

  /**
   * Update an existing user
   * @param id User ID to update
   * @param userData Updated user data
   */
  updateUser(id: string, userData: UpdateUserRequest): Observable<User> {
    return this.http.put<User>(`${this.baseUrl}/odata/Users(${id})`, userData);
  }

  /**
   * Delete a user
   * @param id User ID to delete
   */
  deleteUser(id: string): Observable<User> {
    return this.http.delete<User>(`${this.baseUrl}/odata/Users(${id})`);
  }

  // Additional helper methods for common use cases

  /**
   * Get active users only
   */
  getActiveUsers(): Observable<UsersResponse> {
    return this.getUsers('$filter=isActive eq true&$orderby=firstName');
  }

  /**
   * Get users by city
   * @param city City name to filter by
   */
  getUsersByCity(city: string): Observable<UsersResponse> {
    return this.getUsers(`$filter=city eq '${city}'`);
  }

  /**
   * Search users by name (first name or last name contains search term)
   * @param searchTerm Search term to look for in names
   */
  searchUsersByName(searchTerm: string): Observable<UsersResponse> {
    const filter = `$filter=contains(tolower(firstName), '${searchTerm.toLowerCase()}') or contains(tolower(lastName), '${searchTerm.toLowerCase()}')`;
    return this.getUsers(filter);
  }

  /**
   * Get paginated users
   * @param page Page number (0-based)
   * @param pageSize Number of items per page
   * @param orderBy Optional ordering (default: firstName)
   */
  getPaginatedUsers(
    page: number = 0,
    pageSize: number = 10,
    orderBy: string = 'firstName'
  ): Observable<UsersResponse> {
    const skip = page * pageSize;
    const queryParams = `$orderby=${orderBy}&$skip=${skip}&$top=${pageSize}&$count=true`;
    return this.getUsers(queryParams);
  }
}
