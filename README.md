# Hotellin-varausjarjestelma

Hotel Lakeview -hotellin backend, toteutettu Clean Architecture -mallilla.

## Tavoite

Järjestelmä tarjoaa vastaanotolle toimivan REST API:n huoneiden, asiakkaiden ja varausten hallintaan.
Ensimmäinen MVP-versio ratkaisee erityisesti päällekkaisvarausten eston, saatavuushaun ja automaattisen hinnanlaskennan.

## Toteutetut vaatimukset

- REST API huoneille, asiakkaille ja varauksille (CRUD)
- Päällekkäiset varaukset estetään tietokantatasolla
- Vapaat huoneet voidaan hakea aikavälille
- Kokonaishinta lasketaan automaattisesti yökohtaisesti
- Tiedot tallentuvat pysyvästi SQLite-tietokantaan
- API testattavissa Swaggerilla (OpenAPI)
- Selkeä kerrosarkkitehtuuri ja vastuunjako
- Kattava syötevalidointi (FluentValidation)
- Yhtenäinen virheenkasittely (ProblemDetails)
- Varausten muokkaus ja peruutus
- Huonekuvien tallennus (upload/list/download/delete)
- Raportit: kayttoaste, kuukausitulot, suosituimmat huonetyypit
- 19 yksikkotestiä keskeiselle liiketoimintalogiikalle

## Arkkitehtuuri (Clean Architecture)

Ratkaisu on jaettu seuraaviin kerroksiin:

- Domain
	- Entiteetit ja ydinliiketoimintasaannot
	- Esim. Reservation, Room, Customer, DateRange
- Application
	- Use case -palvelut, rajapinnat, DTO:t ja validointi
	- Ei suoraa riippuvuutta infrastruktuurin toteutuksiin
- Infrastructure
	- EF Core + SQLite, repositoryt, tiedostotallennus
	- Toteuttaa Application-kerroksen rajapinnat
- API
	- Controllerit, middleware, Swagger, DI-kokoonpano

Riippuvuussuunta:

API -> Application -> Domain

Infrastructure toteuttaa Applicationin abstractions-rajapinnat.

## Projektirakenne

```text
src/
	HotelLakeview.Api/
	HotelLakeview.Application/
	HotelLakeview.Domain/
	HotelLakeview.Infrastructure/
tests/
	HotelLakeview.UnitTests/
	HotelLakeview.IntegrationTests/
```

## Teknologiat

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core 8
- SQLite
- FluentValidation
- Swagger / OpenAPI
- xUnit

## Kaynnistysohjeet

1. Palauta paketit:

```bash
dotnet restore HotelLakeview.sln
```

2. Rakenna ratkaisu:

```bash
dotnet build HotelLakeview.sln
```

3. Kaynnista API:

```bash
dotnet run --project src/HotelLakeview.Api/HotelLakeview.Api.csproj
```

4. Avaa Swagger:

- http://localhost:5000/swagger
- tai portti, jonka sovellus tulostaa konsoliin

## Tietokanta ja tallennus

- Oletusyhteys: `Data Source=hotel-lakeview.db`
- Tietokanta luodaan automaattisesti ensimmaisellä kaynnistyksellä
- Huonekuvat tallennetaan oletuksena hakemistoon: `uploads/rooms`

Asetuksia voi muuttaa tiedostoissa:

- `src/HotelLakeview.Api/appsettings.json`
- `src/HotelLakeview.Api/appsettings.Development.json`

## Keskeiset endpointit

### Asiakkaat

- GET /api/customers
- GET /api/customers/{id}
- POST /api/customers
- PUT /api/customers/{id}
- DELETE /api/customers/{id}

### Huoneet

- GET /api/rooms
- GET /api/rooms/{id}
- GET /api/rooms/available?checkInDate=2026-07-01&checkOutDate=2026-07-05&guestCount=2
- POST /api/rooms
- PUT /api/rooms/{id}
- DELETE /api/rooms/{id}

### Varaukset

- GET /api/reservations
- GET /api/reservations/{id}
- POST /api/reservations
- PUT /api/reservations/{id}
- DELETE /api/reservations/{id} (peruutus)

### Huonekuvat

- GET /api/rooms/{roomId}/images
- POST /api/rooms/{roomId}/images (multipart/form-data, kentta: file)
- GET /api/rooms/{roomId}/images/{imageId}/file
- DELETE /api/rooms/{roomId}/images/{imageId}

### Raportit

- GET /api/reports/occupancy?startDate=2026-01-01&endDate=2026-01-31
- GET /api/reports/monthly-revenue?startDate=2026-01-01&endDate=2026-12-31
- GET /api/reports/popular-room-types?startDate=2026-01-01&endDate=2026-12-31

## Liiketoimintasaannot

### Paallekkaisvarausten esto

Varausyön rivit tallennetaan tauluun `ReservationNights`.
Tietokannassa on uniikki indeksi yhdistelmälle `(RoomId, NightDate)`.
Tämä estää päällekkaiset varaukset myos rinnakkaisissa pyynnöissä.

### Sesonkihinnoittelu

Perushintaan lisätään 30 % seuraavina ajanjaksoina:

- 1.6.-31.8.
- 20.12.-6.1.

Hinta lasketaan yökohtaisesti koko varausjaksolta.

### Validointi ja virheenkasittely

- Syotteet validoidaan FluentValidationilla
- Virheet palautetaan ProblemDetails-muodossa
- Tyypilliset statukset: 400, 404, 409, 500

## Testaus

Suorita testit:

```bash
dotnet test HotelLakeview.sln
```

Nykyinen testimaara:

- 19 yksikkötestiä
- Testatut osa-alueet: hinnoittelu, DateRange-säännöt, kayttöastelaskenta, varauksen tilakäyttäytyminen

## Huomio jatkokehitykseen

- Lisää integraatiotestit API-endpointeille ja tietokantakonflikteille
- Lisää autentikointi ja roolipohjainen authorisointi (Receptionist, Manager)
- Siirrä kuvatallennus objektitallennukseen tuotantokäyttöa varten
