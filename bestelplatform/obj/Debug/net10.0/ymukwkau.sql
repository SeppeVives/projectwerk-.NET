CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    PRIMARY KEY (`MigrationId`)
);

START TRANSACTION;
CREATE TABLE `gebruikers` (
    `id` int(11) NOT NULL AUTO_INCREMENT,
    `naam` varchar(255) NULL,
    `wachtwoord_hash` char(255) NULL,
    `unieke_code` varchar(255) NOT NULL,
    `geactiveerd` tinyint(1) NULL DEFAULT '0',
    PRIMARY KEY (`id`)
);

CREATE TABLE `producten` (
    `id` int(11) NOT NULL AUTO_INCREMENT,
    PRIMARY KEY (`id`)
);

CREATE TABLE `rollen` (
    `id` int(11) NOT NULL AUTO_INCREMENT,
    `naam` varchar(255) NOT NULL,
    PRIMARY KEY (`id`)
);

CREATE TABLE `tafels` (
    `id` int(11) NOT NULL AUTO_INCREMENT,
    `nummer` int(11) NOT NULL,
    PRIMARY KEY (`id`)
);

CREATE TABLE `bezoekers` (
    `gebruiker_id` int(11) NOT NULL,
    PRIMARY KEY (`gebruiker_id`),
    CONSTRAINT `bezoekers_ibfk_1` FOREIGN KEY (`gebruiker_id`) REFERENCES `gebruikers` (`id`) ON DELETE RESTRICT
);

CREATE TABLE `medewerkers` (
    `gebruiker_id` int(11) NOT NULL,
    PRIMARY KEY (`gebruiker_id`),
    CONSTRAINT `medewerkers_ibfk_1` FOREIGN KEY (`gebruiker_id`) REFERENCES `gebruikers` (`id`) ON DELETE RESTRICT
);

CREATE TABLE `productdetails` (
    `product_id` int(11) NOT NULL,
    `tijdstip` datetime NOT NULL,
    `naam` varchar(255) NOT NULL,
    `prijs` float NOT NULL,
    `producttype` enum('frisdrank','alcoholische drank','warme drank','dessert','voorgerecht','hoofdgerecht','versnapering') NOT NULL,
    PRIMARY KEY (`tijdstip`, `product_id`),
    CONSTRAINT `productdetails_ibfk_1` FOREIGN KEY (`product_id`) REFERENCES `producten` (`id`) ON DELETE RESTRICT
);

CREATE TABLE `bestellingen` (
    `id` int(11) NOT NULL AUTO_INCREMENT,
    `gebruiker_id` int(11) NULL DEFAULT 'NULL',
    `tijdstip_besteld` datetime NOT NULL,
    `status` enum('geplaatst','geserveerd','klaar','geannuleerd') NOT NULL,
    PRIMARY KEY (`id`),
    CONSTRAINT `bestellingen_ibfk_1` FOREIGN KEY (`gebruiker_id`) REFERENCES `bezoekers` (`gebruiker_id`) ON DELETE RESTRICT
);

CREATE TABLE `tafeltoewijzingen` (
    `gebruiker_id` int(11) NOT NULL,
    `tafel_id` int(11) NOT NULL,
    `tijdstip_toegewezen` datetime NOT NULL,
    PRIMARY KEY (`gebruiker_id`, `tafel_id`, `tijdstip_toegewezen`),
    CONSTRAINT `tafeltoewijzingen_ibfk_1` FOREIGN KEY (`gebruiker_id`) REFERENCES `bezoekers` (`gebruiker_id`) ON DELETE RESTRICT,
    CONSTRAINT `tafeltoewijzingen_ibfk_2` FOREIGN KEY (`tafel_id`) REFERENCES `tafels` (`id`) ON DELETE RESTRICT
);

CREATE TABLE `roltoewijzing` (
    `gebruiker_id` int(11) NOT NULL,
    `rol_id` int(11) NOT NULL,
    PRIMARY KEY (`gebruiker_id`, `rol_id`),
    CONSTRAINT `roltoewijzing_ibfk_1` FOREIGN KEY (`gebruiker_id`) REFERENCES `medewerkers` (`gebruiker_id`) ON DELETE RESTRICT,
    CONSTRAINT `roltoewijzing_ibfk_2` FOREIGN KEY (`rol_id`) REFERENCES `rollen` (`id`) ON DELETE RESTRICT
);

CREATE TABLE `bestellijnen` (
    `bestelling_id` int(11) NOT NULL,
    `product_id` int(11) NOT NULL,
    `hoeveelheid` int(11) NOT NULL,
    PRIMARY KEY (`bestelling_id`, `product_id`),
    CONSTRAINT `bestellijnen_ibfk_1` FOREIGN KEY (`bestelling_id`) REFERENCES `bestellingen` (`id`) ON DELETE RESTRICT,
    CONSTRAINT `bestellijnen_ibfk_2` FOREIGN KEY (`product_id`) REFERENCES `producten` (`id`) ON DELETE RESTRICT
);

CREATE INDEX `product_id` ON `bestellijnen` (`product_id`);

CREATE INDEX `gebruiker_id` ON `bestellingen` (`gebruiker_id`);

CREATE INDEX `product_id1` ON `productdetails` (`product_id`);

CREATE INDEX `rol_id` ON `roltoewijzing` (`rol_id`);

CREATE INDEX `tafel_id` ON `tafeltoewijzingen` (`tafel_id`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260813233909_MaakVeldenNullable', '10.0.2');

COMMIT;

