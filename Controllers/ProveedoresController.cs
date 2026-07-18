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
        var proveedores = await _context.Proveedores.ToListAsync();
        return View(proveedores);
    }

    public IActionResult Create()          // GET: muestra el formulario
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Proveedor proveedor)
    {
        if (ModelState.IsValid)
        {
            _context.Add(proveedor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(proveedor);
    }
    public async Task<IActionResult> Edit(int id)
    {
        var proveedor = await _context.Proveedores.FindAsync(id);
        if (proveedor == null)
        {
            return NotFound();
        }
        return View(proveedor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Proveedor proveedor)
    {
        if (id != proveedor.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            _context.Update(proveedor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
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