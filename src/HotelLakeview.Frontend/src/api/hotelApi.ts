import type {
  AvailabilityRequest,
  CreateCustomerRequest,
  CreateReservationRequest,
  CreateRoomRequest,
  Customer,
  MonthlyRevenue,
  OccupancyReport,
  PopularRoomType,
  Reservation,
  Room,
  RoomImage,
  RoomCategory,
  UpdateCustomerRequest,
  UpdateReservationRequest,
  UpdateRoomRequest,
} from "../types/domain";
import { requestJson, requestNoContent } from "./httpClient";

function toQueryString(params: Record<string, string | number | undefined>): string {
  const query = new URLSearchParams();

  Object.entries(params).forEach(([key, value]) => {
    if (value !== undefined && value !== null && value !== "") {
      query.append(key, String(value));
    }
  });

  const queryString = query.toString();
  return queryString ? `?${queryString}` : "";
}

export const hotelApi = {
  getCustomers(search?: string): Promise<Customer[]> {
    const query = toQueryString({ search });
    return requestJson<Customer[]>(`/api/customers${query}`);
  },

  createCustomer(payload: CreateCustomerRequest): Promise<Customer> {
    return requestJson<Customer>("/api/customers", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload),
    });
  },

  updateCustomer(id: string, payload: UpdateCustomerRequest): Promise<Customer> {
    return requestJson<Customer>(`/api/customers/${id}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload),
    });
  },

  deleteCustomer(id: string): Promise<void> {
    return requestNoContent(`/api/customers/${id}`, { method: "DELETE" });
  },

  getRooms(): Promise<Room[]> {
    return requestJson<Room[]>("/api/rooms");
  },

  createRoom(payload: CreateRoomRequest): Promise<Room> {
    return requestJson<Room>("/api/rooms", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload),
    });
  },

  updateRoom(id: string, payload: UpdateRoomRequest): Promise<Room> {
    return requestJson<Room>(`/api/rooms/${id}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload),
    });
  },

  deleteRoom(id: string): Promise<void> {
    return requestNoContent(`/api/rooms/${id}`, { method: "DELETE" });
  },

  getAvailableRooms(params: AvailabilityRequest): Promise<Room[]> {
    const query = toQueryString({
      checkInDate: params.checkInDate,
      checkOutDate: params.checkOutDate,
      guestCount: params.guestCount,
      category: params.category,
    });

    return requestJson<Room[]>(`/api/rooms/available${query}`);
  },

  getReservations(): Promise<Reservation[]> {
    return requestJson<Reservation[]>("/api/reservations");
  },

  createReservation(payload: CreateReservationRequest): Promise<Reservation> {
    return requestJson<Reservation>("/api/reservations", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload),
    });
  },

  updateReservation(id: string, payload: UpdateReservationRequest): Promise<Reservation> {
    return requestJson<Reservation>(`/api/reservations/${id}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload),
    });
  },

  cancelReservation(id: string): Promise<void> {
    return requestNoContent(`/api/reservations/${id}`, { method: "DELETE" });
  },

  getOccupancy(startDate: string, endDate: string): Promise<OccupancyReport> {
    const query = toQueryString({ startDate, endDate });
    return requestJson<OccupancyReport>(`/api/reports/occupancy${query}`);
  },

  getMonthlyRevenue(startDate: string, endDate: string): Promise<MonthlyRevenue[]> {
    const query = toQueryString({ startDate, endDate });
    return requestJson<MonthlyRevenue[]>(`/api/reports/monthly-revenue${query}`);
  },

  getPopularRoomTypes(startDate: string, endDate: string): Promise<PopularRoomType[]> {
    const query = toQueryString({ startDate, endDate });
    return requestJson<PopularRoomType[]>(`/api/reports/popular-room-types${query}`);
  },

  getRoomImages(roomId: string): Promise<RoomImage[]> {
    return requestJson<RoomImage[]>(`/api/rooms/${roomId}/images`);
  },

  uploadRoomImage(roomId: string, file: File): Promise<RoomImage> {
    const formData = new FormData();
    formData.append("file", file);

    return requestJson<RoomImage>(`/api/rooms/${roomId}/images`, {
      method: "POST",
      body: formData,
    });
  },

  deleteRoomImage(roomId: string, imageId: string): Promise<void> {
    return requestNoContent(`/api/rooms/${roomId}/images/${imageId}`, {
      method: "DELETE",
    });
  },

  getRoomImageFileUrl(roomId: string, imageId: string): string {
    return `${import.meta.env.VITE_API_BASE_URL || ""}/api/rooms/${roomId}/images/${imageId}/file`;
  },

  roomCategoryOptions(): Array<{ value: RoomCategory; label: string }> {
    return [
      { value: 1 as RoomCategory, label: "Economy" },
      { value: 2 as RoomCategory, label: "Standard" },
      { value: 3 as RoomCategory, label: "Superior" },
      { value: 4 as RoomCategory, label: "Junior Suite" },
      { value: 5 as RoomCategory, label: "Suite" },
    ];
  },
};
