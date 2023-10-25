using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DAL.Models
{
    public partial class IndustrialKıtchenEquipmentsDbContext : DbContext
    {
        public IndustrialKıtchenEquipmentsDbContext()
        {
        }

        public IndustrialKıtchenEquipmentsDbContext(DbContextOptions<IndustrialKıtchenEquipmentsDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AltSirket> AltSirkets { get; set; } = null!;
        public virtual DbSet<Kullanıcılar> Kullanıcılars { get; set; } = null!;
        public virtual DbSet<Musteriler> Musterilers { get; set; } = null!;
        public virtual DbSet<ParaBirimleri> ParaBirimleris { get; set; } = null!;
        public virtual DbSet<Sepet> Sepets { get; set; } = null!;
        public virtual DbSet<StokKategori> StokKategoris { get; set; } = null!;
        public virtual DbSet<StokResim> StokResims { get; set; } = null!;
        public virtual DbSet<Stoklar> Stoklars { get; set; } = null!;
        public virtual DbSet<TeklifDetay> TeklifDetays { get; set; } = null!;
        public virtual DbSet<Teklifler> Tekliflers { get; set; } = null!;
        public virtual DbSet<Vergi> Vergis { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=IndustrialKıtchenEquipmentsDb;Integrated Security=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AltSirket>(entity =>
            {
                entity.ToTable("AltSirket");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Mail).HasMaxLength(50);

                entity.Property(e => e.Resim).HasMaxLength(50);

                entity.Property(e => e.SirketIsmi).HasMaxLength(50);
            });

            modelBuilder.Entity<Kullanıcılar>(entity =>
            {
                entity.ToTable("Kullanıcılar");

                entity.Property(e => e.AdSoyad).HasMaxLength(100);

                entity.Property(e => e.KullaniciAdi).HasMaxLength(50);

                entity.Property(e => e.Sifre).HasMaxLength(50);
            });

            modelBuilder.Entity<Musteriler>(entity =>
            {
                entity.ToTable("Musteriler");

                entity.Property(e => e.Mail).HasMaxLength(50);

                entity.Property(e => e.Telefon).HasMaxLength(50);

                entity.Property(e => e.Unvan).HasMaxLength(100);
            });

            modelBuilder.Entity<ParaBirimleri>(entity =>
            {
                entity.ToTable("ParaBirimleri");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BirimId).HasMaxLength(50);

                entity.Property(e => e.Kod).HasMaxLength(50);
            });

            modelBuilder.Entity<Sepet>(entity =>
            {
                entity.ToTable("Sepet");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Miktar).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Kullanıcı)
                    .WithMany(p => p.Sepets)
                    .HasForeignKey(d => d.KullanıcıId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Sepet_Kullanıcılar");

                entity.HasOne(d => d.Musteri)
                    .WithMany(p => p.Sepets)
                    .HasForeignKey(d => d.MusteriId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Sepet_Musteriler");
            });

            modelBuilder.Entity<StokKategori>(entity =>
            {
                entity.ToTable("StokKategori");

                entity.Property(e => e.Ad).HasMaxLength(50);
            });

            modelBuilder.Entity<StokResim>(entity =>
            {
                entity.ToTable("StokResim");

                entity.Property(e => e.Resim).HasMaxLength(50);

                entity.HasOne(d => d.Stok)
                    .WithMany(p => p.StokResims)
                    .HasForeignKey(d => d.StokId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StokResim_Stoklar");
            });

            modelBuilder.Entity<Stoklar>(entity =>
            {
                entity.ToTable("Stoklar");

                entity.Property(e => e.BirimFiyat).HasColumnType("money");

                entity.Property(e => e.Dil_id).HasMaxLength(50);

                entity.Property(e => e.Kdv).HasColumnName("KDV");

                entity.Property(e => e.Kdvoranı)
                    .HasColumnType("money")
                    .HasColumnName("KDVOranı");

                entity.Property(e => e.StokAdı)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.StokKodu).HasMaxLength(50);

                entity.HasOne(d => d.Kategori)
                    .WithMany(p => p.Stoklars)
                    .HasForeignKey(d => d.KategoriId)
                    .HasConstraintName("FK_Stoklar_StokKategori");

                entity.HasOne(d => d.ParaBirimiNavigation)
                    .WithMany(p => p.Stoklars)
                    .HasForeignKey(d => d.ParaBirimiId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Stoklar_ParaBirimleri");
            });

            modelBuilder.Entity<TeklifDetay>(entity =>
            {
                entity.ToTable("TeklifDetay");

                entity.Property(e => e.BirimFiyat).HasColumnType("money");

                entity.Property(e => e.Kdv).HasColumnName("KDV");

                entity.Property(e => e.KdvOrani).HasColumnType("money");

                entity.Property(e => e.KdvTutari).HasColumnType("money");

                entity.Property(e => e.Kur).HasColumnType("money");

                entity.Property(e => e.Miktar).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ParaBirimiId).HasMaxLength(50);

                entity.Property(e => e.StockKodu).HasMaxLength(50);

                entity.Property(e => e.ToplamTutar).HasColumnType("money");

                entity.Property(e => e.Tutar).HasColumnType("money");

                entity.HasOne(d => d.Teklif)
                    .WithMany(p => p.TeklifDetays)
                    .HasForeignKey(d => d.TeklifId)
                    .HasConstraintName("FK_TeklifDetay_Teklifler");
            });

            modelBuilder.Entity<Teklifler>(entity =>
            {
                entity.ToTable("Teklifler");

                entity.Property(e => e.Dil_id).HasMaxLength(50);

                entity.Property(e => e.IskontoOrani).HasColumnType("money");

                entity.Property(e => e.IskontoTutari).HasColumnType("money");

                entity.Property(e => e.IslemTarihi).HasColumnType("datetime");

                entity.Property(e => e.Kur).HasColumnType("money");

                entity.Property(e => e.TeklifTarihi).HasColumnType("datetime");

                entity.Property(e => e.TeklifTutarı).HasColumnType("money");

                entity.Property(e => e.ToplamKdv).HasColumnType("money");

                entity.HasOne(d => d.AltSirket)
                    .WithMany(p => p.Tekliflers)
                    .HasForeignKey(d => d.AltSirketId)
                    .HasConstraintName("FK_Teklifler_Sirket");

                entity.HasOne(d => d.KullanıcıNavigation)
                    .WithMany(p => p.Tekliflers)
                    .HasForeignKey(d => d.Kullanıcı)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Teklifler_Kullanıcılar");

                entity.HasOne(d => d.Musteri)
                    .WithMany(p => p.Tekliflers)
                    .HasForeignKey(d => d.MusteriId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Teklifler_Musteriler");

                entity.HasOne(d => d.ParaBirimiNavigation)
                    .WithMany(p => p.Tekliflers)
                    .HasForeignKey(d => d.ParaBirimiId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Teklifler_ParaBirimleri");
            });

            modelBuilder.Entity<Vergi>(entity =>
            {
                entity.ToTable("Vergi");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Deger).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Isim).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
