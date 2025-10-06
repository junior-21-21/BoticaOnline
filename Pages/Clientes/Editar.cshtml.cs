// Pages/Clientes/Editar.cshtml.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BoticaOnline.Data;
using BoticaOnline.Models;

namespace BoticaOnline.Web.Pages.Clientes
{
    public class EditarModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditarModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Cliente Cliente { get; set; } = default!; // ¡Cambiado a Cliente!

        // 1. Cargar el cliente (GET)
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            // Buscar el cliente, ignorando filtros para poder editar
            var cliente = await _context.Clientes.IgnoreQueryFilters().FirstOrDefaultAsync(m => m.Id == id); // ¡Cambiado a Clientes!

            if (cliente == null) return NotFound();
            
            Cliente = cliente;
            return Page();
        }

        // 2. Guardar los cambios (POST/UPDATE)
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page(); 
            }

            _context.Attach(Cliente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // ¡Cambiado a Clientes!
                if (!_context.Clientes.Any(e => e.Id == Cliente.Id)) 
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            
            // Redirigir al listado de clientes
            return RedirectToPage("./Listado"); 
        }
        
        // 3. Eliminar el cliente (Eliminación lógica) (POST/DELETE)
        public async Task<IActionResult> OnPostDeleteAsync(int? id)
        {
            if (id == null) return NotFound();

            // ¡Cambiado a Clientes!
            var cliente = await _context.Clientes.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == id); 

            if (cliente != null)
            {
                cliente.Eliminado = true; 
                _context.Clientes.Update(cliente);
                await _context.SaveChangesAsync();
            }

            // Redirigir al listado de clientes
            return RedirectToPage("./Listado"); 
        }
    }
}