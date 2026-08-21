using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace bestelplatform.Data.bestelplatform;

public partial class BestelplatformDbContext : DbContext
{
    public BestelplatformDbContext()
    {
    }

    public BestelplatformDbContext(DbContextOptions<BestelplatformDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bestellijnen> Bestellijnens { get; set; }

    public virtual DbSet<Bestellingen> Bestellingens { get; set; }

    public virtual DbSet<Bezoeker> Bezoekers { get; set; }

    public virtual DbSet<EfmigrationsHistory> EfmigrationsHistories { get; set; }

    public virtual DbSet<Gebruiker> Gebruikers { get; set; }

    public virtual DbSet<Medewerker> Medewerkers { get; set; }

    public virtual DbSet<Productdetail> Productdetails { get; set; }

    public virtual DbSet<Producten> Productens { get; set; }

    public virtual DbSet<Rollen> Rollens { get; set; }

    public virtual DbSet<Tafel> Tafels { get; set; }

    public virtual DbSet<Tafeltoewijzingen> Tafeltoewijzingens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySQL("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bestellijnen>(entity =>
        {
            entity.HasKey(e => new { e.BestellingId, e.ProductId }).HasName("PRIMARY");

            entity.ToTable("bestellijnen");

            entity.HasIndex(e => e.ProductId, "product_id");

            entity.Property(e => e.BestellingId)
                .ValueGeneratedOnAdd()
                .HasColumnType("int(11)")
                .HasColumnName("bestelling_id");
            entity.Property(e => e.ProductId)
                .HasColumnType("int(11)")
                .HasColumnName("product_id");
            entity.Property(e => e.Hoeveelheid)
                .HasColumnType("int(11)")
                .HasColumnName("hoeveelheid");

            entity.HasOne(d => d.Bestelling).WithMany(p => p.Bestellijnens)
                .HasForeignKey(d => d.BestellingId)
                .HasConstraintName("FK_bestellijnen_bestellingen");

            entity.HasOne(d => d.Product).WithMany(p => p.Bestellijnens)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_bestellijnen_producten");
        });

        modelBuilder.Entity<Bestellingen>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("bestellingen");

            entity.HasIndex(e => e.GebruikerId, "gebruiker_id");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.GebruikerId)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)")
                .HasColumnName("gebruiker_id");
            entity.Property(e => e.MolliePaymentid)
                .HasMaxLength(255)
                .HasColumnName("mollie_paymentid");
            entity.Property(e => e.Status)
                .HasColumnType("enum('besteld','klaar','afgeleverd')")
                .HasColumnName("status");
            entity.Property(e => e.TijdstipBesteld)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("datetime")
                .HasColumnName("tijdstip_besteld");

            entity.HasOne(d => d.Gebruiker).WithMany(p => p.Bestellingens)
                .HasForeignKey(d => d.GebruikerId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_bestellingen_bezoekers");
        });

        modelBuilder.Entity<Bezoeker>(entity =>
        {
            entity.HasKey(e => e.GebruikerId).HasName("PRIMARY");

            entity.ToTable("bezoekers");

            entity.Property(e => e.GebruikerId)
                .HasColumnType("int(11)")
                .HasColumnName("gebruiker_id");

            entity.HasOne(d => d.Gebruiker).WithOne(p => p.Bezoeker)
                .HasForeignKey<Bezoeker>(d => d.GebruikerId)
                .HasConstraintName("FK_bezoekers_gebruikers");
        });

        modelBuilder.Entity<EfmigrationsHistory>(entity =>
        {
            entity.HasKey(e => e.MigrationId).HasName("PRIMARY");

            entity.ToTable("__EFMigrationsHistory");

            entity.Property(e => e.MigrationId).HasMaxLength(150);
            entity.Property(e => e.ProductVersion).HasMaxLength(32);
        });

        modelBuilder.Entity<Gebruiker>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("gebruikers");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Geactiveerd)
                .HasDefaultValueSql("'0'")
                .HasColumnName("geactiveerd");
            entity.Property(e => e.Naam)
                .HasMaxLength(255)
                .HasColumnName("naam");
            entity.Property(e => e.UniekeCode)
                .HasMaxLength(255)
                .HasColumnName("unieke_code");
            entity.Property(e => e.WachtwoordHash)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .IsFixedLength()
                .HasColumnName("wachtwoord_hash");

            entity.HasMany(d => d.Rols).WithMany(p => p.Gebruikers)
                .UsingEntity<Dictionary<string, object>>(
                    "Roltoewijzing",
                    r => r.HasOne<Rollen>().WithMany()
                        .HasForeignKey("RolId")
                        .HasConstraintName("FK_roltoewijzing_rollen"),
                    l => l.HasOne<Gebruiker>().WithMany()
                        .HasForeignKey("GebruikerId")
                        .HasConstraintName("FK_roltoewijzing_gebruikers"),
                    j =>
                    {
                        j.HasKey("GebruikerId", "RolId").HasName("PRIMARY");
                        j.ToTable("roltoewijzing");
                        j.HasIndex(new[] { "RolId" }, "rol_id");
                        j.IndexerProperty<int>("GebruikerId")
                            .HasColumnType("int(11)")
                            .HasColumnName("gebruiker_id");
                        j.IndexerProperty<int>("RolId")
                            .HasColumnType("int(11)")
                            .HasColumnName("rol_id");
                    });
        });

        modelBuilder.Entity<Medewerker>(entity =>
        {
            entity.HasKey(e => e.GebruikerId).HasName("PRIMARY");

            entity.ToTable("medewerkers");

            entity.Property(e => e.GebruikerId)
                .HasColumnType("int(11)")
                .HasColumnName("gebruiker_id");

            entity.HasOne(d => d.Gebruiker).WithOne(p => p.Medewerker)
                .HasForeignKey<Medewerker>(d => d.GebruikerId)
                .HasConstraintName("FK_medewerkers_gebruikers");
        });

        modelBuilder.Entity<Productdetail>(entity =>
        {
            entity.HasKey(e => new { e.Tijdstip, e.ProductId }).HasName("PRIMARY");

            entity.ToTable("productdetails");

            entity.HasIndex(e => e.ProductId, "product_id");

            entity.Property(e => e.Tijdstip)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("datetime")
                .HasColumnName("tijdstip");
            entity.Property(e => e.ProductId)
                .HasColumnType("int(11)")
                .HasColumnName("product_id");
            entity.Property(e => e.Naam)
                .HasMaxLength(255)
                .HasColumnName("naam");
            entity.Property(e => e.Prijs).HasColumnName("prijs");
            entity.Property(e => e.Producttype)
                .HasColumnType("enum('frisdrank','alcoholische_drank','warme_drank','dessert','voorgerecht','hoofdgerecht','versnapering')")
                .HasColumnName("producttype");
            entity.HasOne<Producten>()
            .WithMany(p => p.Productdetails)
            .HasForeignKey(e => e.ProductId)
            .HasConstraintName("FK_productdetails_producten");
        });

        modelBuilder.Entity<Producten>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("producten");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
        });

        modelBuilder.Entity<Rollen>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("rollen");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Naam)
                .HasMaxLength(255)
                .HasColumnName("naam");
        });

        modelBuilder.Entity<Tafel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tafels");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Nummer)
                .HasColumnType("int(11)")
                .HasColumnName("nummer");
        });

        modelBuilder.Entity<Tafeltoewijzingen>(entity =>
        {
            entity.HasKey(e => new { e.GebruikerId, e.TafelId, e.TijdstipToegewezen }).HasName("PRIMARY");

            entity.ToTable("tafeltoewijzingen");

            entity.HasIndex(e => e.TafelId, "tafel_id");

            entity.Property(e => e.GebruikerId)
                .HasColumnType("int(11)")
                .HasColumnName("gebruiker_id");
            entity.Property(e => e.TafelId)
                .HasColumnType("int(11)")
                .HasColumnName("tafel_id");
            entity.Property(e => e.TijdstipToegewezen)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("datetime")
                .HasColumnName("tijdstip_toegewezen");

            entity.HasOne(d => d.Gebruiker).WithMany(p => p.Tafeltoewijzingens)
                .HasForeignKey(d => d.GebruikerId)
                .HasConstraintName("FK_tafeltoewijzingen_bezoekers");

            entity.HasOne(d => d.Tafel).WithMany(p => p.Tafeltoewijzingens)
                .HasForeignKey(d => d.TafelId)
                .HasConstraintName("FK_tafeltoewijzingen_tafels");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
