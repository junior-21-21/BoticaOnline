// Pages/Pedidos/Listado.cshtml.cs

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BoticaOnline.Data;
using BoticaOnline.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoticaOnline.Web.Pages.Pedidos
{
    public class ListadoModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ListadoModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Propiedad para almacenar la lista de pedidos
        public IList<Pedido> Pedidos { get; set; } = new List<Pedido>();

        public async Task OnGetAsync()
        {
            // Carga los pedidos e incluye la entidad Cliente para mostrar su nombre
            Pedidos = await _context.Pedidos
                .Include(p => p.Cliente) // Incluir el cliente relacionado
                .OrderByDescending(p => p.FechaPedido)
                .ToListAsync();
        }
    }
}