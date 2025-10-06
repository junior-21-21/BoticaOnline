// Controllers/ClientesController.cs

using BoticaOnline.Data;
using BoticaOnline.Models;
using BoticaOnline.Web.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class ClientesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ClientesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Clientes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClienteDTO>>> GetClientes()
    {
        var clientes = await _context.Clientes
            .Where(c => !c.Eliminado)
            .Select(c => new ClienteDTO 
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Apellido = c.Apellido,
                Email = c.Email,
                Telefono = c.Telefono
            })
            .ToListAsync();
        
        return clientes;
    }

    // GET: api/Clientes/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ClienteDTO>> GetCliente(int id)
    {
        var cliente = await _context.Clientes
            .Where(c => c.Id == id && !c.Eliminado)
            .Select(c => new ClienteDTO 
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Apellido = c.Apellido,
                Email = c.Email,
                Telefono = c.Telefono
            })
            .FirstOrDefaultAsync();

        if (cliente == null)
        {
            return NotFound();
        }

        return cliente;
    }

    // POST: api/Clientes
    [HttpPost]
    public async Task<ActionResult<ClienteDTO>> PostCliente(Cliente cliente)
    {
        // Validación básica, aunque se recomienda usar un DTO de creación
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        var clienteDto = new ClienteDTO
        {
            Id = cliente.Id,
            Nombre = cliente.Nombre,
            Apellido = cliente.Apellido,
            Email = cliente.Email,
            Telefono = cliente.Telefono
        };

        return CreatedAtAction(nameof(GetCliente), new { id = cliente.Id }, clienteDto);
    }
    
    // DELETE: api/Clientes/5 (Eliminación Lógica)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCliente(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente == null)
        {
            return NotFound();
        }

        cliente.Eliminado = true;
        _context.Clientes.Update(cliente);
        await _context.SaveChangesAsync();

        return NoContent(); // 204 No Content
    }
}