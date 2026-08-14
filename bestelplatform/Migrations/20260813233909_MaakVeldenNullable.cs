using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace bestelplatform.Migrations
{
    /// <inheritdoc />
    public partial class MaakVeldenNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "gebruikers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    naam = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    wachtwoord_hash = table.Column<string>(type: "char(255)", fixedLength: true, maxLength: 255, nullable: true),
                    unieke_code = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    geactiveerd = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "producten",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "rollen",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    naam = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tafels",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    nummer = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "bezoekers",
                columns: table => new
                {
                    gebruiker_id = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.gebruiker_id);
                    table.ForeignKey(
                        name: "bezoekers_ibfk_1",
                        column: x => x.gebruiker_id,
                        principalTable: "gebruikers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "medewerkers",
                columns: table => new
                {
                    gebruiker_id = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.gebruiker_id);
                    table.ForeignKey(
                        name: "medewerkers_ibfk_1",
                        column: x => x.gebruiker_id,
                        principalTable: "gebruikers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "productdetails",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "int(11)", nullable: false),
                    tijdstip = table.Column<DateTime>(type: "datetime", nullable: false),
                    naam = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    prijs = table.Column<float>(type: "float", nullable: false),
                    producttype = table.Column<string>(type: "enum('frisdrank','alcoholische drank','warme drank','dessert','voorgerecht','hoofdgerecht','versnapering')", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.tijdstip, x.product_id });
                    table.ForeignKey(
                        name: "productdetails_ibfk_1",
                        column: x => x.product_id,
                        principalTable: "producten",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "bestellingen",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    gebruiker_id = table.Column<int>(type: "int(11)", nullable: true, defaultValueSql: "'NULL'"),
                    tijdstip_besteld = table.Column<DateTime>(type: "datetime", nullable: false),
                    status = table.Column<string>(type: "enum('geplaatst','geserveerd','klaar','geannuleerd')", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "bestellingen_ibfk_1",
                        column: x => x.gebruiker_id,
                        principalTable: "bezoekers",
                        principalColumn: "gebruiker_id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tafeltoewijzingen",
                columns: table => new
                {
                    gebruiker_id = table.Column<int>(type: "int(11)", nullable: false),
                    tafel_id = table.Column<int>(type: "int(11)", nullable: false),
                    tijdstip_toegewezen = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.gebruiker_id, x.tafel_id, x.tijdstip_toegewezen });
                    table.ForeignKey(
                        name: "tafeltoewijzingen_ibfk_1",
                        column: x => x.gebruiker_id,
                        principalTable: "bezoekers",
                        principalColumn: "gebruiker_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "tafeltoewijzingen_ibfk_2",
                        column: x => x.tafel_id,
                        principalTable: "tafels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "roltoewijzing",
                columns: table => new
                {
                    gebruiker_id = table.Column<int>(type: "int(11)", nullable: false),
                    rol_id = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.gebruiker_id, x.rol_id });
                    table.ForeignKey(
                        name: "roltoewijzing_ibfk_1",
                        column: x => x.gebruiker_id,
                        principalTable: "medewerkers",
                        principalColumn: "gebruiker_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "roltoewijzing_ibfk_2",
                        column: x => x.rol_id,
                        principalTable: "rollen",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "bestellijnen",
                columns: table => new
                {
                    bestelling_id = table.Column<int>(type: "int(11)", nullable: false),
                    product_id = table.Column<int>(type: "int(11)", nullable: false),
                    hoeveelheid = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.bestelling_id, x.product_id });
                    table.ForeignKey(
                        name: "bestellijnen_ibfk_1",
                        column: x => x.bestelling_id,
                        principalTable: "bestellingen",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "bestellijnen_ibfk_2",
                        column: x => x.product_id,
                        principalTable: "producten",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "product_id",
                table: "bestellijnen",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "gebruiker_id",
                table: "bestellingen",
                column: "gebruiker_id");

            migrationBuilder.CreateIndex(
                name: "product_id1",
                table: "productdetails",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "rol_id",
                table: "roltoewijzing",
                column: "rol_id");

            migrationBuilder.CreateIndex(
                name: "tafel_id",
                table: "tafeltoewijzingen",
                column: "tafel_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bestellijnen");

            migrationBuilder.DropTable(
                name: "productdetails");

            migrationBuilder.DropTable(
                name: "roltoewijzing");

            migrationBuilder.DropTable(
                name: "tafeltoewijzingen");

            migrationBuilder.DropTable(
                name: "bestellingen");

            migrationBuilder.DropTable(
                name: "producten");

            migrationBuilder.DropTable(
                name: "medewerkers");

            migrationBuilder.DropTable(
                name: "rollen");

            migrationBuilder.DropTable(
                name: "tafels");

            migrationBuilder.DropTable(
                name: "bezoekers");

            migrationBuilder.DropTable(
                name: "gebruikers");
        }
    }
}
