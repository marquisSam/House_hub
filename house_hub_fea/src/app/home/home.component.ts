import { Component, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div class="home-container">
      <h1>{{ title() }}</h1>
      <p>Welcome to your family hub! Manage your household tasks and stay organized.</p>

      <div class="navigation-cards">
        <div class="card">
          <h2>Todo Management</h2>
          <p>Create, manage, and track your family's tasks and todos.</p>
          <a routerLink="/todos" class="btn-primary">Go to Todos</a>
        </div>

        <div class="card">
          <h2>Coming Soon</h2>
          <p>More family hub features are coming soon!</p>
          <button class="btn-secondary" disabled>Stay Tuned</button>
        </div>
      </div>
    </div>
  `,
  styles: [
    `
      .home-container {
        padding: 2rem;
        max-width: 1200px;
        margin: 0 auto;
        text-align: center;
      }

      h1 {
        color: #1f2937;
        font-size: 2.5rem;
        margin-bottom: 1rem;
      }

      p {
        color: #6b7280;
        font-size: 1.1rem;
        margin-bottom: 3rem;
      }

      .navigation-cards {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
        gap: 2rem;
        margin-top: 2rem;
      }

      .card {
        background: white;
        padding: 2rem;
        border-radius: 0.75rem;
        box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
        border: 1px solid #e5e7eb;
        transition: transform 0.2s ease, box-shadow 0.2s ease;
      }

      .card:hover {
        transform: translateY(-2px);
        box-shadow: 0 8px 15px rgba(0, 0, 0, 0.15);
      }

      .card h2 {
        color: #1f2937;
        font-size: 1.5rem;
        margin-bottom: 1rem;
      }

      .card p {
        color: #6b7280;
        margin-bottom: 1.5rem;
      }

      .btn-primary,
      .btn-secondary {
        display: inline-block;
        padding: 0.75rem 1.5rem;
        border-radius: 0.375rem;
        font-weight: 600;
        text-decoration: none;
        border: none;
        cursor: pointer;
        transition: background-color 0.2s ease;
      }

      .btn-primary {
        background-color: #3b82f6;
        color: white;
      }

      .btn-primary:hover {
        background-color: #2563eb;
      }

      .btn-secondary {
        background-color: #9ca3af;
        color: white;
      }

      .btn-secondary:disabled {
        cursor: not-allowed;
        opacity: 0.6;
      }

      @media (max-width: 768px) {
        .home-container {
          padding: 1rem;
        }

        .navigation-cards {
          grid-template-columns: 1fr;
        }
      }
    `,
  ],
})
export class HomeComponent {
  protected readonly title = signal('Le hub de la maison');
}
