// Pages/Prescripciones/Subir.cshtml.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http; 
using BoticaOnline.Data;
using BoticaOnline.Models;
using Microsoft.AspNetCore.Hosting; // Necesario para IWebHostEnvironment
using System.IO;

namespace BoticaOnline.Web.Pages.Prescripciones
{
    public class SubirModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public SubirModel(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        [BindProperty]
        public Prescripcion Prescripcion { get; set; } = new Prescripcion();

        [BindProperty]
        public IFormFile? ArchivoPrescripcion { get; set; }


        public void OnGet()
        {
            // Muestra la vista
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ArchivoPrescripcion == null || ArchivoPrescripcion.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Debe seleccionar un archivo para subir.");
                return Page();
            }

            // 1. Guardar el archivo en el sistema de archivos
            string wwwRootPath = _hostEnvironment.WebRootPath;
            string uploadsFolder = Path.Combine(wwwRootPath, "prescripciones");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ArchivoPrescripcion.FileName);
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await ArchivoPrescripcion.CopyToAsync(fileStream);
            }

            // 2. Guardar la información en la base de datos
            Prescripcion.RutaArchivo = "/prescripciones/" + fileName;
            Prescripcion.EstadoValidacion = "Pendiente";
            Prescripcion.FechaSubida = DateTime.Now;

            _context.Prescripciones.Add(Prescripcion);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Prescripción subida y registrada exitosamente. Pendiente de validación.";

            return RedirectToPage("./Listado");
        }
    }
}