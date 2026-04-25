# Hotellin-varausjärjestelmä

Hotel Lakeview -hotellin varausjärjestelmä, jossa backend on toteutettu Clean Architecture -mallilla ja frontend henkilökunnan sisäiseksi käyttöliittymäksi.

## Tavoite

Järjestelmä tarjoaa vastaanotolle toimivan REST API:n huoneiden, asiakkaiden ja varausten hallintaan.
Ensimmäinen MVP-versio ratkaisee erityisesti päällekkäisvarausten eston, saatavuushaun ja automaattisen hinnanlaskennan.

## Toteutetut vaatimukset

- REST API huoneille, asiakkaille ja varauksille (CRUD)
- Päällekkäiset varaukset estetään tietokantatasolla
- Vapaat huoneet voidaan hakea aikavälille
- Kokonaishinta lasketaan automaattisesti yökohtaisesti
- Tiedot tallentuvat pysyvästi SQLite-tietokantaan
- API testattavissa Swaggerilla (OpenAPI)
- Selkeä kerrosarkkitehtuuri ja vastuunjako
- Kattava syötevalidointi (FluentValidation)
- Yhtenäinen virheenkäsittely (ProblemDetails)
- Varausten muokkaus ja peruutus
- Huonekuvien tallennus (upload/list/download/delete)
- Raportit: käyttöaste, kuukausitulot, suosituimmat huonetyypit
- Frontend vastaanotolle (Koontinäkymä, Varaukset, Asiakkaat, Huoneet, Raportit)
- Staff console -kielenvaihto (suomi/englanti)
- Xamk-tyylinen UI (valkoinen, keltainen, musta)
- Roolinäkymä frontendissa (Receptionist / Manager)
- 20 testiä (19 yksikkötestiä + 1 integraatio smoke -testi)

## Arkkitehtuuri (Clean Architecture)

Ratkaisu on jaettu seuraaviin kerroksiin:

- Domain
  - Entiteetit ja ydinliiketoimintasäännöt
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

Infrastructure toteuttaa Applicationin abstraction-rajapinnat.

## Projektirakenne

```text
src/
  HotelLakeview.Api/
  HotelLakeview.Application/
  HotelLakeview.Domain/
  HotelLakeview.Frontend/
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
- React + TypeScript + Vite
- TanStack Query
- date-fns

## Käynnistysohjeet

### Backend

1. Palauta paketit:

```bash
dotnet restore HotelLakeview.sln
```

2. Rakenna ratkaisu:

```bash
dotnet build HotelLakeview.sln
```

3. Käynnistä API:

```bash
dotnet run --project src/HotelLakeview.Api/HotelLakeview.Api.csproj
```

4. Avaa Swagger:

- http://localhost:5000/swagger
- tai portti, jonka sovellus tulostaa konsoliin

### Frontend (henkilökunnan sisäinen näkymä)

1. Siirry frontend-kansioon:

```bash
cd src/HotelLakeview.Frontend
```

2. Asenna riippuvuudet:

```bash
npm install
```

3. Käynnistä frontend:

```bash
npm run dev
```

4. Avaa selaimessa:

- http://localhost:5173

Vite dev server proxyaa /api-kutsut oletuksena osoitteeseen http://localhost:5178.
Tarvittaessa muuta arvoa tiedostossa src/HotelLakeview.Frontend/.env.example.

## Frontend-moduulit

- Koontinäkymä: päivän yhteenveto, käyttöaste, tulot, aktiiviset varaukset
- Varaukset: saatavuushaku, varauksen luonti, muokkaus, peruutus, automaattinen kokonaishinta
- Asiakkaat: luonti, haku, päivitys, poisto, lisätiedot (allergiat/erityistoiveet)
- Huoneet: CRUD, saatavuushaku, huonekuvien hallinta
- Raportit: käyttöaste, kuukausitulot, suosituimmat huonetyypit

## Turvallisuus huomio frontendissa

- Frontend on suunniteltu vain sisäkäyttöön tässä vaiheessa
- Manager-näkymä (raportit) on erotettu frontendin rooligatingilla
- Täysi tietoturvavaatimus (käyttäjäkohtainen auth + API authorisointi) vaatii backendin autentikoinnin viimeistelyn ennen tuotantokäyttöä

## Tietokanta ja tallennus

- Oletusyhteys: Data Source=hotel-lakeview.db
- Tietokanta luodaan automaattisesti ensimmäisellä käynnistyksellä
- Huonekuvat tallennetaan oletuksena hakemistoon: uploads/rooms

Asetuksia voi muuttaa tiedostoissa:

- src/HotelLakeview.Api/appsettings.json
- src/HotelLakeview.Api/appsettings.Development.json

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
- POST /api/rooms/{roomId}/images (multipart/form-data, kenttä: file)
- GET /api/rooms/{roomId}/images/{imageId}/file
- DELETE /api/rooms/{roomId}/images/{imageId}

### Raportit

- GET /api/reports/occupancy?startDate=2026-01-01&endDate=2026-01-31
- GET /api/reports/monthly-revenue?startDate=2026-01-01&endDate=2026-12-31
- GET /api/reports/popular-room-types?startDate=2026-01-01&endDate=2026-12-31

## Liiketoimintasäännöt

### Päällekkäisvarausten esto

Varausyön rivit tallennetaan tauluun ReservationNights.
Tietokannassa on uniikki indeksi yhdistelmälle (RoomId, NightDate).
Tämä estää päällekkäiset varaukset myös rinnakkaisissa pyynnöissä.

### Sesonkihinnoittelu

Perushintaan lisätään 30 % seuraavina ajanjaksoina:

- 1.6.-31.8.
- 20.12.-6.1.

Hinta lasketaan yökohtaisesti koko varausjaksolta.

### Validointi ja virheenkäsittely

- Syötteet validoidaan FluentValidationilla
- Virheet palautetaan ProblemDetails-muodossa
- Tyypilliset statukset: 400, 404, 409, 500

## Testaus

Suorita testit:

```bash
dotnet test HotelLakeview.sln
```

Nykyinen testimäärä:

- 20 testiä yhteensä
- 19 yksikkötestiä
- 1 integraatio smoke -testi
- Testatut osa-alueet: hinnoittelu, DateRange-säännöt, käyttöastelaskenta, varauksen tilakäyttäytyminen

## Huomio jatkokehitykseen

- Lisää integraatiotestit API-endpointeille ja tietokantakonflikteille
- Lisää autentikointi ja roolipohjainen authorisointi (Receptionist, Manager)
- Siirrä kuvatallennus objektitallennukseen tuotantokäyttöä varten
