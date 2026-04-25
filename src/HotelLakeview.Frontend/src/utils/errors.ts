import { ApiError } from "../api/httpClient";
import type { AppLanguage } from "../i18n";

export function getErrorMessage(error: unknown, language: AppLanguage = "fi"): string {
  if (error instanceof ApiError) {
    if (error.problem.errors) {
      const firstEntry = Object.values(error.problem.errors)[0];
      if (firstEntry && firstEntry.length > 0) {
        return firstEntry[0];
      }
    }

    return error.problem.detail || error.problem.title || (language === "fi" ? "Pyyntö epäonnistui." : "Request failed.");
  }

  if (error instanceof Error) {
    return error.message;
  }

  return language === "fi" ? "Tuntematon virhe." : "Unknown error.";
}
