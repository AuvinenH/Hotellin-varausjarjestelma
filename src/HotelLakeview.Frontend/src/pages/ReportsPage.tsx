import { useMemo, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { endOfMonth, formatISO, startOfMonth } from "date-fns";
import { hotelApi } from "../api/hotelApi";
import { PageTitle } from "../components/PageTitle";
import { useI18n } from "../i18n";
import { formatCompactNumber, formatCurrency, roomCategoryLabel } from "../utils/format";

const today = new Date();

export function ReportsPage() {
  const { text, language } = useI18n();
  const [startDate, setStartDate] = useState(
    formatISO(startOfMonth(today), { representation: "date" }),
  );
  const [endDate, setEndDate] = useState(
    formatISO(endOfMonth(today), { representation: "date" }),
  );

  const occupancyQuery = useQuery({
    queryKey: ["report-occupancy", startDate, endDate],
    queryFn: () => hotelApi.getOccupancy(startDate, endDate),
    enabled: Boolean(startDate && endDate),
  });

  const revenueQuery = useQuery({
    queryKey: ["report-revenue", startDate, endDate],
    queryFn: () => hotelApi.getMonthlyRevenue(startDate, endDate),
    enabled: Boolean(startDate && endDate),
  });

  const popularRoomsQuery = useQuery({
    queryKey: ["report-popular-rooms", startDate, endDate],
    queryFn: () => hotelApi.getPopularRoomTypes(startDate, endDate),
    enabled: Boolean(startDate && endDate),
  });

  const maxRevenue = useMemo(() => {
    return Math.max(...(revenueQuery.data ?? []).map((item) => item.revenue), 1);
  }, [revenueQuery.data]);

  const maxNightCount = useMemo(() => {
    return Math.max(...(popularRoomsQuery.data ?? []).map((item) => item.nightCount), 1);
  }, [popularRoomsQuery.data]);

  return (
    <div className="stack-large">
      <PageTitle
        title={text.reports.title}
        subtitle={text.reports.subtitle}
      />

      <section className="panel">
        <div className="inline-filters">
          <label>
            {text.common.start}
            <input type="date" value={startDate} onChange={(event) => setStartDate(event.target.value)} />
          </label>
          <label>
            {text.common.end}
            <input type="date" value={endDate} onChange={(event) => setEndDate(event.target.value)} />
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
            {formatCompactNumber(occupancyQuery.data?.reservedNights ?? 0, language)} / {formatCompactNumber(occupancyQuery.data?.totalRoomNights ?? 0, language)} {language === "fi" ? "huoneyötä" : "room nights"}
          </p>
        </article>

        <article className="metric-card">
          <h3>{text.reports.totalRevenue}</h3>
          <p className="metric-value">
            {formatCurrency((revenueQuery.data ?? []).reduce((sum, row) => sum + row.revenue, 0), language)}
          </p>
          <p className="metric-meta">{text.reports.rowsCount((revenueQuery.data ?? []).length)}</p>
        </article>

        <article className="metric-card">
          <h3>{text.reports.mostPopularRoomType}</h3>
          <p className="metric-value">
            {(popularRoomsQuery.data ?? [])[0]
              ? roomCategoryLabel((popularRoomsQuery.data ?? [])[0].category, language)
              : "-"}
          </p>
          <p className="metric-meta">{text.reports.basedOnNights}</p>
        </article>
      </section>

      <section className="panel split-panel">
        <div>
          <h3>{text.reports.monthlyRevenue}</h3>
          <div className="bar-list">
            {(revenueQuery.data ?? []).map((row) => (
              <div className="bar-row" key={`${row.year}-${row.month}`}>
                <span>{`${row.month}.${row.year}`}</span>
                <div className="bar-track">
                  <div
                    className="bar-fill"
                    style={{ width: `${Math.max((row.revenue / maxRevenue) * 100, 4)}%` }}
                  />
                </div>
                <strong>{formatCurrency(row.revenue, language)}</strong>
              </div>
            ))}
            {(revenueQuery.data ?? []).length === 0 ? <p>{text.reports.noRevenueData}</p> : null}
          </div>
        </div>

        <div>
          <h3>{text.reports.popularRoomTypes}</h3>
          <div className="bar-list">
            {(popularRoomsQuery.data ?? []).map((row) => (
              <div className="bar-row" key={row.category}>
                <span>{roomCategoryLabel(row.category, language)}</span>
                <div className="bar-track">
                  <div
                    className="bar-fill alt"
                    style={{ width: `${Math.max((row.nightCount / maxNightCount) * 100, 4)}%` }}
                  />
                </div>
                <strong>{text.reports.nightsCount(row.nightCount)}</strong>
              </div>
            ))}
            {(popularRoomsQuery.data ?? []).length === 0 ? <p>{text.reports.noRoomTypeData}</p> : null}
          </div>
        </div>
      </section>
    </div>
  );
}
