// DTOs/PedidoDTO.cs

using System;
using System.Collections.Generic;

namespace BoticaOnline.Web.DTOs
{
    public class PedidoDTO
    {
        public int Id { get; set; }
        public string ClienteNombreCompleto { get; set; } = string.Empty;
        public DateTime FechaPedido { get; set; }
        public string Estado { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public int? PrescripcionId { get; set; }
        
        public List<DetallePedidoDTO> Detalles { get; set; } = new List<DetallePedidoDTO>();
    }

    public class DetallePedidoDTO
    {
        public string ProductoNombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}