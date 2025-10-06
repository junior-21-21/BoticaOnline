// Pages/Productos/Edit.cshtml.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BoticaOnline.Data;
using BoticaOnline.Models;

namespace BoticaOnline.Web.Pages.Productos
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Producto Producto { get; set; } = default!;

        // 1. Cargar el producto (GET)
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            // Buscar el producto, ignorando filtros para poder editar
            var producto = await _context.Productos.IgnoreQueryFilters().FirstOrDefaultAsync(m => m.Id == id);

            if (producto == null) return NotFound();
            
            Producto = producto;
            return Page();
        }

        // 2. Guardar los cambios (POST/UPDATE)
        public async Task<IActionResult> OnPostAsync()
        {
            // Verifica la validez del modelo (basado en anotaciones de Producto.cs)
            if (!ModelState.IsValid)
            {
                return Page(); 
            }

            // Usar Attach y Entry para marcar el objeto como modificado
            _context.Attach(Producto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Productos.Any(e => e.Id == Producto.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            
            // Redirigir al listado de productos
            return RedirectToPage("./Listado");
        }
        
        // 3. Eliminar el producto (Eliminación lógica) (POST/DELETE)
        public async Task<IActionResult> OnPostDeleteAsync(int? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Productos.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == id);

            if (producto != null)
            {
                producto.Eliminado = true; 
                _context.Productos.Update(producto);
                await _context.SaveChangesAsync();
            }

            // Redirigir al listado de productos
            return RedirectToPage("./Listado"); 
        }
    }
}