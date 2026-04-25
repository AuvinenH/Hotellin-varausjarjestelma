import { useMemo, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { addDays, endOfMonth, formatISO, startOfMonth } from "date-fns";
import { hotelApi } from "../api/hotelApi";
import { PageTitle } from "../components/PageTitle";
import { useI18n } from "../i18n";
import { ReservationStatus } from "../types/domain";
import { formatCompactNumber, formatCurrency, formatDate, roomCategoryLabel } from "../utils/format";

const today = new Date();
const todayIso = formatISO(today, { representation: "date" });
const tomorrowIso = formatISO(addDays(today, 1), { representation: "date" });

export function DashboardPage() {
  const { text, language } = useI18n();
  const [startDate, setStartDate] = useState(
    formatISO(startOfMonth(today), { representation: "date" }),
  );
  const [endDate, setEndDate] = useState(
    formatISO(endOfMonth(today), { representation: "date" }),
  );

  const reservationsQuery = useQuery({
    queryKey: ["reservations"],
    queryFn: () => hotelApi.getReservations(),
  });

  const roomsQuery = useQuery({
    queryKey: ["rooms"],
    queryFn: () => hotelApi.getRooms(),
  });

  const customersQuery = useQuery({
    queryKey: ["customers", "dashboard"],
    queryFn: () => hotelApi.getCustomers(),
  });

  const availableRoomsQuery = useQuery({
    queryKey: ["dashboard-available", todayIso, tomorrowIso],
    queryFn: () =>
      hotelApi.getAvailableRooms({
        checkInDate: todayIso,
        checkOutDate: tomorrowIso,
        guestCount: 1,
      }),
  });

  const occupancyQuery = useQuery({
    queryKey: ["occupancy", startDate, endDate],
    queryFn: () => hotelApi.getOccupancy(startDate, endDate),
    enabled: Boolean(startDate && endDate),
  });

  const monthlyRevenueQuery = useQuery({
    queryKey: ["monthly-revenue", startDate, endDate],
    queryFn: () => hotelApi.getMonthlyRevenue(startDate, endDate),
    enabled: Boolean(startDate && endDate),
  });

  const popularRoomTypesQuery = useQuery({
    queryKey: ["popular-room-types", startDate, endDate],
    queryFn: () => hotelApi.getPopularRoomTypes(startDate, endDate),
    enabled: Boolean(startDate && endDate),
  });

  const roomById = useMemo(() => {
    return new Map((roomsQuery.data ?? []).map((room) => [room.id, room]));
  }, [roomsQuery.data]);

  const customerById = useMemo(() => {
    return new Map((customersQuery.data ?? []).map((customer) => [customer.id, customer]));
  }, [customersQuery.data]);

  const todaysReservations = useMemo(() => {
    return (reservationsQuery.data ?? [])
      .filter((reservation) => {
        return (
          reservation.status === ReservationStatus.Confirmed
          && reservation.checkInDate <= todayIso
          && reservation.checkOutDate > todayIso
        );
      })
      .slice(0, 8);
  }, [reservationsQuery.data]);

  const totalRevenue = useMemo(() => {
    return (monthlyRevenueQuery.data ?? []).reduce((sum, item) => sum + item.revenue, 0);
  }, [monthlyRevenueQuery.data]);

  return (
    <div className="stack-large">
      <PageTitle
        title={text.dashboard.title}
        subtitle={text.dashboard.subtitle}
      />

      <section className="panel">
        <div className="inline-filters">
          <label>
            {text.common.start}
            <input
              type="date"
              value={startDate}
              onChange={(event) => setStartDate(event.target.value)}
            />
          </label>
          <label>
            {text.common.end}
            <input
              type="date"
              value={endDate}
              onChange={(event) => setEndDate(event.target.value)}
            />
          </label>
        </div>
      </section>

      <section className="metrics-grid">
        <article className="metric-card">
          <h3>{text.dashboard.occupancy}</h3>
          <p className="metric-value">
            {occupancyQuery.data ? `${occupancyQuery.data.occupancyRatePercent.toFixed(2)} %` : "-"}
          </p>
          <p className="metric-meta">
            {text.dashboard.reservedNights(formatCompactNumber(occupancyQuery.data?.reservedNights ?? 0, language))}
          </p>
        </article>

        <article className="metric-card">
          <h3>{text.dashboard.revenueInRange}</h3>
          <p className="metric-value">{formatCurrency(totalRevenue, language)}</p>
          <p className="metric-meta">{text.dashboard.monthRows((monthlyRevenueQuery.data ?? []).length)}</p>
        </article>

        <article className="metric-card">
          <h3>{text.dashboard.activeReservationsToday}</h3>
          <p className="metric-value">{todaysReservations.length}</p>
          <p className="metric-meta">{text.dashboard.presentGuestsToday}</p>
        </article>

        <article className="metric-card">
          <h3>{text.dashboard.availableRoomsToday}</h3>
          <p className="metric-value">{availableRoomsQuery.data?.length ?? "-"}</p>
          <p className="metric-meta">
            {text.dashboard.fetchedForRange(
              formatDate(today, language),
              formatDate(addDays(today, 1), language),
            )}
          </p>
        </article>
      </section>

      <section className="panel split-panel">
        <div>
          <h3>{text.dashboard.presentGuestsToday}</h3>
          <table className="table">
            <thead>
              <tr>
                <th>{text.common.customer}</th>
                <th>{text.common.room}</th>
                <th>{text.dashboard.checkOutColumn}</th>
              </tr>
            </thead>
            <tbody>
              {todaysReservations.length === 0 ? (
                <tr>
                  <td colSpan={3}>{text.dashboard.noActiveReservations}</td>
                </tr>
              ) : (
                todaysReservations.map((reservation) => (
                  <tr key={reservation.id}>
                    <td>{customerById.get(reservation.customerId)?.fullName ?? reservation.customerId}</td>
                    <td>{roomById.get(reservation.roomId)?.number ?? reservation.roomId}</td>
                    <td>{formatDate(reservation.checkOutDate, language)}</td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>

        <div>
          <h3>{text.dashboard.popularRoomTypes}</h3>
          <table className="table">
            <thead>
              <tr>
                <th>{text.common.type}</th>
                <th>{text.common.reservations}</th>
                <th>{text.common.nights}</th>
              </tr>
            </thead>
            <tbody>
              {(popularRoomTypesQuery.data ?? []).length === 0 ? (
                <tr>
                  <td colSpan={3}>{text.dashboard.noReportData}</td>
                </tr>
              ) : (
                (popularRoomTypesQuery.data ?? []).map((item) => (
                  <tr key={item.category}>
                    <td>{roomCategoryLabel(item.category, language)}</td>
                    <td>{item.reservationCount}</td>
                    <td>{item.nightCount}</td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </section>
    </div>
  );
}
