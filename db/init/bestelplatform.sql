-- phpMyAdmin SQL Dump
-- version 5.2.2
-- https://www.phpmyadmin.net/
--
-- Host: dotnet-bestelplatform-db
-- Gegenereerd op: 28 feb 2026 om 19:16
-- Serverversie: 12.0.2-MariaDB-ubu2404
-- PHP-versie: 8.2.29

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `bestelplatform`
--

-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel `bestellingen`
--

CREATE TABLE `bestellingen` (
  `id` int(11) NOT NULL,
  `gebruiker_id` int(11) DEFAULT NULL,
  `tijdstip_besteld` datetime NOT NULL,
  `status` enum('besteld','geleverd','klaar','geannuleerd') NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel `bezoekers`
--

CREATE TABLE `bezoekers` (
  `gebruiker_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel `gebruikers`
--

CREATE TABLE `gebruikers` (
  `id` int(11) NOT NULL,
  `naam` varchar(255) NOT NULL,
  `wachtwoord_hash` char(255) NOT NULL,
  `unieke_code` varchar(255) NOT NULL,
  `geactiveerd` tinyint(1) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel `medewerkers`
--

CREATE TABLE `medewerkers` (
  `gebruiker_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel `productdetails`
--

CREATE TABLE `productdetails` (
  `product_id` int(11) NOT NULL,
  `tijdstip` datetime NOT NULL,
  `naam` varchar(255) NOT NULL,
  `prijs` float NOT NULL,
  `producttype` enum('frisdrank','alcoholische drank','warme drank','dessert','voorgerecht','hoofdgerecht','versnapering') NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel `producten`
--

CREATE TABLE `producten` (
  `id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel `rollen`
--

CREATE TABLE `rollen` (
  `id` int(11) NOT NULL,
  `naam` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel `roltoewijzing`
--

CREATE TABLE `roltoewijzing` (
  `gebruiker_id` int(11) NOT NULL,
  `rol_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel `tafels`
--

CREATE TABLE `tafels` (
  `id` int(11) NOT NULL,
  `nummer` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel `tafeltoewijzingen`
--

CREATE TABLE `tafeltoewijzingen` (
  `gebruiker_id` int(11) NOT NULL,
  `tafel_id` int(11) NOT NULL,
  `tijdstip_toegewezen` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

--
-- Indexen voor geëxporteerde tabellen
--

--
-- Indexen voor tabel `bestellingen`
--
ALTER TABLE `bestellingen`
  ADD PRIMARY KEY (`id`),
  ADD KEY `gebruiker_id` (`gebruiker_id`);

--
-- Indexen voor tabel `bezoekers`
--
ALTER TABLE `bezoekers`
  ADD PRIMARY KEY (`gebruiker_id`);

--
-- Indexen voor tabel `gebruikers`
--
ALTER TABLE `gebruikers`
  ADD PRIMARY KEY (`id`);

--
-- Indexen voor tabel `medewerkers`
--
ALTER TABLE `medewerkers`
  ADD PRIMARY KEY (`gebruiker_id`);

--
-- Indexen voor tabel `productdetails`
--
ALTER TABLE `productdetails`
  ADD PRIMARY KEY (`tijdstip`,`product_id`),
  ADD KEY `product_id` (`product_id`);

--
-- Indexen voor tabel `producten`
--
ALTER TABLE `producten`
  ADD PRIMARY KEY (`id`);

--
-- Indexen voor tabel `rollen`
--
ALTER TABLE `rollen`
  ADD PRIMARY KEY (`id`);

--
-- Indexen voor tabel `roltoewijzing`
--
ALTER TABLE `roltoewijzing`
  ADD PRIMARY KEY (`gebruiker_id`,`rol_id`),
  ADD KEY `rol_id` (`rol_id`);

--
-- Indexen voor tabel `tafels`
--
ALTER TABLE `tafels`
  ADD PRIMARY KEY (`id`);

--
-- Indexen voor tabel `tafeltoewijzingen`
--
ALTER TABLE `tafeltoewijzingen`
  ADD PRIMARY KEY (`gebruiker_id`,`tafel_id`,`tijdstip_toegewezen`),
  ADD KEY `tafel_id` (`tafel_id`);

--
-- AUTO_INCREMENT voor geëxporteerde tabellen
--

--
-- AUTO_INCREMENT voor een tabel `bestellingen`
--
ALTER TABLE `bestellingen`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT voor een tabel `gebruikers`
--
ALTER TABLE `gebruikers`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT voor een tabel `producten`
--
ALTER TABLE `producten`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT voor een tabel `rollen`
--
ALTER TABLE `rollen`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT voor een tabel `tafels`
--
ALTER TABLE `tafels`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- Beperkingen voor geëxporteerde tabellen
--

--
-- Beperkingen voor tabel `bestellingen`
--
ALTER TABLE `bestellingen`
  ADD CONSTRAINT `bestellingen_ibfk_1` FOREIGN KEY (`gebruiker_id`) REFERENCES `bezoekers` (`gebruiker_id`);

--
-- Beperkingen voor tabel `bezoekers`
--
ALTER TABLE `bezoekers`
  ADD CONSTRAINT `bezoekers_ibfk_1` FOREIGN KEY (`gebruiker_id`) REFERENCES `gebruikers` (`id`);

--
-- Beperkingen voor tabel `medewerkers`
--
ALTER TABLE `medewerkers`
  ADD CONSTRAINT `medewerkers_ibfk_1` FOREIGN KEY (`gebruiker_id`) REFERENCES `gebruikers` (`id`);

--
-- Beperkingen voor tabel `productdetails`
--
ALTER TABLE `productdetails`
  ADD CONSTRAINT `productdetails_ibfk_1` FOREIGN KEY (`product_id`) REFERENCES `producten` (`id`);

--
-- Beperkingen voor tabel `roltoewijzing`
--
ALTER TABLE `roltoewijzing`
  ADD CONSTRAINT `roltoewijzing_ibfk_1` FOREIGN KEY (`gebruiker_id`) REFERENCES `medewerkers` (`gebruiker_id`),
  ADD CONSTRAINT `roltoewijzing_ibfk_2` FOREIGN KEY (`rol_id`) REFERENCES `rollen` (`id`);

--
-- Beperkingen voor tabel `tafeltoewijzingen`
--
ALTER TABLE `tafeltoewijzingen`
  ADD CONSTRAINT `tafeltoewijzingen_ibfk_1` FOREIGN KEY (`gebruiker_id`) REFERENCES `bezoekers` (`gebruiker_id`),
  ADD CONSTRAINT `tafeltoewijzingen_ibfk_2` FOREIGN KEY (`tafel_id`) REFERENCES `tafels` (`id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
