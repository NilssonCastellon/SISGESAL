using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SISGESAL.Models;

public partial class SisgesalContext : DbContext
{
    public SisgesalContext()
    {
    }

    public SisgesalContext(DbContextOptions<SisgesalContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Almacene> Almacenes { get; set; }

    public virtual DbSet<Articulo> Articulos { get; set; }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<ConfiguraAlmacenArticulo> ConfiguraAlmacenArticulos { get; set; }

    public virtual DbSet<ContadorArticulo> ContadorArticulos { get; set; }

    public virtual DbSet<ControlIncremental> ControlIncrementals { get; set; }

    public virtual DbSet<Transaccion> Transaccions { get; set; }

    public virtual DbSet<TransaccionDetalle> TransaccionDetalles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=0801-INFO-PRAC\\SQLEXPRESS; Database=SISGESAL; Trusted_Connection=True; Encrypt=True; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Almacene>(entity =>
        {
            entity.HasKey(e => e.Identificador).HasName("PK__Almacene__F2374EB11194D985");

            entity.Property(e => e.Descripcion).HasMaxLength(255);
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
            entity.Property(e => e.UsuarioCreacion).HasMaxLength(255);
        });

        modelBuilder.Entity<Articulo>(entity =>
        {
            entity.HasKey(e => e.IdentificadorArticulo).HasName("PK__Articulo__44208B17F112FB42");

            entity.HasIndex(e => e.CodigoArticulo, "UQ__Articulo__5A77498520A09B73").IsUnique();

            entity.Property(e => e.CodigoAlmacen).HasColumnName("codigoAlmacen");
            entity.Property(e => e.DescripcionArticulo).HasMaxLength(255);
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
            entity.Property(e => e.PuntoMaximo).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PuntoMinimo).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PuntoReorden).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TipoArticulo).HasMaxLength(255);
            entity.Property(e => e.UsuarioCreacion).HasMaxLength(255);

            entity.HasOne(d => d.CodigoAlmacenNavigation).WithMany(p => p.Articulos)
                .HasForeignKey(d => d.CodigoAlmacen)
                .HasConstraintName("fk_codigoAlmacen");
        });

        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AspNetRo__3214EC079A9C0A22");

            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AspNetRo__3214EC077881AF64");

            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__AspNetRol__RoleI__286302EC");
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AspNetUs__3214EC07AF1D4EA3");

            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex").IsUnique();

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK__AspNetUse__RoleI__30F848ED"),
                    l => l.HasOne<AspNetUser>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__AspNetUse__UserI__31EC6D26"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("PK__AspNetUs__AF2760AD87B7A183");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AspNetUs__3214EC0707876C32");

            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__AspNetUse__UserI__2B3F6F97");
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey }).HasName("PK__AspNetUs__2B2C5B52743228F7");

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.ProviderKey).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__AspNetUse__UserI__2E1BDC42");
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name }).HasName("PK__AspNetUs__8CC49841D3800FA9");

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__AspNetUse__UserI__34C8D9D1");
        });

        modelBuilder.Entity<ConfiguraAlmacenArticulo>(entity =>
        {
            entity.HasKey(e => e.IdentificadorConfig).HasName("PK__Configur__9566548746C5383F");

            entity.ToTable("ConfiguraAlmacenArticulo");

            entity.HasOne(d => d.IdentificadorAlmacenNavigation).WithMany(p => p.ConfiguraAlmacenArticulos)
                .HasForeignKey(d => d.IdentificadorAlmacen)
                .HasConstraintName("FK__Configura__Ident__571DF1D5");

            entity.HasOne(d => d.IdentificadorArticuloNavigation).WithMany(p => p.ConfiguraAlmacenArticulos)
                .HasForeignKey(d => d.IdentificadorArticulo)
                .HasConstraintName("FK__Configura__Ident__5812160E");
        });

        modelBuilder.Entity<ContadorArticulo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Contador__3214EC07146FE865");

            entity.Property(e => e.Contador).HasDefaultValue(1);

            entity.HasOne(d => d.CodigoAlmacenNavigation).WithMany(p => p.ContadorArticulos)
                .HasForeignKey(d => d.CodigoAlmacen)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ContadorA__Codig__41EDCAC5");
        });

        modelBuilder.Entity<ControlIncremental>(entity =>
        {
            entity.HasKey(e => new { e.Año, e.CodigoAlmacen }).HasName("PK__ControlI__846DB5C50D50014F");

            entity.ToTable("ControlIncremental");

            entity.Property(e => e.CodigoAlmacen)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Transaccion>(entity =>
        {
            entity.HasKey(e => e.Ningresos).HasName("PK__Transacc__F5B448F1C07449A4");

            entity.ToTable("Transaccion");

            entity.Property(e => e.Ningresos).HasMaxLength(20);
            entity.Property(e => e.FechaIngreso).HasColumnType("datetime");
            entity.Property(e => e.FechaRequisicion).HasColumnType("datetime");
            entity.Property(e => e.Operacion).HasMaxLength(255);

            entity.HasOne(d => d.CodigoAlmacenNavigation).WithMany(p => p.Transaccions)
                .HasForeignKey(d => d.CodigoAlmacen)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transacci__Codig__403A8C7D");
        });

        modelBuilder.Entity<TransaccionDetalle>(entity =>
        {
            entity.HasKey(e => e.IdentificadorTrans).HasName("PK__Transacc__A691C8D73759A022");

            entity.ToTable("TransaccionDetalle");

            entity.Property(e => e.FechaIngreso).HasColumnType("datetime");
            entity.Property(e => e.Ningresos).HasMaxLength(20);
            entity.Property(e => e.Operacion).HasMaxLength(255);
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.CodigoAlmacenNavigation).WithMany(p => p.TransaccionDetalles)
                .HasForeignKey(d => d.CodigoAlmacen)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transacci__Codig__5BE2A6F2");

            entity.HasOne(d => d.CodigoArticuloNavigation).WithMany(p => p.TransaccionDetalles)
                .HasForeignKey(d => d.CodigoArticulo)
                .HasConstraintName("FK__Transacci__Codig__5CD6CB2B");

            entity.HasOne(d => d.NingresosNavigation).WithMany(p => p.TransaccionDetalles)
                .HasForeignKey(d => d.Ningresos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transacci__Ningr__5DCAEF64");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
