export interface Todo {
  Id: string;
  Title: string;
  Description: string;
  IsCompleted: boolean;
  CreatedAt: string;
  CompletedAt: string | null;
  DueDate: string | null;
  Priority: number;
  Category: string | null;
  UpdatedAt: string;
}
