// Models/Pedido.cs

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoticaOnline.Models
{
    public class Pedido
    {
        [Key]
        public int Id { get; set; }

        // Propiedades existentes
        public int ClienteId { get; set; }
        public DateTime FechaPedido { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public decimal Total { get; set; }

        // --- RELACIÓN CON PRESCRIPCIÓN (Asegúrate de que exista) ---
        public int? PrescripcionId { get; set; } // DEBE SER NULLABLE (int?)
        
        [ForeignKey("PrescripcionId")]
        public Prescripcion? Prescripcion { get; set; }
        
        // Relaciones de navegación
        public Cliente? Cliente { get; set; }
        public ICollection<DetallePedido> DetallesPedido { get; set; } = new List<DetallePedido>();
    }
}