using Microsoft.EntityFrameworkCore;
using tCRUD.Models;

namespace tCRUD.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Categoria)
                .WithMany(c => c.Productos)
                .HasForeignKey(p => p.CategoriaId)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<VentaItem>()
                .HasOne(i => i.Venta)
                .WithMany(v => v.Items)
                .HasForeignKey(i => i.VentaId)
                .OnDelete(DeleteBehavior.Cascade);      // sin ticket, las líneas no significan nada

            modelBuilder.Entity<VentaItem>()
                .HasOne(i => i.Producto)
                .WithMany()                              // Producto no necesita lista de VentaItems
                .HasForeignKey(i => i.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);      // el historial de ventas protege: no se borra un producto vendido
        }
    }
}