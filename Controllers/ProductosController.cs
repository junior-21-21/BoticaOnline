using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BoticaOnline.Data;
using BoticaOnline.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ProductosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProductosController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Productos (Listar)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
    {
        return await _context.Productos.ToListAsync();
    }

    // GET: api/Productos/5 (Obtener por ID)
    [HttpGet("{id}")]
    public async Task<ActionResult<Producto>> GetProducto(int id)
    {
        // FindAsync() busca directamente en la tabla por la clave primaria (Id)
        var producto = await _context.Productos.FindAsync(id); 
        
        if (producto == null) return NotFound();
        return producto;
    }

    // POST: api/Productos (Crear)
    [HttpPost]
    public async Task<ActionResult<Producto>> CreateProducto(Producto producto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        producto.Eliminado = false; 
        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProducto), new { id = producto.Id }, producto);
    }
    
    // PUT: api/Productos/5 (Actualizar)
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProducto(int id, Producto producto)
    {
        if (id != producto.Id) return BadRequest();
        
        // 1. Marcar el estado de la entidad como modificado.
        // Esto es más directo que buscar y copiar propiedades si ya tienes la entidad completa.
        _context.Entry(producto).State = EntityState.Modified;

        // 2. Asegurarse de que el campo 'Eliminado' no se sobrescriba accidentalmente
        // Esto es vital para mantener la eliminación lógica.
        _context.Entry(producto).Property(p => p.Eliminado).IsModified = false;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // Manejo de errores de concurrencia
            if (!await ProductoExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
        catch (DbUpdateException)
        {
             // Manejo de errores de base de datos (p.ej., violaciones de restricciones)
             return StatusCode(500, "Error al actualizar en la base de datos.");
        }

        return NoContent();
    }
    
    // DELETE: api/Productos/5 (Eliminación Lógica)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProducto(int id)
    {
        // 1. Usar IgnoreQueryFilters() para buscar un producto incluso si ya está eliminado.
        var producto = await _context.Productos
                                     .IgnoreQueryFilters()
                                     .FirstOrDefaultAsync(p => p.Id == id);
        
        if (producto == null || producto.Eliminado) 
            return NotFound("El producto no existe o ya fue eliminado.");

        // 2. Marcar como eliminado y guardar
        producto.Eliminado = true; 
        _context.Productos.Update(producto);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }

    private async Task<bool> ProductoExists(int id)
    {
        return await _context.Productos.AnyAsync(e => e.Id == id);
    }
}