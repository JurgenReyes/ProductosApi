using Microsoft.AspNetCore.Mvc;
using ProductosApi.Models;
using ProductosApi.Repositories;

namespace ProductosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly IProductoRepository _repo;

    public ProductosController(IProductoRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public IActionResult ObtenerTodos() => Ok(_repo.ObtenerTodos());

    [HttpGet("{id}")]
    public IActionResult ObtenerPorId(int id)
    {
        var p = _repo.ObtenerPorId(id);
        return p is null ? NotFound() : Ok(p);
    }

    [HttpGet("buscar")]
    public IActionResult Buscar([FromQuery] string nombre) => Ok(_repo.Buscar(nombre));

    [HttpPost]
    public IActionResult Insertar(Producto producto)
    {
        producto.Id = _repo.Insertar(producto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = producto.Id }, producto);
    }

    [HttpPut("{id}")]
    public IActionResult Actualizar(int id, Producto producto)
    {
        producto.Id = id;
        return _repo.Actualizar(producto) ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id) => _repo.Eliminar(id) ? NoContent() : NotFound();
}
