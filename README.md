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
- Result pattern odotettuihin liiketoimintavirheisiin
- CQRS + MediatR (command/query handlerit)
- Varausten muokkaus ja peruutus
- Huonekuvien tallennus (upload/list/download/delete)
- Raportit: käyttöaste, kuukausitulot, suosituimmat huonetyypit
- Health check endpoint: /health
- Frontend vastaanotolle (Koontinäkymä, Varaukset, Asiakkaat, Huoneet, Raportit)
- Staff console -kielenvaihto (suomi/englanti)
- Xamk-tyylinen UI (valkoinen, keltainen, musta)
- Roolinäkymä frontendissa (Receptionist / Manager)
- 33 testiä (32 yksikkötestiä + 1 integraatio smoke -testi)

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
- MediatR (CQRS)
- Swagger / OpenAPI
- xUnit
- Moq
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

## Docker-käyttö

Sovellus on ajettavissa kokonaan Dockerilla (API + frontend) tiedostolla `docker-compose.yml`.

### Esivaatimukset

- Docker Desktop (tai Docker Engine + Compose v2)

### Käynnistys

Suorita projektin juuresta:

```bash
docker compose up --build
```

Palvelut:

- Frontend: http://localhost:8080
- API (suora): http://localhost:5178/swagger

### Pysäytys

```bash
docker compose down
```

Jos haluat poistaa myös datavoluumit (SQLite + uploads):

```bash
docker compose down -v
```

### Mitä compose tekee

- `api` service:
  - Buildaa imagensa tiedostosta `src/HotelLakeview.Api/Dockerfile`
  - Ajaa API:n portissa `8080` (hostilta `5178`)
  - Tallentaa SQLite-datan volumeen `api_db`
  - Tallentaa kuvat volumeen `api_uploads`

- `frontend` service:
  - Buildaa imagensa tiedostosta `src/HotelLakeview.Frontend/Dockerfile`
  - Ajaa Nginxissa portissa `80` (hostilta `8080`)
  - Proxytaa `/api/*` kutsut API-containeriin, joten selain käyttää samaa originia

## Azure deployment (Bicep)

Repository sisältää valmiin Bicep-pohjan infrastruktuurille:

- bicep/main.bicep
- bicep/main.parameters.example.json

Malli luo seuraavat resurssit:

- App Service Plan
- App Service (API)
- Storage Account (static website frontendille)
- Blob container huonekuville (oletus: roomimages)
- API app settings + connection string (SQLite polku App Servicessä)

### 1. Esivaatimukset

- Azure CLI asennettuna
- Kirjautuminen: az login
- Resource group olemassa (esim. rg-hotel-lakeview)

### 2. Deployaa infrastruktuuri Bicepillä

Kopioi ensin parametriesimerkki omaksi tiedostoksi:

```bash
copy bicep\\main.parameters.example.json bicep\\main.parameters.json
```

Päivitä arvot tiedostoon bicep/main.parameters.json (nimet, region, origin).
Huom: frontendOrigin ilman lopun kauttaviivaa (/).

Suorita deployment:

```bash
az deployment group create \
  --resource-group rg-hotel-lakeview \
  --template-file bicep/main.bicep \
  --parameters @bicep/main.parameters.json
```

### 3. Deployaa API App Serviceen

```bash
dotnet publish -c Release -o src/HotelLakeview.Api/publish src/HotelLakeview.Api/HotelLakeview.Api.csproj
```

Pakkaa publish-kansio zipiksi ja deployaa:

```bash
powershell -Command "Compress-Archive -Path src/HotelLakeview.Api/publish/* -DestinationPath src/HotelLakeview.Api/publish.zip -Force"

az webapp deploy \
  --resource-group rg-hotel-lakeview \
  --name hotel-lakeview-api \
  --src-path src/HotelLakeview.Api/publish.zip \
  --type zip
```

### 4. Buildaa ja julkaise frontend static websiteen

Anna buildille API-osoite:

```bash
VITE_API_BASE_URL=https://hotel-lakeview-api.azurewebsites.net npm run build --prefix src/HotelLakeview.Frontend
```

Upload dist -> $web:

```bash
az storage blob upload-batch \
  --account-name stlakeview \
  --destination '$web' \
  --source src/HotelLakeview.Frontend/dist \
  --auth-mode login \
  --overwrite
```

Jos RBAC estää uploadin, käytä auth-mode key.

### 5. Varmista CORS

- Aseta frontendOrigin arvoksi frontendin origin ilman trailing slashia.
- Esimerkki oikein: https://stlakeview.z43.web.core.windows.net
- Esimerkki väärin: https://stlakeview.z43.web.core.windows.net/

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

### Health

- GET /health

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

- 34 testiä yhteensä
- 33 yksikkötestiä
- 1 integraatio smoke -testi
- Testatut osa-alueet: hinnoittelu, DateRange-säännöt, käyttöastelaskenta, varauksen tilakäyttäytyminen, ReportService (mockit), CustomerService (mockit), RoomService (mockit)

## Huomio jatkokehitykseen

- Lisää integraatiotestit API-endpointeille ja tietokantakonflikteille
- Lisää autentikointi ja roolipohjainen authorisointi (Receptionist, Manager)
- Siirrä kuvatallennus objektitallennukseen tuotantokäyttöä varten
