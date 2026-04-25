# Hotel Lakeview Frontend

Henkilökunnan sisäinen käyttöliittymä Hotel Lakeview -varausjärjestelmälle.

## Moduulit

- Koontinäkymä
- Varaukset
- Asiakkaat
- Huoneet
- Raportit (Manager)

## Kielet

- Suomi
- Englanti

Staff consolen kielen voi vaihtaa käyttöliittymästä reaaliaikaisesti.

## Teknologiat

- React
- TypeScript
- Vite
- TanStack Query

## Käynnistys

1. Asenna paketit:

```bash
npm install
```

2. Käynnistä dev-palvelin:

```bash
npm run dev
```

3. Rakenna tuotantoversio:

```bash
npm run build
```

## API-yhteys

Frontend kutsuu backendia /api-polun kautta.

Dev-ympäristössä Vite proxyaa pyynnöt oletuksena osoitteeseen:

- http://localhost:5178

Muuta tarvittaessa tiedostossa .env (katso .env.example):

- VITE_PROXY_TARGET
- VITE_API_BASE_URL

## Turvallisuusrajaus

Tämä frontend on tarkoitettu vain hotellin henkilökunnan sisäkäyttöön. Roolinäkymä (Receptionist/Manager) on toteutettu UI-tasolla. Täyteen tuotantoturvallisuuteen tarvitaan backend-autentikointi ja API-authorisointi.
