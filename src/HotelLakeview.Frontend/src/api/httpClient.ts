import type { ProblemDetails } from "../types/problemDetails";

export class ApiError extends Error {
  public readonly status: number;

  public readonly problem: ProblemDetails;

  constructor(status: number, problem: ProblemDetails) {
    super(problem.detail || problem.title || "API request failed.");
    this.name = "ApiError";
    this.status = status;
    this.problem = problem;
  }
}

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || "";

async function parseJsonSafe<T>(response: Response): Promise<T | undefined> {
  const contentType = response.headers.get("content-type");

  if (contentType?.includes("application/json")) {
    return (await response.json()) as T;
  }

  return undefined;
}

export async function requestJson<T>(
  path: string,
  init?: RequestInit,
): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...init,
    headers: {
      Accept: "application/json",
      ...(init?.headers ?? {}),
    },
  });

  if (!response.ok) {
    const problem = (await parseJsonSafe<ProblemDetails>(response)) || {
      status: response.status,
      title: "Request failed",
      detail: `Request to ${path} failed with status ${response.status}.`,
    };

    throw new ApiError(response.status, problem);
  }

  const data = await parseJsonSafe<T>(response);

  return data as T;
}

export async function requestNoContent(path: string, init?: RequestInit): Promise<void> {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...init,
    headers: {
      Accept: "application/json",
      ...(init?.headers ?? {}),
    },
  });

  if (!response.ok) {
    const problem = (await parseJsonSafe<ProblemDetails>(response)) || {
      status: response.status,
      title: "Request failed",
      detail: `Request to ${path} failed with status ${response.status}.`,
    };

    throw new ApiError(response.status, problem);
  }
}
