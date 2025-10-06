// Pages/Clientes/Registro.cshtml.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BoticaOnline.Data;
using BoticaOnline.Models;

namespace BoticaOnline.Web.Pages.Clientes
{
    // El nombre de la clase debe coincidir con el nombre del archivo: RegistroModel
    public class RegistroModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public RegistroModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // [BindProperty] enlaza automáticamente los datos del formulario a este objeto
        [BindProperty]
        public Cliente Cliente { get; set; } = new Cliente();

        public IActionResult OnGet()
        {
            return Page(); // Simplemente muestra el formulario vacío
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // 1. Verificar la validación (basada en atributos [Required], [EmailAddress], etc. en Cliente.cs)
            if (!ModelState.IsValid)
            {
                return Page(); // Regresa al formulario mostrando los errores
            }

            // 2. Lógica de Negocio
            Cliente.Eliminado = false; // Asegura que el cliente está activo por defecto

            // 3. Guardar en la base de datos
            _context.Clientes.Add(Cliente);
            await _context.SaveChangesAsync();

            // 4. Redirigir al listado de clientes
            return RedirectToPage("./Listado"); 
        }
    }
}