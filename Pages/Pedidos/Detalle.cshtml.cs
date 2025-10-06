// Pages/Pedidos/Detalle.cshtml.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BoticaOnline.Data;
using BoticaOnline.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoticaOnline.Web.Pages.Pedidos
{
    public class DetalleModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetalleModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Pedido? Pedido { get; set; }

        [BindProperty]
        public string NuevoEstado { get; set; } = string.Empty;

        public List<string> EstadosDisponibles { get; set; } = new List<string>
        {
            "Pendiente", "Procesando", "Enviado", "Completado"
        };


        public async Task<IActionResult> OnGetAsync(int id)
        {
            // CÓDIGO CORREGIDO PARA INCLUIR TODAS LAS RELACIONES NECESARIAS
            Pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Prescripcion) // Nuevo: Incluir la prescripción
                .Include(p => p.DetallesPedido)! // Usamos '!' para asegurar que no es null si la lista existe
                    .ThenInclude(dp => dp.Producto)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (Pedido == null)
            {
                return NotFound();
            }

            // Inicializar NuevoEstado con el estado actual
            NuevoEstado = Pedido.Estado;
            
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(int id)
        {
            Pedido = await _context.Pedidos.FindAsync(id);

            if (Pedido == null)
            {
                return NotFound();
            }

            Pedido.Estado = NuevoEstado;
            _context.Pedidos.Update(Pedido);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"El estado del pedido #{id} ha sido actualizado a '{NuevoEstado}'.";
            return RedirectToPage(new { id });
        }
        
        public async Task<IActionResult> OnPostCancelAsync(int id)
        {
            Pedido = await _context.Pedidos
                .Include(p => p.DetallesPedido)!
                    .ThenInclude(dp => dp.Producto)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (Pedido == null)
            {
                return NotFound();
            }
            
            if (Pedido.Estado == "Cancelado" || Pedido.Estado == "Completado")
            {
                TempData["Message"] = "No se puede cancelar un pedido que ya está finalizado.";
                return RedirectToPage(new { id });
            }
            
            // 1. Devolver el stock
            foreach (var detalle in Pedido.DetallesPedido) // <--- CORRECCIÓN CLAVE
            {
                if (detalle.Producto != null)
                {
                    detalle.Producto.Stock += detalle.Cantidad;
                    _context.Productos.Update(detalle.Producto);
                }
            }

            // 2. Cambiar estado
            Pedido.Estado = "Cancelado";
            _context.Pedidos.Update(Pedido);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"El pedido #{id} ha sido CANCELADO y el stock ha sido devuelto.";
            return RedirectToPage(new { id });
        }
    }
}