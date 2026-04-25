import { createContext, useContext, useEffect, useMemo, useState, type PropsWithChildren } from "react";
import type { StaffRole } from "./types/domain";

export type AppLanguage = "fi" | "en";

const languageStorageKey = "hotel-lakeview-language";

interface TranslationSet {
  appShell: {
    nav: {
      dashboard: string;
      reservations: string;
      customers: string;
      rooms: string;
      reports: string;
    };
    staffConsole: string;
    mainNavigation: string;
    internalOnly: string;
    managementViewTitle: string;
    roleLabel: string;
    languageLabel: string;
  };
  roles: Record<StaffRole, string>;
  common: {
    all: string;
    start: string;
    end: string;
    checkIn: string;
    checkOut: string;
    category: string;
    guestCount: string;
    actions: string;
    type: string;
    rooms: string;
    reservations: string;
    nights: string;
    customer: string;
    room: string;
    saveChanges: string;
    cancelEdit: string;
    loading: string;
    unknown: string;
  };
  dashboard: {
    title: string;
    subtitle: string;
    occupancy: string;
    reservedNights: (count: string) => string;
    revenueInRange: string;
    monthRows: (count: number) => string;
    activeReservationsToday: string;
    presentGuestsToday: string;
    availableRoomsToday: string;
    fetchedForRange: (from: string, to: string) => string;
    noActiveReservations: string;
    popularRoomTypes: string;
    noReportData: string;
    checkOutColumn: string;
  };
  customers: {
    title: string;
    subtitle: string;
    searchLabel: string;
    searchPlaceholder: string;
    editCustomer: string;
    addCustomer: string;
    name: string;
    email: string;
    phone: string;
    notes: string;
    createButton: string;
    listTitle: string;
    created: string;
    loading: string;
    noResults: string;
    editButton: string;
    deleteButton: string;
    updatedSuccess: string;
    createdSuccess: string;
    deletedSuccess: string;
  };
  rooms: {
    title: string;
    subtitle: string;
    editRoom: string;
    addRoom: string;
    roomNumber: string;
    maxGuests: string;
    basePricePerNight: string;
    description: string;
    createButton: string;
    listTitle: string;
    numberColumn: string;
    maxColumn: string;
    priceColumn: string;
    editButton: string;
    imagesButton: string;
    deleteButton: string;
    availabilityTitle: string;
    searchAvailableButton: string;
    foundRooms: (count: number) => string;
    noAvailableRooms: string;
    imagesTitle: string;
    selectedRoom: string;
    selectRoom: string;
    uploadImage: string;
    deleteImage: string;
    noImagesForRoom: string;
    imagesForRoom: (roomNumber: string) => string;
    updatedSuccess: string;
    createdSuccess: string;
    deletedSuccess: string;
    imageUploaded: string;
    imageDeleted: string;
    selectRoomBeforeUpload: string;
    selectRoomBeforeDelete: string;
  };
  reservations: {
    title: string;
    subtitle: string;
    editReservation: string;
    addReservation: string;
    selectCustomer: string;
    selectRoom: string;
    totalPrice: string;
    createInfo: string;
    editInfo: string;
    createButton: string;
    currentReservations: string;
    period: string;
    status: string;
    noReservations: string;
    roomBookedForRange: string;
    editButton: string;
    cancelButton: string;
    selectCustomerBeforeSave: string;
    selectRoomBeforeSave: string;
    updatedSuccess: string;
    createdSuccess: string;
    cancelledSuccess: string;
  };
  reports: {
    title: string;
    subtitle: string;
    totalRevenue: string;
    rowsCount: (count: number) => string;
    mostPopularRoomType: string;
    basedOnNights: string;
    monthlyRevenue: string;
    noRevenueData: string;
    popularRoomTypes: string;
    noRoomTypeData: string;
    nightsCount: (count: number) => string;
  };
  format: {
    unknownCategory: string;
    confirmed: string;
    cancelled: string;
    unknownStatus: string;
  };
  errors: {
    requestFailed: string;
    unknownError: string;
  };
}

