import { format } from "date-fns";
import { enGB, fi } from "date-fns/locale";
import type { Locale } from "date-fns/locale";
import type { AppLanguage } from "../i18n";
import { ReservationStatus, RoomCategory } from "../types/domain";

const dateLocaleByLanguage: Record<AppLanguage, Locale> = {
  fi,
  en: enGB,
};

const currencyFormatterByLanguage: Record<AppLanguage, Intl.NumberFormat> = {
  fi: new Intl.NumberFormat("fi-FI", {
    style: "currency",
    currency: "EUR",
    maximumFractionDigits: 2,
  }),
  en: new Intl.NumberFormat("en-GB", {
    style: "currency",
    currency: "EUR",
    maximumFractionDigits: 2,
  }),
};

const compactNumberFormatterByLanguage: Record<AppLanguage, Intl.NumberFormat> = {
  fi: new Intl.NumberFormat("fi-FI", {
    maximumFractionDigits: 0,
  }),
  en: new Intl.NumberFormat("en-GB", {
    maximumFractionDigits: 0,
  }),
};

export function formatCurrency(value: number, language: AppLanguage = "fi"): string {
  return currencyFormatterByLanguage[language].format(value ?? 0);
}

export function formatCompactNumber(value: number, language: AppLanguage = "fi"): string {
  return compactNumberFormatterByLanguage[language].format(value ?? 0);
}

export function formatDate(value: string | Date, language: AppLanguage = "fi"): string {
  const dateValue = value instanceof Date ? value : new Date(value);
  const pattern = language === "fi" ? "dd.MM.yyyy" : "dd/MM/yyyy";
  return format(dateValue, pattern, { locale: dateLocaleByLanguage[language] });
}

export function roomCategoryLabel(category: RoomCategory, language: AppLanguage = "fi"): string {
  switch (category) {
    case RoomCategory.Economy:
      return "Economy";
    case RoomCategory.Standard:
      return "Standard";
    case RoomCategory.Superior:
      return "Superior";
    case RoomCategory.JuniorSuite:
      return language === "fi" ? "Juniorsviitti" : "Junior Suite";
    case RoomCategory.Suite:
      return language === "fi" ? "Sviitti" : "Suite";
    default:
      return language === "fi" ? "Tuntematon" : "Unknown";
  }
}

export function reservationStatusLabel(status: ReservationStatus, language: AppLanguage = "fi"): string {
  switch (status) {
    case ReservationStatus.Confirmed:
      return language === "fi" ? "Vahvistettu" : "Confirmed";
    case ReservationStatus.Cancelled:
      return language === "fi" ? "Peruttu" : "Cancelled";
    default:
      return language === "fi" ? "Tuntematon" : "Unknown";
  }
}
