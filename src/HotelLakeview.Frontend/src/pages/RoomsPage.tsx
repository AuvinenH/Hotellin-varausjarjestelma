import { useMemo, useState } from "react";
import type { ChangeEvent, FormEvent } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { hotelApi } from "../api/hotelApi";
import { MessagePanel } from "../components/MessagePanel";
import { PageTitle } from "../components/PageTitle";
import { useI18n } from "../i18n";
import { RoomCategory, type Room, type RoomCategory as RoomCategoryType } from "../types/domain";
import { formatCurrency, roomCategoryLabel } from "../utils/format";
import { getErrorMessage } from "../utils/errors";

interface RoomForm {
  number: string;
  category: RoomCategoryType;
  maxGuests: number;
  basePricePerNight: number;
  description: string;
}

const initialRoomForm: RoomForm = {
  number: "",
  category: 1,
  maxGuests: 1,
  basePricePerNight: 79,
  description: "",
};

const today = new Date().toISOString().slice(0, 10);
const nextDay = new Date(Date.now() + 86400000).toISOString().slice(0, 10);

export function RoomsPage() {
  const { text, language } = useI18n();
  const queryClient = useQueryClient();
  const [editingRoom, setEditingRoom] = useState<Room | null>(null);
  const [roomForm, setRoomForm] = useState<RoomForm>(initialRoomForm);
  const [availabilityForm, setAvailabilityForm] = useState({
    checkInDate: today,
    checkOutDate: nextDay,
    guestCount: 1,
    category: "",
  });
  const [availableRooms, setAvailableRooms] = useState<Room[] | null>(null);
  const [selectedRoomId, setSelectedRoomId] = useState<string>("");
  const [message, setMessage] = useState<string | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const roomsQuery = useQuery({
    queryKey: ["rooms"],
    queryFn: () => hotelApi.getRooms(),
  });

  const roomImagesQuery = useQuery({
    queryKey: ["room-images", selectedRoomId],
    queryFn: () => hotelApi.getRoomImages(selectedRoomId),
    enabled: Boolean(selectedRoomId),
  });

  const saveMutation = useMutation({
    mutationFn: async () => {
      if (editingRoom) {
        await hotelApi.updateRoom(editingRoom.id, {
          number: roomForm.number,
          category: roomForm.category,
          maxGuests: roomForm.maxGuests,
          basePricePerNight: roomForm.basePricePerNight,
          description: roomForm.description || null,
        });

        return text.rooms.updatedSuccess;
      }

      await hotelApi.createRoom({
        number: roomForm.number,
        category: roomForm.category,
        maxGuests: roomForm.maxGuests,
        basePricePerNight: roomForm.basePricePerNight,
        description: roomForm.description || null,
      });

      return text.rooms.createdSuccess;
    },
    onSuccess: async (successMessage) => {
      setMessage(successMessage);
      setErrorMessage(null);
      setEditingRoom(null);
      setRoomForm(initialRoomForm);
      await queryClient.invalidateQueries({ queryKey: ["rooms"] });
    },
    onError: (error) => {
      setMessage(null);
      setErrorMessage(getErrorMessage(error, language));
    },
  });

  const deleteMutation = useMutation({
    mutationFn: (roomId: string) => hotelApi.deleteRoom(roomId),
    onSuccess: async () => {
      setMessage(text.rooms.deletedSuccess);
      setErrorMessage(null);
      await queryClient.invalidateQueries({ queryKey: ["rooms"] });
    },
    onError: (error) => {
      setMessage(null);
      setErrorMessage(getErrorMessage(error, language));
    },
  });

  const availabilityMutation = useMutation({
    mutationFn: () =>
      hotelApi.getAvailableRooms({
        checkInDate: availabilityForm.checkInDate,
        checkOutDate: availabilityForm.checkOutDate,
        guestCount: availabilityForm.guestCount,
        category: availabilityForm.category ? Number(availabilityForm.category) as RoomCategoryType : undefined,
      }),
    onSuccess: (rooms) => {
      setAvailableRooms(rooms);
      setMessage(text.rooms.foundRooms(rooms.length));
      setErrorMessage(null);
    },
    onError: (error) => {
      setAvailableRooms(null);
      setMessage(null);
      setErrorMessage(getErrorMessage(error, language));
    },
  });

  const uploadMutation = useMutation({
    mutationFn: (file: File) => {
      if (!selectedRoomId) {
        throw new Error(text.rooms.selectRoomBeforeUpload);
      }

      return hotelApi.uploadRoomImage(selectedRoomId, file);
    },
    onSuccess: async () => {
      setMessage(text.rooms.imageUploaded);
      setErrorMessage(null);
      await queryClient.invalidateQueries({ queryKey: ["room-images", selectedRoomId] });
    },
    onError: (error) => {
      setMessage(null);
      setErrorMessage(getErrorMessage(error, language));
    },
  });

  const deleteImageMutation = useMutation({
    mutationFn: (imageId: string) => {
      if (!selectedRoomId) {
        throw new Error(text.rooms.selectRoomBeforeDelete);
      }

      return hotelApi.deleteRoomImage(selectedRoomId, imageId);
    },
    onSuccess: async () => {
      setMessage(text.rooms.imageDeleted);
      setErrorMessage(null);
      await queryClient.invalidateQueries({ queryKey: ["room-images", selectedRoomId] });
    },
    onError: (error) => {
      setMessage(null);
      setErrorMessage(getErrorMessage(error, language));
    },
  });

  const roomLookup = useMemo(() => {
    return new Map((roomsQuery.data ?? []).map((room) => [room.id, room]));
  }, [roomsQuery.data]);

  function handleRoomSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    saveMutation.mutate();
  }

  function startEditing(room: Room) {
    setEditingRoom(room);
    setRoomForm({
      number: room.number,
      category: room.category,
      maxGuests: room.maxGuests,
      basePricePerNight: room.basePricePerNight,
      description: room.description ?? "",
    });
  }

  function clearRoomForm() {
    setEditingRoom(null);
    setRoomForm(initialRoomForm);
  }

  function handleImageInput(event: ChangeEvent<HTMLInputElement>) {
    const file = event.target.files?.[0];

    if (file) {
      uploadMutation.mutate(file);
      event.target.value = "";
    }
  }

  const categoryOptions: RoomCategoryType[] = [
    RoomCategory.Economy,
    RoomCategory.Standard,
    RoomCategory.Superior,
    RoomCategory.JuniorSuite,
    RoomCategory.Suite,
  ];

  return (
    <div className="stack-large">
      <PageTitle
        title={text.rooms.title}
        subtitle={text.rooms.subtitle}
      />

      {message ? <MessagePanel tone="success" message={message} /> : null}
      {errorMessage ? <MessagePanel tone="error" message={errorMessage} /> : null}

      <section className="panel split-panel">
        <form className="stack-medium" onSubmit={handleRoomSubmit}>
          <h3>{editingRoom ? text.rooms.editRoom : text.rooms.addRoom}</h3>

          <label>
            {text.rooms.roomNumber}
            <input
              required
              value={roomForm.number}
              onChange={(event) => setRoomForm((prev) => ({ ...prev, number: event.target.value }))}
            />
          </label>

          <label>
            {text.common.category}
            <select
              value={roomForm.category}
              onChange={(event) =>
                setRoomForm((prev) => ({ ...prev, category: Number(event.target.value) as RoomCategoryType }))
              }
            >
              {categoryOptions.map((option) => (
                <option key={option} value={option}>
                  {roomCategoryLabel(option, language)}
                </option>
              ))}
            </select>
          </label>

          <label>
            {text.rooms.maxGuests}
            <input
              min={1}
              max={10}
              required
              type="number"
              value={roomForm.maxGuests}
              onChange={(event) =>
                setRoomForm((prev) => ({ ...prev, maxGuests: Number(event.target.value) }))
              }
            />
          </label>

          <label>
            {text.rooms.basePricePerNight}
            <input
              min={1}
              step={0.01}
              required
              type="number"
              value={roomForm.basePricePerNight}
              onChange={(event) =>
                setRoomForm((prev) => ({ ...prev, basePricePerNight: Number(event.target.value) }))
              }
            />
          </label>

          <label>
            {text.rooms.description}
            <textarea
              rows={3}
              value={roomForm.description}
              onChange={(event) => setRoomForm((prev) => ({ ...prev, description: event.target.value }))}
            />
          </label>

          <div className="button-row">
            <button className="btn-primary" type="submit">
              {editingRoom ? text.common.saveChanges : text.rooms.createButton}
            </button>
            {editingRoom ? (
              <button className="btn-ghost" type="button" onClick={clearRoomForm}>
                {text.common.cancelEdit}
              </button>
            ) : null}
          </div>
        </form>

        <div>
          <h3>{text.rooms.listTitle}</h3>
          <table className="table">
            <thead>
              <tr>
                <th>{text.rooms.numberColumn}</th>
                <th>{text.common.category}</th>
                <th>{text.rooms.maxColumn}</th>
                <th>{text.rooms.priceColumn}</th>
                <th>{text.common.actions}</th>
              </tr>
            </thead>
            <tbody>
              {(roomsQuery.data ?? []).map((room) => (
                <tr key={room.id}>
                  <td>{room.number}</td>
                  <td>{roomCategoryLabel(room.category, language)}</td>
                  <td>{room.maxGuests}</td>
                  <td>{formatCurrency(room.basePricePerNight, language)}</td>
                  <td>
                    <div className="button-row compact">
                      <button className="btn-ghost" type="button" onClick={() => startEditing(room)}>
                        {text.rooms.editButton}
                      </button>
                      <button className="btn-ghost" type="button" onClick={() => setSelectedRoomId(room.id)}>
                        {text.rooms.imagesButton}
                      </button>
                      <button className="btn-danger" type="button" onClick={() => deleteMutation.mutate(room.id)}>
                        {text.rooms.deleteButton}
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </section>

      <section className="panel">
        <h3>{text.rooms.availabilityTitle}</h3>
        <div className="inline-filters">
          <label>
            {text.common.checkIn}
            <input
              type="date"
              value={availabilityForm.checkInDate}
              onChange={(event) =>
                setAvailabilityForm((prev) => ({ ...prev, checkInDate: event.target.value }))
              }
            />
          </label>

          <label>
            {text.common.checkOut}
            <input
              type="date"
              value={availabilityForm.checkOutDate}
              onChange={(event) =>
                setAvailabilityForm((prev) => ({ ...prev, checkOutDate: event.target.value }))
              }
            />
          </label>

          <label>
            {text.common.guestCount}
            <input
              min={1}
              type="number"
              value={availabilityForm.guestCount}
              onChange={(event) =>
                setAvailabilityForm((prev) => ({ ...prev, guestCount: Number(event.target.value) }))
              }
            />
          </label>

          <label>
            {text.common.category}
            <select
              value={availabilityForm.category}
              onChange={(event) =>
                setAvailabilityForm((prev) => ({ ...prev, category: event.target.value }))
              }
            >
              <option value="">{text.common.all}</option>
              {categoryOptions.map((option) => (
                <option key={option} value={option}>
                  {roomCategoryLabel(option, language)}
                </option>
              ))}
            </select>
          </label>

          <button className="btn-primary" type="button" onClick={() => availabilityMutation.mutate()}>
            {text.rooms.searchAvailableButton}
          </button>
        </div>

        {availableRooms ? (
          <div className="chips-wrap">
            {availableRooms.map((room) => (
              <span className="chip" key={room.id}>
                {room.number} / {roomCategoryLabel(room.category, language)} / {room.maxGuests} {language === "fi" ? "hlö" : "guests"}
              </span>
            ))}
            {availableRooms.length === 0 ? <span>{text.rooms.noAvailableRooms}</span> : null}
          </div>
        ) : null}
      </section>

      <section className="panel">
        <h3>{text.rooms.imagesTitle}</h3>
        <div className="inline-filters">
          <label>
            {text.rooms.selectedRoom}
            <select value={selectedRoomId} onChange={(event) => setSelectedRoomId(event.target.value)}>
              <option value="">{text.rooms.selectRoom}</option>
              {(roomsQuery.data ?? []).map((room) => (
                <option key={room.id} value={room.id}>
                  {room.number} - {roomCategoryLabel(room.category, language)}
                </option>
              ))}
            </select>
          </label>

          <label>
            {text.rooms.uploadImage}
            <input type="file" accept="image/*" onChange={handleImageInput} disabled={!selectedRoomId} />
          </label>
        </div>

        <div className="image-grid">
          {(roomImagesQuery.data ?? []).map((image) => (
            <article className="image-card" key={image.id}>
              <img
                src={hotelApi.getRoomImageFileUrl(image.roomId, image.id)}
                alt={image.fileName}
                loading="lazy"
              />
              <div className="image-meta">
                <div>{image.fileName}</div>
                <small>{Math.round(image.sizeBytes / 1024)} kB</small>
              </div>
              <button className="btn-danger" type="button" onClick={() => deleteImageMutation.mutate(image.id)}>
                {text.rooms.deleteImage}
              </button>
            </article>
          ))}

          {selectedRoomId && (roomImagesQuery.data ?? []).length === 0 ? (
            <p>{text.rooms.noImagesForRoom}</p>
          ) : null}

          {selectedRoomId && roomLookup.has(selectedRoomId) ? (
            <p className="muted-text">
              {text.rooms.imagesForRoom(roomLookup.get(selectedRoomId)?.number ?? "")}
            </p>
          ) : null}
        </div>
      </section>
    </div>
  );
}
