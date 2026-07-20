using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tCRUD.Data;
using tCRUD.Models;

namespace tCRUD.Controllers;

public class ProveedoresController : Controller
{
    private readonly AppDbContext _context;

    public ProveedoresController(AppDbContext context)
    {
        _context = context;
    } 

    public async Task<IActionResult> Index()
    {
        var proveedores = await _context.Proveedores
                                        .Include(p => p.Productos)
                                        .ToListAsync();
        return View(proveedores);
    }

    public IActionResult Create()          // GET: muestra el formulario
    {
        ViewBag.Productos = _context.Productos.OrderBy(p => p.Nombre).ToList();
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Proveedor proveedor, List<int>? productoIds)
    {
        if (ModelState.IsValid)
        {
            if (productoIds is { Count: > 0 })
            {
                proveedor.Productos = await _context.Productos
                    .Where(p => productoIds.Contains(p.Id))
                    .ToListAsync();
            }
            _context.Add(proveedor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Productos = _context.Productos.OrderBy(p => p.Nombre).ToList();
        ViewBag.ProductosSeleccionados = productoIds;
        return View(proveedor);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var proveedor = await _context.Proveedores
                                        .Include(p => p.Productos)
                                        .FirstOrDefaultAsync(p => p.Id == id);
        if (proveedor == null)
        {
            return NotFound();
        }
        ViewBag.Productos = _context.Productos.OrderBy(p => p.Nombre).ToList();
        ViewBag.ProductosSeleccionados = proveedor.Productos.Select(p => p.Id).ToList();
        return View(proveedor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Proveedor proveedor, List<int>? productoIds)
    {
        if (id != proveedor.Id) return NotFound();

        if (ModelState.IsValid)
        {
            // 1. Cargar la versión VIGILADA, con su colección actual de productos
            var existente = await _context.Proveedores
                                        .Include(p => p.Productos)
                                        .FirstOrDefaultAsync(p => p.Id == id);
            if (existente == null) return NotFound();

            // 2. Copiar los campos escalares del formulario al objeto vigilado
            existente.Nombre = proveedor.Nombre;
            existente.Direccion = proveedor.Direccion;
            existente.Telefono = proveedor.Telefono;
            existente.Email = proveedor.Email;
            // ...agrega aquí los demás campos que tenga tu Proveedor (Telefono, Email, etc.)...

            // 3. Sincronizar la colección: lo marcado ES la verdad
            var nuevos = await _context.Productos
                .Where(p => productoIds != null && productoIds.Contains(p.Id))
                .ToListAsync();
            existente.Productos.Clear();
            foreach (var prod in nuevos)
                existente.Productos.Add(prod);

            await _context.SaveChangesAsync();
            TempData["Ok"] = $"Proveedor actualizado: {existente.Nombre}";
            return RedirectToAction(nameof(Index));
        }

        // Camino triste: repón lo que la vista necesita y devuelve
        ViewBag.Productos = await _context.Productos.OrderBy(p => p.Nombre).ToListAsync();
        ViewBag.ProductosSeleccionados = productoIds;
        return View(proveedor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var proveedor = await _context.Proveedores.FindAsync(id);
        if (proveedor == null) return NotFound();

        _context.Proveedores.Remove(proveedor);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}