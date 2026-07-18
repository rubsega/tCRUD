using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tCRUD.Data;
using tCRUD.Models;

namespace tCRUD.Controllers;

public class ComprasController : Controller
{
    private readonly AppDbContext _context;

    public ComprasController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /Compras
    public async Task<IActionResult> Index()
    {
        var compras = await _context.Compras
                                    .Include(c => c.Items)
                                    .OrderByDescending(c => c.Fecha)
                                    .ToListAsync();
        return View(compras);
    }

    // GET: /Compras/Items/5
    public async Task<IActionResult> Items(int id)
    {
        var compra = await _context.Compras
                                   .Include(c => c.Items)
                                   .ThenInclude(i => i.Producto)
                                   .FirstOrDefaultAsync(c => c.Id == id);

        if (compra == null) return NotFound();
        return View(compra);
    }

    // GET: /Compras/Create
    public async Task<IActionResult> Create()
    {
        ViewBag.Productos = await _context.Productos
                                          .OrderBy(p => p.Nombre)
                                          .ToListAsync();
        return View(new Compra());
    }

    // POST: /Compras/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Compra compra)
    {
        // --- Validaciones de forma ---
        if (compra.Items == null || compra.Items.Count == 0)
            ModelState.AddModelError("", "Agrega al menos un producto a la compra.");
        else if (compra.Items.GroupBy(i => i.ProductoId).Any(g => g.Count() > 1))
            ModelState.AddModelError("", "Hay productos repetidos en la compra.");

        if (ModelState.IsValid)
        {
            // --- Primera pasada: validar todo sin tocar nada ---
            foreach (var item in compra.Items)
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
                if (item.PrecioCompra <= 0)
                {
                    ModelState.AddModelError("", $"Precio inválido para {producto.Nombre}.");
                    break;
                }
            }

            if (ModelState.IsValid)
            {
                // --- Segunda pasada: aplicar cambios ---
                foreach (var item in compra.Items)
                {
                    var producto = await _context.Productos.FindAsync(item.ProductoId);
                    producto!.Stock += item.Cantidad;      // entrada de inventario: SUMA
                    // el PrecioUnitario NO se pisa: es captura del usuario (costo real pagado)
                }

                compra.Fecha = DateTime.UtcNow;

                _context.Compras.Add(compra);              // graph add: maestro + líneas
                await _context.SaveChangesAsync();          // una transacción: inserts + updates de stock

                TempData["Ok"] = $"Compra registrada: {compra.Total:C}";
                return RedirectToAction(nameof(Index));
            }
        }

        // --- Camino triste: reponer el ViewBag ---
        ViewBag.Productos = await _context.Productos
                                          .OrderBy(p => p.Nombre)
                                          .ToListAsync();
        return View(compra);
    }
}