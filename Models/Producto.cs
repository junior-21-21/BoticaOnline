using System.ComponentModel.DataAnnotations;

namespace BoticaOnline.Models
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El nombre es obligatorio."), MaxLength(255)]
        public string Nombre { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La categoría es obligatoria."), MaxLength(100)]
        public string Categoria { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, 100000, ErrorMessage = "El precio debe ser positivo.")]
        public decimal Precio { get; set; }
        
        [Required(ErrorMessage = "El stock es obligatorio.")]
        [Range(0, 99999, ErrorMessage = "El stock debe ser un número entero.")]
        public int Stock { get; set; }
        
        // Eliminación lógica: por defecto FALSE (activo)
        public bool Eliminado { get; set; } = false; 
    }
}