// DTOs/ClienteDTO.cs

namespace BoticaOnline.Web.DTOs
{
    public class ClienteDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        // Se omite la propiedad 'Eliminado' para simplificar la API
    }
}