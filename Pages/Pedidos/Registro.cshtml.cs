// Pages/Pedidos/Registro.cshtml.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering; 
using BoticaOnline.Data;
using BoticaOnline.Models;
using BoticaOnline.Web.ViewModels; // Necesario para PedidoItemViewModel
using System.Linq; 
using System.Text.Json; 
using System.Collections.Generic; 
using System.Threading.Tasks; 

namespace BoticaOnline.Web.Pages.Pedidos
{
    public class RegistroModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public RegistroModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- Propiedades para Carga de Datos ---
        public SelectList ClientesSelectList { get; set; } = default!;
        public List<Producto> ProductosDisponibles { get; set; } = new List<Producto>();
        public SelectList PrescripcionesSelectList { get; set; } = default!; 

        // --- Propiedades para el Formulario (Input) ---
        [BindProperty]
        public int ClienteSeleccionadoId { get; set; } 

        [BindProperty]
        public string CarritoJson { get; set; } = "[]"; 
        
        [BindProperty]
        public int? PrescripcionSeleccionadaId { get; set; } // ID de la prescripción seleccionada


        // --- Método GET ---
        public async Task OnGetAsync()
        {
            // 1. Cargar Clientes
            var clientes = await _context.Clientes
                .Where(c => !c.Eliminado)
                .OrderBy(c => c.Apellido)
                .ToListAsync();
            
            // Asumiendo que has agregado la propiedad NombreCompleto al modelo Cliente
            ClientesSelectList = new SelectList(clientes, "Id", "NombreCompleto"); 

            // 2. Cargar Productos
            ProductosDisponibles = await _context.Productos
                .Where(p => !p.Eliminado && p.Stock > 0)
                .OrderBy(p => p.Nombre)
                .ToListAsync();
            
            // 3. Cargar Prescripciones Aprobadas y No Asociadas
            var prescripciones = await _context.Prescripciones
                .Where(p => p.EstadoValidacion == "Aprobada" && p.PedidoId == null)
                .OrderByDescending(p => p.FechaSubida)
                .ToListAsync();

            // Construir la SelectList usando SelectListItem para mayor seguridad
            var selectListItems = prescripciones.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"#{p.Id} - {p.Cliente?.NombreCompleto ?? "Cliente Eliminado"} - Subida el {p.FechaSubida.ToString("dd/MM/yyyy")}" 
            }).ToList();
            
            PrescripcionesSelectList = new SelectList(selectListItems, "Value", "Text");
        }

        // --- Método POST (Crear Pedido y Asociar Prescripción) ---
        public async Task<IActionResult> OnPostAsync()
        {
            List<PedidoItemViewModel>? carrito = null;
            
            // 1. Deserializar el carrito de forma segura
            try
            {
                carrito = JsonSerializer.Deserialize<List<PedidoItemViewModel>>(CarritoJson, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException)
            {
                ModelState.AddModelError(string.Empty, "Error al procesar los productos del carrito.");
                await OnGetAsync();
                return Page();
            }

            // 2. Validación de Datos Críticos
            if (ClienteSeleccionadoId <= 0 || carrito == null || carrito.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Debe seleccionar un cliente y agregar al menos un producto al carrito.");
                await OnGetAsync(); 
                return Page();
            }

            // 3. Iniciar el Pedido
            var nuevoPedido = new Pedido
            {
                ClienteId = ClienteSeleccionadoId,
                FechaPedido = DateTime.Now,
                Estado = "Pendiente",
                Total = 0m,
                PrescripcionId = PrescripcionSeleccionadaId, 
            };

            _context.Pedidos.Add(nuevoPedido);
            await _context.SaveChangesAsync(); 

            decimal totalPedido = 0;
            var detalles = new List<DetallePedido>();

            // 4. Procesar Detalles y Stock
            foreach (var item in carrito)
            {
                var productoBD = await _context.Productos.FindAsync(item.ProductoId);

                if (productoBD == null || productoBD.Stock < item.Cantidad)
                {
                    ModelState.AddModelError(string.Empty, $"Stock insuficiente para {item.NombreProducto}.");
                    
                    // Revertir el pedido y salir
                    _context.Pedidos.Remove(nuevoPedido);
                    await _context.SaveChangesAsync(); 
                    
                    await OnGetAsync();
                    return Page(); 
                }

                // Actualizar Stock y Total
                decimal subtotalItem = item.Cantidad * productoBD.Precio;
                totalPedido += subtotalItem;
                productoBD.Stock -= item.Cantidad; 

                // Crear el DetallePedido
                detalles.Add(new DetallePedido
                {
                    PedidoId = nuevoPedido.Id,
                    ProductoId = item.ProductoId,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = productoBD.Precio 
                });
            }

            // 5. Finalizar la Transacción
            nuevoPedido.Total = totalPedido;
            
            // Marcar Prescripción como usada
            if (PrescripcionSeleccionadaId.HasValue)
            {
                var prescripcion = await _context.Prescripciones.FindAsync(PrescripcionSeleccionadaId.Value);
                if (prescripcion != null)
                {
                    prescripcion.PedidoId = nuevoPedido.Id;
                    _context.Prescripciones.Update(prescripcion);
                }
            }

            _context.DetallesPedido.AddRange(detalles);
            
            // Guardar todos los cambios finales
            await _context.SaveChangesAsync();

            // 6. Éxito
            return RedirectToPage("./Listado");
        }
    }
}