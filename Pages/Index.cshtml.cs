using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BoticaOnline.Web.Pages
{
    public class IndexModel : PageModel
    {
        public string MensajeBienvenida { get; set; } = "Bienvenido al Sistema de Gestión de Inventario BoticaOnline.";

        public void OnGet()
        {
            // No se necesita lógica compleja, solo cargar la página.
        }
    }
}