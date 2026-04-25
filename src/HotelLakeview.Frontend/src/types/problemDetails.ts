export interface ValidationErrors {
  [fieldName: string]: string[];
}

export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  errors?: ValidationErrors;
}
