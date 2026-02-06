# 🍻 ORW Bestelplatform – Projectwerk .NET

Dit project werd ontwikkeld in het kader van het **.NET projectwerk** en heeft als doel het digitaal verwerken van bestellingen tijdens activiteiten van de **ORW (Ouderraad Wielewaal)**.

Bezoekers kunnen via een **unieke QR-code per tafel** eenvoudig drank en versnaperingen bestellen, onmiddellijk betalen en de status van hun bestelling opvolgen. Medewerkers krijgen via verschillende dashboards een duidelijk overzicht van alle bestellingen, afgestemd op hun rol.

---

## 📌 Functionaliteiten

### 👤 Bezoekers (Mobiele Webpagina)
Bezoekers scannen een QR-code op hun tafel en worden doorgestuurd naar een mobiele webpagina waar ze:

- Het **tafelnummer** duidelijk kunnen zien
- Een **assortiment van drank en versnaperingen** kunnen bekijken
- Producten kunnen toevoegen aan hun bestelling
- Een **overzicht krijgen van hun bestelling**, inclusief:
  - Eenheidsprijs
  - Aantal
  - Subtotaal per product
  - Totaalprijs
- Hun bestelling **onmiddellijk betalen via Mollie**
- De **status van hun bestelling** opvolgen (in wachtrij, in bereiding, klaar, …)

➡️ Een bestelling is pas definitief **na succesvolle betaling**.

---

### 🧑‍🍳 Medewerkersdashboard
Het dashboard biedt verschillende overzichten, afhankelijk van de rol van de medewerker:

#### 🍔 Keukenmedewerkers
- Overzicht van alle bestelde **versnaperingen**
- Mogelijkheid om producten als **klaar voor ophaling** te markeren

#### 🍺 Barmedewerkers
- Overzicht van alle bestelde **dranken**
- Mogelijkheid om dranken als **klaar voor ophaling** te markeren

#### 🧍 Zaalmedewerkers
- Overzicht van **klaargezette bestellingen per tafel**
- Duidelijk zichtbaar welke tafel bediend moet worden

#### 🗂️ Administratieve medewerkers
- Toegang tot **alle bovenstaande overzichten**
- Uitgebreide **bestelhistoriek**, inclusief:
  - Tijdstip
  - Tafel
  - Totaalprijs
  - Gebruikersnaam
  - Bestelstatus
  - Betaalstatus
  - Link naar een **detailpagina per bestelling**
- Historiek is **sorteerbaar en filterbaar** op alle relevante velden

---

### 📊 JSON Web API
De applicatie voorziet een **publiek toegankelijke JSON API** voor statistieken:

- Meest en minst bestelde drank/versnapering
- Tafel met de hoogste uitgaven (drank en/of versnaperingen)

---

### 🛠️ Admin Dashboard
Administrators kunnen via een apart dashboard:

- Gebruikers **aanmaken en verwijderen**
- Rollen toewijzen:
  - bezoeker
  - barmedewerker
  - keukenmedewerker
  - zaalmedewerker
  - administratief medewerker
  - administrator  
  *(één gebruiker kan meerdere rollen hebben)*
- Een **unieke registratie-URL + QR-code** genereren voor nieuwe gebruikers
- Het **assortiment beheren**:
  - Producten toevoegen/verwijderen
  - Prijzen aanpassen

---

## 🔐 Toegangscontrole

| Onderdeel | Toegang |
|------------|---------|
| Mobiele webpagina | Bezoekers |
| Dashboard | Medewerkers |
| Admin dashboard | Administrators |
| JSON API | Publiek |

---

## 💾 Technologieën & Architectuur

- **C# .NET**
- **ASP.NET (back-end)**
- **MVC Framework (front-end)**
- **Bootstrap** voor layout & responsive design
- **Mollie API** voor betalingen
- **Docker**:
  - lokale ontwikkelomgeving
  - cloud hosting
- Persistente opslag om **gegevensverlies bij stroomuitval** te vermijden

---

## 🐳 Docker Setup

De volledige applicatie draait:
- lokaal via **Docker**
- én in de **cloud**

Alle services (webapplicatie, database, admin tools) zijn gecontaineriseerd zodat de applicatie eenvoudig opstartbaar en reproduceerbaar is.

---

## 🎯 Doel van het project

Dit project toont aan hoe een **realistisch bestelsysteem** kan worden opgebouwd met:

- Role-based toegang
- Betalingen
- Real-time opvolging
- Schaalbare architectuur

Het systeem is ontworpen met gebruiksgemak voor bezoekers én efficiëntie voor medewerkers als belangrijkste focus.

---

## 👨‍💻 Auteur

Project ontwikkeld in het kader van het **.NET projectwerk**  
Opleiding: Bachelor ICT 
School: VIVES-Kortrijk

---
