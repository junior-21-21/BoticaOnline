// Models/DetallePedido.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoticaOnline.Models
{
    public class DetallePedido
    {
        public int Id { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        // Guardar el precio al que se vendió, no el precio actual del producto
        [Column(TypeName = "decimal(10, 2)")]
        public decimal PrecioUnitario { get; set; }

        // Relación con Pedido (Foreign Key)
        public int PedidoId { get; set; }
        public Pedido Pedido { get; set; } = default!;

        // Relación con Producto (Foreign Key)
        public int ProductoId { get; set; }
        public Producto Producto { get; set; } = default!;
    }
}