const translations: Record<AppLanguage, TranslationSet> = {
  fi: {
    appShell: {
      nav: {
        dashboard: "Koontinäkymä",
        reservations: "Varaukset",
        customers: "Asiakkaat",
        rooms: "Huoneet",
        reports: "Raportit",
      },
      staffConsole: "Henkilökunnan konsoli",
      mainNavigation: "Päänavigaatio",
      internalOnly: "Vain sisäinen käyttö. Julkinen varausnäkymä toteutetaan myöhemmin.",
      managementViewTitle: "Vastaanoton hallintanäkymä",
      roleLabel: "Rooli",
      languageLabel: "Kieli",
    },
    roles: {
      Receptionist: "Vastaanottovirkailija",
      Manager: "Esihenkilö",
    },
    common: {
      all: "Kaikki",
      start: "Alku",
      end: "Loppu",
      checkIn: "Saapuminen",
      checkOut: "Lähtö",
      category: "Kategoria",
      guestCount: "Henkilömäärä",
      actions: "Toiminnot",
      type: "Tyyppi",
      rooms: "Huoneet",
      reservations: "Varaukset",
      nights: "Yöt",
      customer: "Asiakas",
      room: "Huone",
      saveChanges: "Tallenna muutokset",
      cancelEdit: "Peru muokkaus",
      loading: "Ladataan...",
      unknown: "Tuntematon",
    },
    dashboard: {
      title: "Koontinäkymä",
      subtitle: "Yhteenveto päivän tilanteesta ja valitun aikavälin tunnusluvuista.",
      occupancy: "Käyttöaste",
      reservedNights: (count) => `Varatut yöt ${count}`,
      revenueInRange: "Tulot valitulla välillä",
      monthRows: (count) => `Kuukausirivejä ${count}`,
      activeReservationsToday: "Päivän aktiiviset varaukset",
      presentGuestsToday: "Tänään paikalla olevat asiakkaat",
      availableRoomsToday: "Vapaat huoneet tänään",
      fetchedForRange: (from, to) => `Haettu välille ${from} - ${to}`,
      noActiveReservations: "Ei aktiivisia varauksia tänään.",
      popularRoomTypes: "Suosituimmat huonetyypit",
      noReportData: "Ei raporttidataa valitulla välillä.",
      checkOutColumn: "Lähtö",
    },
    customers: {
      title: "Asiakkaat",
      subtitle: "Luo ja hae asiakkaat nopeasti puhelinvaraustilanteissa.",
      searchLabel: "Haku nimellä, sähköpostilla tai puhelimella",
      searchPlaceholder: "Esim. Liisa Järvinen",
      editCustomer: "Päivitä asiakas",
      addCustomer: "Lisää uusi asiakas",
      name: "Nimi",
      email: "Sähköposti",
      phone: "Puhelinnumero",
      notes: "Lisätiedot (allergiat, erityistoiveet)",
      createButton: "Luo asiakas",
      listTitle: "Asiakaslista",
      created: "Luotu",
      loading: "Ladataan asiakkaita...",
      noResults: "Ei asiakkaita valituilla hakuehdoilla.",
      editButton: "Muokkaa",
      deleteButton: "Poista",
      updatedSuccess: "Asiakas päivitetty.",
      createdSuccess: "Asiakas lisätty.",
      deletedSuccess: "Asiakas poistettu.",
    },
    rooms: {
      title: "Huoneet",
      subtitle: "Hallinnoi huoneita, tarkista saatavuus ja ylläpidä huonekuvia.",
      editRoom: "Päivitä huone",
      addRoom: "Lisää huone",
      roomNumber: "Huonenumero",
      maxGuests: "Maksimihenkilömäärä",
      basePricePerNight: "Perushinta / yö",
      description: "Kuvaus",
      createButton: "Luo huone",
      listTitle: "Huonelista",
      numberColumn: "Nro",
      maxColumn: "Maksimi",
      priceColumn: "Hinta",
      editButton: "Muokkaa",
      imagesButton: "Kuvat",
      deleteButton: "Poista",
      availabilityTitle: "Vapaiden huoneiden haku",
      searchAvailableButton: "Hae vapaat",
      foundRooms: (count) => `Löytyi ${count} vapaata huonetta.`,
      noAvailableRooms: "Ei vapaita huoneita valitulle välille.",
      imagesTitle: "Huonekuvat",
      selectedRoom: "Valittu huone",
      selectRoom: "Valitse huone",
      uploadImage: "Lataa kuva",
      deleteImage: "Poista kuva",
      noImagesForRoom: "Ei kuvia valitulle huoneelle.",
      imagesForRoom: (roomNumber) => `Kuvat huoneelle ${roomNumber}`,
      updatedSuccess: "Huone päivitetty.",
      createdSuccess: "Huone lisätty.",
      deletedSuccess: "Huone poistettu.",
      imageUploaded: "Huonekuva ladattu.",
      imageDeleted: "Huonekuva poistettu.",
      selectRoomBeforeUpload: "Valitse huone ennen kuvan latausta.",
      selectRoomBeforeDelete: "Valitse huone ennen kuvan poistamista.",
    },
    reservations: {
      title: "Varaukset",
      subtitle: "Tarkista vapaat huoneet, luo varaus ja käsittele muokkaukset tai peruutukset.",
      editReservation: "Muokkaa varausta",
      addReservation: "Luo uusi varaus",
      selectCustomer: "Valitse asiakas",
      selectRoom: "Valitse huone",
      totalPrice: "Kokonaishinta (automaattinen)",
      createInfo: "Näytetään vain vapaat huoneet valitulle ajalle. Jos huone varataan rinnakkaisesti, backend palauttaa 409-konfliktin.",
      editInfo: "Muokkaustilassa voit valita minkä tahansa huoneen. Backend estää päällekkäiset varaukset automaattisesti.",
      createButton: "Luo varaus",
      currentReservations: "Nykyiset varaukset",
      period: "Jakso",
      status: "Tila",
      noReservations: "Ei varauksia.",
      roomBookedForRange: "Huone varattu valitulla välillä",
      editButton: "Muokkaa",
      cancelButton: "Peruuta",
      selectCustomerBeforeSave: "Valitse asiakas ennen varauksen tallennusta.",
      selectRoomBeforeSave: "Valitse huone ennen varauksen tallennusta.",
      updatedSuccess: "Varaus päivitetty.",
      createdSuccess: "Varaus luotu.",
      cancelledSuccess: "Varaus peruttu.",
    },
    reports: {
      title: "Raportit",
      subtitle: "Seuraa käyttöastetta, kuukausituloja ja suosituimpia huonetyyppejä.",
      totalRevenue: "Raportin kokonaistulot",
      rowsCount: (count) => `Rivimäärä ${count}`,
      mostPopularRoomType: "Suosituin huonetyyppi",
      basedOnNights: "Perustuu yöpymisten määrään",
      monthlyRevenue: "Kuukausitulot",
      noRevenueData: "Ei tulodataa valitulle välille.",
      popularRoomTypes: "Suosituimmat huonetyypit",
      noRoomTypeData: "Ei huonetyyppidataa valitulle välille.",
      nightsCount: (count) => `${count} yötä`,
    },
    format: {
      unknownCategory: "Tuntematon",
      confirmed: "Vahvistettu",
      cancelled: "Peruttu",
      unknownStatus: "Tuntematon",
    },
    errors: {
      requestFailed: "Pyyntö epäonnistui.",
      unknownError: "Tuntematon virhe.",
    },
  },
  en: {
    appShell: {
      nav: {
        dashboard: "Dashboard",
        reservations: "Reservations",
        customers: "Customers",
        rooms: "Rooms",
        reports: "Reports",
      },
      staffConsole: "Staff Console",
      mainNavigation: "Main navigation",
      internalOnly: "Internal use only. Public booking view will be added later.",
      managementViewTitle: "Reception management view",
      roleLabel: "Role",
      languageLabel: "Language",
    },
    roles: {
      Receptionist: "Receptionist",
      Manager: "Manager",
    },
    common: {
      all: "All",
      start: "Start",
      end: "End",
      checkIn: "Check-in",
      checkOut: "Check-out",
      category: "Category",
      guestCount: "Guest count",
      actions: "Actions",
      type: "Type",
      rooms: "Rooms",
      reservations: "Reservations",
      nights: "Nights",
      customer: "Customer",
      room: "Room",
      saveChanges: "Save changes",
      cancelEdit: "Cancel editing",
      loading: "Loading...",
      unknown: "Unknown",
    },
    dashboard: {
      title: "Dashboard",
      subtitle: "Summary of today and selected range KPIs.",
      occupancy: "Occupancy",
      reservedNights: (count) => `Booked nights ${count}`,
      revenueInRange: "Revenue in selected range",
      monthRows: (count) => `Monthly rows ${count}`,
      activeReservationsToday: "Active reservations today",
      presentGuestsToday: "Guests currently staying today",
      availableRoomsToday: "Available rooms today",
      fetchedForRange: (from, to) => `Fetched for ${from} - ${to}`,
      noActiveReservations: "No active reservations today.",
      popularRoomTypes: "Most popular room types",
      noReportData: "No report data for selected range.",
      checkOutColumn: "Check-out",
    },
    customers: {
      title: "Customers",
      subtitle: "Create and find customers quickly during phone bookings.",
      searchLabel: "Search by name, email, or phone",
      searchPlaceholder: "Example: Alice Johnson",
      editCustomer: "Update customer",
      addCustomer: "Add new customer",
      name: "Name",
      email: "Email",
      phone: "Phone",
      notes: "Notes (allergies, special requests)",
      createButton: "Create customer",
      listTitle: "Customer list",
      created: "Created",
      loading: "Loading customers...",
      noResults: "No customers match the selected search terms.",
      editButton: "Edit",
      deleteButton: "Delete",
      updatedSuccess: "Customer updated.",
      createdSuccess: "Customer added.",
      deletedSuccess: "Customer deleted.",
    },
    rooms: {
      title: "Rooms",
      subtitle: "Manage rooms, check availability, and maintain room images.",
      editRoom: "Update room",
      addRoom: "Add room",
      roomNumber: "Room number",
      maxGuests: "Maximum guests",
      basePricePerNight: "Base price / night",
      description: "Description",
      createButton: "Create room",
      listTitle: "Room list",
      numberColumn: "No",
      maxColumn: "Max",
      priceColumn: "Price",
      editButton: "Edit",
      imagesButton: "Images",
      deleteButton: "Delete",
      availabilityTitle: "Available room search",
      searchAvailableButton: "Search available",
      foundRooms: (count) => `Found ${count} available rooms.`,
      noAvailableRooms: "No available rooms for the selected range.",
      imagesTitle: "Room images",
      selectedRoom: "Selected room",
      selectRoom: "Select room",
      uploadImage: "Upload image",
      deleteImage: "Delete image",
      noImagesForRoom: "No images for the selected room.",
      imagesForRoom: (roomNumber) => `Images for room ${roomNumber}`,
      updatedSuccess: "Room updated.",
      createdSuccess: "Room added.",
      deletedSuccess: "Room deleted.",
      imageUploaded: "Room image uploaded.",
      imageDeleted: "Room image deleted.",
      selectRoomBeforeUpload: "Select a room before uploading an image.",
      selectRoomBeforeDelete: "Select a room before deleting an image.",
    },
    reservations: {
      title: "Reservations",
      subtitle: "Check available rooms, create reservations, and process edits or cancellations.",
      editReservation: "Edit reservation",
      addReservation: "Create new reservation",
      selectCustomer: "Select customer",
      selectRoom: "Select room",
      totalPrice: "Total price (automatic)",
      createInfo: "Only available rooms are shown for the selected range. If room is reserved concurrently, backend returns 409 conflict.",
      editInfo: "In edit mode you can choose any room. Backend automatically blocks overlapping reservations.",
      createButton: "Create reservation",
      currentReservations: "Current reservations",
      period: "Period",
      status: "Status",
      noReservations: "No reservations.",
      roomBookedForRange: "Room is booked for the selected range",
      editButton: "Edit",
      cancelButton: "Cancel",
      selectCustomerBeforeSave: "Select a customer before saving the reservation.",
      selectRoomBeforeSave: "Select a room before saving the reservation.",
      updatedSuccess: "Reservation updated.",
      createdSuccess: "Reservation created.",
      cancelledSuccess: "Reservation cancelled.",
    },
    reports: {
      title: "Reports",
      subtitle: "Track occupancy, monthly revenue, and most popular room types.",
      totalRevenue: "Total revenue in report",
      rowsCount: (count) => `Rows ${count}`,
      mostPopularRoomType: "Most popular room type",
      basedOnNights: "Based on number of nights",
      monthlyRevenue: "Monthly revenue",
      noRevenueData: "No revenue data for selected range.",
      popularRoomTypes: "Most popular room types",
      noRoomTypeData: "No room type data for selected range.",
      nightsCount: (count) => `${count} nights`,
    },
    format: {
      unknownCategory: "Unknown",
      confirmed: "Confirmed",
      cancelled: "Cancelled",
      unknownStatus: "Unknown",
    },
    errors: {
      requestFailed: "Request failed.",
      unknownError: "Unknown error.",
    },
  },
};

interface I18nContextValue {
  language: AppLanguage;
  setLanguage: (language: AppLanguage) => void;
  text: TranslationSet;
  roleLabel: (role: StaffRole) => string;
}

const I18nContext = createContext<I18nContextValue | undefined>(undefined);

function getStoredLanguage(): AppLanguage {
  return localStorage.getItem(languageStorageKey) === "en" ? "en" : "fi";
}

export function I18nProvider({ children }: PropsWithChildren) {
  const [language, setLanguage] = useState<AppLanguage>(getStoredLanguage);

  useEffect(() => {
    localStorage.setItem(languageStorageKey, language);
  }, [language]);

  const text = translations[language];

  const value = useMemo<I18nContextValue>(() => {
    return {
      language,
      setLanguage,
      text,
      roleLabel: (role) => text.roles[role],
    };
  }, [language, text]);

  return <I18nContext.Provider value={value}>{children}</I18nContext.Provider>;
}

export function useI18n(): I18nContextValue {
  const context = useContext(I18nContext);

  if (!context) {
    throw new Error("useI18n must be used inside I18nProvider.");
  }

  return context;
}
