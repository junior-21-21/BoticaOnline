
using System; 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BoticaOnline.Models
{
    public class Prescripcion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime FechaSubida { get; set; } = DateTime.Now;

        public DateTime? FechaRevision { get; set; }

        [Required]
        public string RutaArchivo { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string EstadoValidacion { get; set; } = "Pendiente";

        public bool Eliminado { get; set; } = false;

        public int? PedidoId { get; set; }
        public Pedido? Pedido { get; set; } 

        public int? ClienteId { get; set; }

        public Cliente? Cliente { get; set; } 
    }
}