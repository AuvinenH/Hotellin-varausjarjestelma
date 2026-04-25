import { useMemo, useState } from "react";
import type { FormEvent } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { hotelApi } from "../api/hotelApi";
import { MessagePanel } from "../components/MessagePanel";
import { PageTitle } from "../components/PageTitle";
import { useI18n } from "../i18n";
import { ReservationStatus } from "../types/domain";
import { formatCurrency, formatDate, reservationStatusLabel, roomCategoryLabel } from "../utils/format";
import { calculateReservationTotal } from "../utils/pricing";
import { getErrorMessage } from "../utils/errors";

interface ReservationForm {
  customerId: string;
  roomId: string;
  checkInDate: string;
  checkOutDate: string;
  guestCount: number;
}

const initialForm: ReservationForm = {
  customerId: "",
  roomId: "",
  checkInDate: new Date().toISOString().slice(0, 10),
  checkOutDate: new Date(Date.now() + 86400000).toISOString().slice(0, 10),
  guestCount: 1,
};

export function ReservationsPage() {
  const { text, language } = useI18n();
  const queryClient = useQueryClient();
  const [form, setForm] = useState<ReservationForm>(initialForm);
  const [editingReservationId, setEditingReservationId] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const customersQuery = useQuery({
    queryKey: ["customers", "reservation-form"],
    queryFn: () => hotelApi.getCustomers(),
  });

  const roomsQuery = useQuery({
    queryKey: ["rooms", "reservation-form"],
    queryFn: () => hotelApi.getRooms(),
  });

  const reservationsQuery = useQuery({
    queryKey: ["reservations"],
    queryFn: () => hotelApi.getReservations(),
  });

  const availableRoomsQuery = useQuery({
    queryKey: ["available-rooms", form.checkInDate, form.checkOutDate, form.guestCount],
    queryFn: () =>
      hotelApi.getAvailableRooms({
        checkInDate: form.checkInDate,
        checkOutDate: form.checkOutDate,
        guestCount: form.guestCount,
      }),
    enabled: Boolean(form.checkInDate && form.checkOutDate && form.guestCount > 0),
  });

  const saveMutation = useMutation({
    mutationFn: async () => {
      if (!form.customerId) {
        throw new Error(text.reservations.selectCustomerBeforeSave);
      }

      if (!form.roomId) {
        throw new Error(text.reservations.selectRoomBeforeSave);
      }

      if (editingReservationId) {
        await hotelApi.updateReservation(editingReservationId, {
          roomId: form.roomId,
          checkInDate: form.checkInDate,
          checkOutDate: form.checkOutDate,
          guestCount: form.guestCount,
        });
        return text.reservations.updatedSuccess;
      }

      await hotelApi.createReservation({
        customerId: form.customerId,
        roomId: form.roomId,
        checkInDate: form.checkInDate,
        checkOutDate: form.checkOutDate,
        guestCount: form.guestCount,
      });

      return text.reservations.createdSuccess;
    },
    onSuccess: async (successMessage) => {
      setMessage(successMessage);
      setErrorMessage(null);
      setForm(initialForm);
      setEditingReservationId(null);
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: ["reservations"] }),
        queryClient.invalidateQueries({ queryKey: ["available-rooms"] }),
      ]);
    },
    onError: (error) => {
      setMessage(null);
      setErrorMessage(getErrorMessage(error, language));
    },
  });

  const cancelMutation = useMutation({
    mutationFn: (reservationId: string) => hotelApi.cancelReservation(reservationId),
    onSuccess: async () => {
      setMessage(text.reservations.cancelledSuccess);
      setErrorMessage(null);
      await queryClient.invalidateQueries({ queryKey: ["reservations"] });
    },
    onError: (error) => {
      setMessage(null);
      setErrorMessage(getErrorMessage(error, language));
    },
  });

  const customerLookup = useMemo(() => {
    return new Map((customersQuery.data ?? []).map((customer) => [customer.id, customer]));
  }, [customersQuery.data]);

  const roomLookup = useMemo(() => {
    return new Map((roomsQuery.data ?? []).map((room) => [room.id, room]));
  }, [roomsQuery.data]);

  const availableRoomIds = useMemo(() => {
    return new Set((availableRoomsQuery.data ?? []).map((room) => room.id));
  }, [availableRoomsQuery.data]);

  const selectableRooms = useMemo(() => {
    if (editingReservationId) {
      return roomsQuery.data ?? [];
    }

    return availableRoomsQuery.data ?? [];
  }, [availableRoomsQuery.data, editingReservationId, roomsQuery.data]);

  const selectedRoom = roomLookup.get(form.roomId);
  const pricePreview = selectedRoom
    ? calculateReservationTotal(selectedRoom.basePricePerNight, form.checkInDate, form.checkOutDate)
    : 0;

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    saveMutation.mutate();
  }

  function startEditingReservation(reservationId: string) {
    const reservation = (reservationsQuery.data ?? []).find((item) => item.id === reservationId);

    if (!reservation) {
      return;
    }

    setEditingReservationId(reservation.id);
    setForm({
      customerId: reservation.customerId,
      roomId: reservation.roomId,
      checkInDate: reservation.checkInDate,
      checkOutDate: reservation.checkOutDate,
      guestCount: reservation.guestCount,
    });
    setMessage(null);
    setErrorMessage(null);
  }

  function resetForm() {
    setEditingReservationId(null);
    setForm(initialForm);
    setMessage(null);
    setErrorMessage(null);
  }

  return (
    <div className="stack-large">
      <PageTitle
        title={text.reservations.title}
        subtitle={text.reservations.subtitle}
      />

      {message ? <MessagePanel tone="success" message={message} /> : null}
      {errorMessage ? <MessagePanel tone="error" message={errorMessage} /> : null}

      <section className="panel split-panel">
        <form className="stack-medium" onSubmit={handleSubmit}>
          <h3>{editingReservationId ? text.reservations.editReservation : text.reservations.addReservation}</h3>

          <label>
            {text.common.customer}
            <select
              value={form.customerId}
              onChange={(event) => setForm((prev) => ({ ...prev, customerId: event.target.value }))}
              required
            >
              <option value="">{text.reservations.selectCustomer}</option>
              {(customersQuery.data ?? []).map((customer) => (
                <option key={customer.id} value={customer.id}>
                  {customer.fullName} ({customer.phoneNumber})
                </option>
              ))}
            </select>
          </label>

          <label>
            {text.common.checkIn}
            <input
              required
              type="date"
              value={form.checkInDate}
              onChange={(event) => setForm((prev) => ({ ...prev, checkInDate: event.target.value }))}
            />
          </label>

          <label>
            {text.common.checkOut}
            <input
              required
              type="date"
              value={form.checkOutDate}
              onChange={(event) => setForm((prev) => ({ ...prev, checkOutDate: event.target.value }))}
            />
          </label>

          <label>
            {text.common.guestCount}
            <input
              required
              min={1}
              max={10}
              type="number"
              value={form.guestCount}
              onChange={(event) => setForm((prev) => ({ ...prev, guestCount: Number(event.target.value) }))}
            />
          </label>

          <label>
            {text.common.room}
            <select
              value={form.roomId}
              onChange={(event) => setForm((prev) => ({ ...prev, roomId: event.target.value }))}
              required
            >
              <option value="">{text.reservations.selectRoom}</option>
              {selectableRooms.map((room) => (
                <option key={room.id} value={room.id}>
                  {room.number} - {roomCategoryLabel(room.category, language)} ({room.maxGuests} {language === "fi" ? "hlö" : "guests"})
                </option>
              ))}
            </select>
          </label>

          <div className="price-preview">
            <span>{text.reservations.totalPrice}</span>
            <strong>{formatCurrency(pricePreview, language)}</strong>
          </div>

          {!editingReservationId ? (
            <small className="muted-text">
              {text.reservations.createInfo}
            </small>
          ) : (
            <small className="muted-text">
              {text.reservations.editInfo}
            </small>
          )}

          <div className="button-row">
            <button className="btn-primary" type="submit">
              {editingReservationId ? text.common.saveChanges : text.reservations.createButton}
            </button>
            {editingReservationId ? (
              <button className="btn-ghost" type="button" onClick={resetForm}>
                {text.common.cancelEdit}
              </button>
            ) : null}
          </div>
        </form>

        <div>
          <h3>{text.reservations.currentReservations}</h3>
          <table className="table">
            <thead>
              <tr>
                <th>{text.common.customer}</th>
                <th>{text.common.room}</th>
                <th>{text.reservations.period}</th>
                <th>{text.rooms.priceColumn}</th>
                <th>{text.reservations.status}</th>
                <th>{text.common.actions}</th>
              </tr>
            </thead>
            <tbody>
              {(reservationsQuery.data ?? []).length === 0 ? (
                <tr>
                  <td colSpan={6}>{text.reservations.noReservations}</td>
                </tr>
              ) : (
                (reservationsQuery.data ?? []).map((reservation) => {
                  const customerName = customerLookup.get(reservation.customerId)?.fullName ?? reservation.customerId;
                  const room = roomLookup.get(reservation.roomId);
                  const isActive = reservation.status === ReservationStatus.Confirmed;
                  const isAvailableNow = availableRoomIds.has(reservation.roomId);

                  return (
                    <tr key={reservation.id}>
                      <td>{customerName}</td>
                      <td>{room?.number ?? reservation.roomId}</td>
                      <td>
                        {formatDate(reservation.checkInDate, language)} - {formatDate(reservation.checkOutDate, language)}
                        {!editingReservationId && isActive && !isAvailableNow ? (
                          <div>
                            <small className="muted-text">{text.reservations.roomBookedForRange}</small>
                          </div>
                        ) : null}
                      </td>
                      <td>{formatCurrency(reservation.totalPrice, language)}</td>
                      <td>{reservationStatusLabel(reservation.status, language)}</td>
                      <td>
                        <div className="button-row compact">
                          <button
                            className="btn-ghost"
                            type="button"
                            disabled={reservation.status !== ReservationStatus.Confirmed}
                            onClick={() => startEditingReservation(reservation.id)}
                          >
                            {text.reservations.editButton}
                          </button>
                          <button
                            className="btn-danger"
                            type="button"
                            disabled={reservation.status !== ReservationStatus.Confirmed}
                            onClick={() => cancelMutation.mutate(reservation.id)}
                          >
                            {text.reservations.cancelButton}
                          </button>
                        </div>
                      </td>
                    </tr>
                  );
                })
              )}
            </tbody>
          </table>
        </div>
      </section>
    </div>
  );
}
