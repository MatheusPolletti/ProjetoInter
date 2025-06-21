using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class TransferenciaController : BaseController
{
    private readonly DbZoologico _context;

    public TransferenciaController(DbZoologico context) : base(context)
    {
        _context = context;
    }

    public async Task<IActionResult> Transferencias()
    {
        var transferencias = await _context.Transferencias
            .Include(t => t.Animal)
                        .Include(a => a.Animal)
                .ThenInclude(animal => animal.Setor)
            .Include(a => a.Animal)
                .ThenInclude(animal => animal.Especie)
            .Include(t => t.InstituicaoOrigem)
            .Include(t => t.InstituicaoDestino)
            .OrderByDescending(t => t.DataSaida)
            .ToListAsync();

        return View(transferencias);
    }
}