using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace bestelplatform.bestelplatform;

public partial class BestelplatformContext : DbContext
{
    public BestelplatformContext()
    {
    }

    public BestelplatformContext(DbContextOptions<BestelplatformContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bestellijnen> Bestellijnens { get; set; }

    public virtual DbSet<Bestellingen> Bestellingens { get; set; }

    public virtual DbSet<Bezoeker> Bezoekers { get; set; }

    public virtual DbSet<Gebruiker> Gebruikers { get; set; }

    public virtual DbSet<Medewerker> Medewerkers { get; set; }

    public virtual DbSet<Productdetail> Productdetails { get; set; }

    public virtual DbSet<Producten> Productens { get; set; }

    public virtual DbSet<Rollen> Rollens { get; set; }

    public virtual DbSet<Tafel> Tafels { get; set; }

    public virtual DbSet<Tafeltoewijzingen> Tafeltoewijzingens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("server=127.0.0.1;port=3307;uid=root;pwd=root;database=bestelplatform");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bestellijnen>(entity =>
        {
            entity.HasKey(e => new { e.BestellingId, e.ProductId }).HasName("PRIMARY");

            entity.ToTable("bestellijnen");

            entity.HasIndex(e => e.ProductId, "product_id");

            entity.Property(e => e.BestellingId)
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
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("bestellijnen_ibfk_1");

            entity.HasOne(d => d.Product).WithMany(p => p.Bestellijnens)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("bestellijnen_ibfk_2");
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
            entity.Property(e => e.Status)
                .HasColumnType("enum('geplaatst','geserveerd','klaar','geannuleerd')")
                .HasColumnName("status");
            entity.Property(e => e.TijdstipBesteld)
                .HasColumnType("datetime")
                .HasColumnName("tijdstip_besteld");

            entity.HasOne(d => d.Gebruiker).WithMany(p => p.Bestellingens)
                .HasForeignKey(d => d.GebruikerId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("bestellingen_ibfk_1");
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
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("bezoekers_ibfk_1");
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
                .IsFixedLength()
                .HasColumnName("wachtwoord_hash");
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
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("medewerkers_ibfk_1");

            entity.HasMany(d => d.Rols).WithMany(p => p.Gebruikers)
                .UsingEntity<Dictionary<string, object>>(
                    "Roltoewijzing",
                    r => r.HasOne<Rollen>().WithMany()
                        .HasForeignKey("RolId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("roltoewijzing_ibfk_2"),
                    l => l.HasOne<Medewerker>().WithMany()
                        .HasForeignKey("GebruikerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("roltoewijzing_ibfk_1"),
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

        modelBuilder.Entity<Productdetail>(entity =>
        {
            entity.HasKey(e => new { e.Tijdstip, e.ProductId }).HasName("PRIMARY");

            entity.ToTable("productdetails");

            entity.HasIndex(e => e.ProductId, "product_id");

            entity.Property(e => e.Tijdstip)
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
                .HasColumnType("enum('frisdrank','alcoholische drank','warme drank','dessert','voorgerecht','hoofdgerecht','versnapering')")
                .HasColumnName("producttype");

            entity.HasOne(d => d.Product).WithMany(p => p.Productdetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("productdetails_ibfk_1");
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
                .HasColumnType("datetime")
                .HasColumnName("tijdstip_toegewezen");

            entity.HasOne(d => d.Gebruiker).WithMany(p => p.Tafeltoewijzingens)
                .HasForeignKey(d => d.GebruikerId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("tafeltoewijzingen_ibfk_1");

            entity.HasOne(d => d.Tafel).WithMany(p => p.Tafeltoewijzingens)
                .HasForeignKey(d => d.TafelId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("tafeltoewijzingen_ibfk_2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
