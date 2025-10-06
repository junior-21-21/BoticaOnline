// ViewModels/PedidoItemViewModel.cs

namespace BoticaOnline.Web.ViewModels
{
    // Usado temporalmente en la vista para manejar un producto en el carrito.
    public class PedidoItemViewModel
    {
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
    }
}