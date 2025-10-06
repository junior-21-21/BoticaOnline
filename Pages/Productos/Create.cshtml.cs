using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BoticaOnline.Data;
using BoticaOnline.Models;

namespace BoticaOnline.Web.Pages.Productos 
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Producto Producto { get; set; } = new Producto();

        public IActionResult OnGet()
        {
            return Page();
        }

        // Pages/Productos/Create.cshtml.cs

public async Task<IActionResult> OnPostAsync()
{
    // 1. **VERIFICAR MODELSTATE:** Usa la propiedad [BindProperty]
    // Si la validación (basada en atributos [Required], [Range], etc., en Producto.cs) falla,
    // el código salta al 'return Page()', recargando la página con los errores.
    if (!ModelState.IsValid)
    {
        return Page();
    }
    
    // 2. Si es válido, usar directamente el objeto [BindProperty] Producto
    
    // Asegurar que el campo de eliminación esté en FALSE
    Producto.Eliminado = false; 

    _context.Productos.Add(Producto);
    await _context.SaveChangesAsync();

    // 3. Redirigir al listado (ruta absoluta)
    return RedirectToPage("/Productos/Listado"); 
}
    }
}