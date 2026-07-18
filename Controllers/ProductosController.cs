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
                                      .OrderBy(p => p.Nombre)
                                      .ToListAsync();
        return View(productos);
    }

    // GET: /Productos/Create
    public IActionResult Create()
    {
        ViewBag.Categorias = new SelectList(_context.Categorias.OrderBy(c => c.Nombre), "Id", "Nombre");
        return View();
    }

    // POST: /Productos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Producto producto)
    {
        if (ModelState.IsValid)
        {
            _context.Productos.Add(producto);          // el Uuid ya viene puesto: nació en el new
            await _context.SaveChangesAsync();
            TempData["Ok"] = $"Producto creado: {producto.Nombre}";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categorias = new SelectList(_context.Categorias.OrderBy(c => c.Nombre), "Id", "Nombre", producto.CategoriaId);
        return View(producto);
    }

    // GET: /Productos/Edit/{uuid}
    public async Task<IActionResult> Edit(Guid id)
    {
        var producto = await _context.Productos
                                     .FirstOrDefaultAsync(p => p.Uuid == id);

        if (producto == null) return NotFound();

        ViewBag.Categorias = new SelectList(_context.Categorias.OrderBy(c => c.Nombre), "Id", "Nombre", producto.CategoriaId);
        return View(producto);
    }

    // POST: /Productos/Edit/{uuid}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Producto producto)
    {
        if (id != producto.Uuid) return NotFound();     // guard: la URL y el form deben coincidir

        if (ModelState.IsValid)
        {
            _context.Update(producto);
            await _context.SaveChangesAsync();
            TempData["Ok"] = $"Producto actualizado: {producto.Nombre}";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categorias = new SelectList(_context.Categorias.OrderBy(c => c.Nombre), "Id", "Nombre", producto.CategoriaId);
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