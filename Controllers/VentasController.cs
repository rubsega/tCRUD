using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using tCRUD.Data;
using tCRUD.Models;

namespace tCRUD.Controllers;

public class VentasController : Controller

{
    private readonly AppDbContext _context;

    public VentasController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var ventas = await _context.Ventas
                                .Include(v => v.Items)
                                .OrderByDescending(v => v.Fecha)
                                .ToListAsync();
        return View(ventas);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Productos = await _context.Productos
                                        .Where(p => p.Stock > 0)
                                        .OrderBy(p => p.Nombre)
                                        .ToListAsync();
        return View(new Venta());
    }

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(Venta venta)
{
    // --- Validaciones de forma ---
    if (venta.Items == null || venta.Items.Count == 0)
        ModelState.AddModelError("", "Agrega al menos un producto al carrito.");
    else if (venta.Items.GroupBy(i => i.ProductoId).Any(g => g.Count() > 1))
        ModelState.AddModelError("", "Hay productos repetidos en la venta.");

    if (ModelState.IsValid)
    {
        // --- Primera pasada: validar TODO sin tocar nada ---
        foreach (var item in venta.Items)
        {
            var producto = await _context.Productos.FindAsync(item.ProductoId);

            if (producto == null)
            {
                ModelState.AddModelError("", "Uno de los productos no existe.");
                break;
            }
            if (item.Cantidad < 1)
            {
                ModelState.AddModelError("", $"Cantidad inválida para {producto.Nombre}.");
                break;
            }
            if (producto.Stock < item.Cantidad)
                ModelState.AddModelError("", $"Stock insuficiente de {producto.Nombre} (quedan {producto.Stock}).");
        }

        if (ModelState.IsValid)
        {
            // --- Segunda pasada: aplicar los cambios ---
            foreach (var item in venta.Items)
            {
                var producto = await _context.Productos.FindAsync(item.ProductoId);

                item.PrecioUnitario = producto!.Precio;   // snapshot: el precio lo decide el SERVIDOR
                producto.Stock -= item.Cantidad;          // producto vigilado → UPDATE quirúrgico
            }

            venta.Fecha = DateTime.UtcNow;

            _context.Ventas.Add(venta);                   // graph add: maestro + líneas juntos
            await _context.SaveChangesAsync();            // UNA transacción: inserts + updates de stock
            return RedirectToAction(nameof(Index));
        }
    }

    ViewBag.Productos = new SelectList(_context.Productos.Where(p => p.Stock > 0), "Id", "Nombre");
    return View(venta);
    }

    public async Task<IActionResult> Items(int id)
    {
        var venta = await _context.Ventas
                                .Include(v => v.Items)
                                .ThenInclude(i => i.Producto)
                                .FirstOrDefaultAsync(v => v.Id == id);

        if (venta == null) return NotFound();
        return View(venta);
    }
}