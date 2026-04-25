import { differenceInCalendarDays, eachDayOfInterval, parseISO } from "date-fns";

const PEAK_MULTIPLIER = 1.3;

export function isPeakSeason(date: Date): boolean {
  const month = date.getMonth() + 1;
  const day = date.getDate();

  const isSummer = month >= 6 && month <= 8;
  const isChristmas = (month === 12 && day >= 20) || (month === 1 && day <= 6);

  return isSummer || isChristmas;
}

export function calculateReservationTotal(
  basePricePerNight: number,
  checkInDateIso: string,
  checkOutDateIso: string,
): number {
  if (!checkInDateIso || !checkOutDateIso || basePricePerNight <= 0) {
    return 0;
  }

  const checkInDate = parseISO(checkInDateIso);
  const checkOutDate = parseISO(checkOutDateIso);
  const nights = differenceInCalendarDays(checkOutDate, checkInDate);

  if (nights <= 0) {
    return 0;
  }

  const stayNights = eachDayOfInterval({
    start: checkInDate,
    end: new Date(checkOutDate.getFullYear(), checkOutDate.getMonth(), checkOutDate.getDate() - 1),
  });

  const total = stayNights.reduce((sum, currentNight) => {
    const nightlyRate = isPeakSeason(currentNight)
      ? basePricePerNight * PEAK_MULTIPLIER
      : basePricePerNight;

    return sum + nightlyRate;
  }, 0);

  return Math.round(total * 100) / 100;
}
