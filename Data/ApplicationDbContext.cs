// Data/ApplicationDbContext.cs

using Microsoft.EntityFrameworkCore;
using BoticaOnline.Models; 

namespace BoticaOnline.Data
{
    public class ApplicationDbContext : DbContext

    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
        {
        }
        // ... (Constructor existente)

        // DbSet para la tabla Productos
        public DbSet<Producto> Productos { get; set; } = default!;

        // DbSet para la tabla Clientes
        public DbSet<Cliente> Clientes { get; set; } = default!;

        // ¡NUEVO! DbSet para la tabla Pedidos
        public DbSet<Pedido> Pedidos { get; set; } = default!;

        // ¡NUEVO! DbSet para la tabla DetallePedido
        public DbSet<DetallePedido> DetallesPedido { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ... (Tu configuración existente de filtros, si la tienes)
        }
        public DbSet<Prescripcion> Prescripciones { get; set; } = default!;
    }
}