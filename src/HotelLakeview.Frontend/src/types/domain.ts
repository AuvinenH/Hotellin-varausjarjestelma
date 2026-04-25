export const RoomCategory = {
  Economy: 1,
  Standard: 2,
  Superior: 3,
  JuniorSuite: 4,
  Suite: 5,
} as const;

export type RoomCategory = (typeof RoomCategory)[keyof typeof RoomCategory];

export const ReservationStatus = {
  Confirmed: 1,
  Cancelled: 2,
} as const;

export type ReservationStatus = (typeof ReservationStatus)[keyof typeof ReservationStatus];

export interface Customer {
  id: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  notes: string | null;
  createdAtUtc: string;
}

export interface Room {
  id: string;
  number: string;
  category: RoomCategory;
  maxGuests: number;
  basePricePerNight: number;
  description: string | null;
}

export interface Reservation {
  id: string;
  roomId: string;
  customerId: string;
  checkInDate: string;
  checkOutDate: string;
  guestCount: number;
  totalPrice: number;
  status: ReservationStatus;
  createdAtUtc: string;
}

export interface RoomImage {
  id: string;
  roomId: string;
  fileName: string;
  contentType: string;
  sizeBytes: number;
  uploadedAtUtc: string;
}

export interface OccupancyReport {
  startDate: string;
  endDate: string;
  reservedNights: number;
  totalRoomNights: number;
  occupancyRatePercent: number;
}

export interface MonthlyRevenue {
  year: number;
  month: number;
  revenue: number;
}

export interface PopularRoomType {
  category: RoomCategory;
  reservationCount: number;
  nightCount: number;
}

export interface CreateCustomerRequest {
  fullName: string;
  email: string;
  phoneNumber: string;
  notes?: string | null;
}

export interface UpdateCustomerRequest extends CreateCustomerRequest {}

export interface CreateRoomRequest {
  number: string;
  category: RoomCategory;
  maxGuests: number;
  basePricePerNight: number;
  description?: string | null;
}

export interface UpdateRoomRequest extends CreateRoomRequest {}

export interface AvailabilityRequest {
  checkInDate: string;
  checkOutDate: string;
  guestCount: number;
  category?: RoomCategory;
}

export interface CreateReservationRequest {
  roomId: string;
  customerId: string;
  checkInDate: string;
  checkOutDate: string;
  guestCount: number;
}

export interface UpdateReservationRequest {
  roomId: string;
  checkInDate: string;
  checkOutDate: string;
  guestCount: number;
}

export type StaffRole = "Receptionist" | "Manager";
