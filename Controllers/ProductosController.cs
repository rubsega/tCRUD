using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using tCRUD.Data;
using tCRUD.Models;

namespace tCRUD.Controllers;

public class ProductosController : Controller
{
    private readonly AppDbContext _context;

    public ProductosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /Productos
    public async Task<IActionResult> Index()
    {
        var productos = await _context.Productos
                                      .Include(p => p.Categoria)
                                      .Include(p => p.Proveedores)
                                      .OrderBy(p => p.Nombre)
                                      .ToListAsync();
        return View(productos);
    }

    // GET: /Productos/Create
    public IActionResult Create()
{
    ViewBag.Categorias = new SelectList(_context.Categorias.OrderBy(c => c.Nombre), "Id", "Nombre");
    ViewBag.Proveedores = _context.Proveedores.OrderBy(p => p.Nombre).ToList();
    return View();
}

    // POST: /Productos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Producto producto, List<int>? proveedorIds)
    {
        if (ModelState.IsValid)
        {
            if (proveedorIds is { Count: > 0 })
            {
                producto.Proveedores = await _context.Proveedores
                    .Where(p => proveedorIds.Contains(p.Id))
                    .ToListAsync();
            }

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            TempData["Ok"] = $"Producto creado: {producto.Nombre}";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categorias = new SelectList(_context.Categorias.OrderBy(c => c.Nombre), "Id", "Nombre", producto.CategoriaId);
        ViewBag.Proveedores = _context.Proveedores.OrderBy(p => p.Nombre).ToList();
        ViewBag.ProveedoresSeleccionados = proveedorIds;
        return View(producto);
    }

    // GET
    public async Task<IActionResult> Edit(Guid id)
    {
        var producto = await _context.Productos
                                    .Include(p => p.Proveedores)
                                    .FirstOrDefaultAsync(p => p.Uuid == id);
        if (producto == null) return NotFound();

        ViewBag.Categorias = new SelectList(_context.Categorias.OrderBy(c => c.Nombre), "Id", "Nombre", producto.CategoriaId);
        ViewBag.Proveedores = _context.Proveedores.OrderBy(p => p.Nombre).ToList();
        ViewBag.ProveedoresSeleccionados = producto.Proveedores.Select(p => p.Id).ToList();
        return View(producto);
    }

   [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Producto producto, List<int>? proveedorIds)
    {
    if (id != producto.Uuid) return NotFound();

        if (ModelState.IsValid)
        {
            // 1. Cargar la versión VIGILADA, con su colección actual
            var existente = await _context.Productos
                                        .Include(p => p.Proveedores)
                                        .FirstOrDefaultAsync(p => p.Uuid == id);
            if (existente == null) return NotFound();

            // 2. Mapear los campos del formulario al objeto vigilado
            existente.Nombre = producto.Nombre;
            existente.Precio = producto.Precio;
            existente.Stock = producto.Stock;
            existente.Descripcion = producto.Descripcion;
            existente.CategoriaId = producto.CategoriaId;

            // 3. Sincronizar la colección: lo marcado ES la verdad
            var nuevos = await _context.Proveedores
                .Where(p => proveedorIds != null && proveedorIds.Contains(p.Id))
                .ToListAsync();
            existente.Proveedores.Clear();
            foreach (var prov in nuevos)
                existente.Proveedores.Add(prov);

            await _context.SaveChangesAsync();
            TempData["Ok"] = $"Producto actualizado: {existente.Nombre}";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categorias = new SelectList(_context.Categorias.OrderBy(c => c.Nombre), "Id", "Nombre", producto.CategoriaId);
        ViewBag.Proveedores = _context.Proveedores.OrderBy(p => p.Nombre).ToList();
        ViewBag.ProveedoresSeleccionados = proveedorIds;
        return View(producto);
    }
    // POST: /Productos/Delete/{uuid}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var producto = await _context.Productos
                                     .FirstOrDefaultAsync(p => p.Uuid == id);

        if (producto == null) return NotFound();

        try
        {
            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            TempData["Ok"] = $"Producto eliminado: {producto.Nombre}";
        }
        catch (DbUpdateException)
        {
            // el Restrict de VentaItem/CompraItem protegiendo el historial
            TempData["Error"] = $"No se puede eliminar {producto.Nombre}: tiene ventas o compras registradas.";
        }

        return RedirectToAction(nameof(Index));
    }

}