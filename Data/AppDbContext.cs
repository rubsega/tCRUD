using Microsoft.EntityFrameworkCore;
using tCRUD.Models;

namespace tCRUD.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // --- Catálogos ---
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }

        // --- Ventas (salidas de inventario) ---
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<VentaItem> VentaItems { get; set; }

        // --- Compras (entradas de inventario) ---
        public DbSet<Compra> Compras { get; set; }
        public DbSet<CompraItem> CompraItems { get; set; }


        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== Producto — Categoria =====
            // Categoría opcional: borrar una categoría deja a sus productos
            // vivos y sin categoría, listos para reasignarse.
            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Categoria)
                .WithMany(c => c.Productos)
                .HasForeignKey(p => p.CategoriaId)
                .OnDelete(DeleteBehavior.SetNull);

            // ===== VentaItem — Venta (maestro-detalle) =====
            // Sin ticket, las líneas no significan nada: se van con él.
            modelBuilder.Entity<VentaItem>()
                .HasOne(i => i.Venta)
                .WithMany(v => v.Items)
                .HasForeignKey(i => i.VentaId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== VentaItem — Producto =====
            // El historial de ventas protege: no se borra un producto vendido.
            modelBuilder.Entity<VentaItem>()
                .HasOne(i => i.Producto)
                .WithMany()
                .HasForeignKey(i => i.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== CompraItem — Compra (maestro-detalle) =====
            modelBuilder.Entity<CompraItem>()
                .HasOne(i => i.Compra)
                .WithMany(c => c.Items)
                .HasForeignKey(i => i.CompraId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== CompraItem — Producto =====
            // Igual que en ventas: el historial de compras también protege.
            modelBuilder.Entity<CompraItem>()
                .HasOne(i => i.Producto)
                .WithMany()
                .HasForeignKey(i => i.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}