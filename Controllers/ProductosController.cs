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
}