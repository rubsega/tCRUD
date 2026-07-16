using Microsoft.AspNetCore.Mvc;
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

    public async Task<IActionResult> Index()
    {
        var productos = await _context.Productos.ToListAsync();
        return View(productos);
    }

    public IActionResult Create()          // GET: muestra el formulario
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Producto producto)
    {
        if (ModelState.IsValid)
        {
            _context.Add(producto);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(producto);
    }
    public async Task<IActionResult> Edit(int id)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto == null)
        {
            return NotFound();
        }
        return View(producto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Producto producto)
    {
        if (id != producto.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            _context.Update(producto);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(producto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto == null) return NotFound();

        _context.Productos.Remove(producto);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}