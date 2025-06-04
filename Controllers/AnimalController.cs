using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class AnimalController : Controller
{
    private readonly DbZoologico context;

    public AnimalController(DbZoologico _context)
    {
        context = _context;
    }

    public async Task<IActionResult> Animais(string busca)
    {
        var query = context.Animais
            .Include(a => a.Setor)
            .Include(a => a.Status)
            .Include(a => a.Especie)
            .AsQueryable();

        if (!string.IsNullOrEmpty(busca))
        {
            query = query.Where(a => a.Nome.ToLower().Contains(busca.ToLower()));
        }

        var animais = await query.ToListAsync();

        return View(animais);
    }
}