export interface Todo {
  id: number;
  title: string;
  description: string;
  dueDate: Date;
  priority: number;
  category: string;
  isCompleted: boolean;
}
