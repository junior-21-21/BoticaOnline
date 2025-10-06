// Pages/Clientes/Listado.cshtml.cs

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BoticaOnline.Data;
using BoticaOnline.Models;

namespace BoticaOnline.Web.Pages.Clientes
{
    public class ListadoModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ListadoModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Cliente> Clientes { get;set; } = new List<Cliente>();

        public async Task OnGetAsync()
        {
            // Carga todos los clientes que no están eliminados lógicamente
            Clientes = await _context.Clientes
                .Where(c => !c.Eliminado)
                .OrderBy(c => c.Apellido)
                .ToListAsync();
        }
    }
}