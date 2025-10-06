using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BoticaOnline.Data;
using BoticaOnline.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoticaOnline.Web.Pages.Productos 
{
    // Asegúrate de que el nombre de la clase sea ListadoModel
    public class ListadoModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ListadoModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. ¡PROPIEDAD FALTANTE! La vista la necesita para el foreach y el .Any()
        public IList<Producto> Productos { get;set; } = new List<Producto>();
        
        // 2. ¡PROPIEDAD FALTANTE! La vista la necesita para el @if (!string.IsNullOrEmpty(...))
        public string ErrorMessage { get; set; } = string.Empty; 

// Pages/Productos/Listado.cshtml.cs

// ... (resto del código)

public async Task OnGetAsync()
{
    try
    {
        // ¡Filtro aplicado! Solo trae productos donde Eliminado es FALSE
        Productos = await _context.Productos
            .Where(p => !p.Eliminado) 
            .OrderBy(p => p.Nombre)
            .ToListAsync();
    }
    catch (Exception)
    {
        ErrorMessage = "No se pudo cargar el inventario. Verifique la conexión a la base de datos.";
        Productos = new List<Producto>();
    }
}
// ...
    }
}