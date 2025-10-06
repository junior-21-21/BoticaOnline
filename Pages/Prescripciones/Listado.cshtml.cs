// Pages/Prescripciones/Listado.cshtml.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BoticaOnline.Data;
using BoticaOnline.Models;
using System.Collections.Generic;
using System.Linq;

namespace BoticaOnline.Web.Pages.Prescripciones
{
    public class ListadoModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ListadoModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Prescripcion> Prescripciones { get; set; } = new List<Prescripcion>();

        [BindProperty]
        public string NuevoEstado { get; set; } = string.Empty;

        // --- Método GET: Cargar Prescripciones ---
        public async Task OnGetAsync()
        {
            Prescripciones = await _context.Prescripciones
                .Where(p => !p.Eliminado)
                .Include(p => p.Cliente)
                .OrderByDescending(p => p.FechaSubida)
                .ToListAsync();
        }

        // --- Método POST: Validar (Aprobar/Rechazar) Prescripción (Update) ---
        public async Task<IActionResult> OnPostValidarAsync(int id)
        {
            var prescripcion = await _context.Prescripciones.FindAsync(id);

            if (prescripcion == null) return NotFound();

            if (NuevoEstado != "Aprobada" && NuevoEstado != "Rechazada")
            {
                TempData["Message"] = "Error: Estado de validación no válido.";
                return RedirectToPage("./Listado");
            }
            
            prescripcion.EstadoValidacion = NuevoEstado;
            prescripcion.FechaRevision = DateTime.Now; 
            
            _context.Prescripciones.Update(prescripcion);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Prescripción #{id} marcada como {NuevoEstado} exitosamente.";

            return RedirectToPage("./Listado");
        }
    }
}