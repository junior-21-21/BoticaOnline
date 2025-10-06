// Controllers/PedidosController.cs

using BoticaOnline.Data;
using BoticaOnline.Models;
using BoticaOnline.Web.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class PedidosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PedidosController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Pedidos (Listado de todos los pedidos)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PedidoDTO>>> GetPedidos()
    {
        var pedidos = await _context.Pedidos
            .Include(p => p.Cliente)
            .OrderByDescending(p => p.FechaPedido)
            .Select(p => new PedidoDTO
            {
                Id = p.Id,
                ClienteNombreCompleto = p.Cliente != null ? $"{p.Cliente.Nombre} {p.Cliente.Apellido}" : "Cliente Desconocido",
                FechaPedido = p.FechaPedido,
                Estado = p.Estado,
                Total = p.Total,
                PrescripcionId = p.PrescripcionId,
            })
            .ToListAsync();

        return pedidos;
    }

    // GET: api/Pedidos/5 (Detalle de un pedido con sus ítems)
    [HttpGet("{id}")]
    public async Task<ActionResult<PedidoDTO>> GetPedido(int id)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.DetallesPedido)!
                .ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pedido == null)
        {
            return NotFound();
        }

        var pedidoDto = new PedidoDTO
        {
            Id = pedido.Id,
            ClienteNombreCompleto = pedido.Cliente != null ? $"{pedido.Cliente.Nombre} {pedido.Cliente.Apellido}" : "Cliente Desconocido",
            FechaPedido = pedido.FechaPedido,
            Estado = pedido.Estado,
            Total = pedido.Total,
            PrescripcionId = pedido.PrescripcionId,
            Detalles = pedido.DetallesPedido.Select(d => new DetallePedidoDTO
            {
                ProductoNombre = d.Producto?.Nombre ?? "Producto Eliminado",
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario
            }).ToList()
        };

        return pedidoDto;
    }
    
    // Nota: El POST (Crear Pedido) requiere un DTO de entrada más complejo y es sensible a la lógica de negocio (stock, total),
    // pero los endpoints GET ya están listos para ser consumidos.
}