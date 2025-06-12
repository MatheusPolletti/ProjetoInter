using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ProjetoInter.Models;

[Authorize]
public class AnimalController : Controller
{
    private readonly DbZoologico _context;
    private readonly IWebHostEnvironment _hostingEnvironment;

    public AnimalController(DbZoologico context, IWebHostEnvironment hostingEnvironment)
    {
        _context = context;
        _hostingEnvironment = hostingEnvironment;
    }

    // GET: /Animal/Animais?busca=texto
    public async Task<IActionResult> Animais(string busca)
    {
        var query = _context.Animais
            .Include(a => a.Setor)
            .Include(a => a.Status)
            .Include(a => a.Especie)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(busca))
        {
            busca = busca.ToLower();
            query = query.Where(a => a.Nome.ToLower().Contains(busca));
        }

        var animais = await query.ToListAsync();

        ViewBag.Especies = await _context.AnimalEspecies.ToListAsync();
        ViewBag.Setores = await _context.Setores.ToListAsync();

        return View(animais);
    }

    // GET: /Animal/Novo - Retorna o partial view modal
    public async Task<IActionResult> Novo()
    {
        ViewBag.Especies = await _context.AnimalEspecies.ToListAsync();
        ViewBag.Setores = await _context.Setores.ToListAsync();
        return PartialView("_ModalNovoAnimal");
    }

    // POST: /Animal/AdicionarAnimal
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdicionarAnimal(Animal novoAnimal, IFormFile Imagem)
    {
        var erros = new List<string>();

        // Validações básicas
        if (string.IsNullOrWhiteSpace(novoAnimal.Nome))
            erros.Add("O nome do animal é obrigatório.");

        if (novoAnimal.Peso <= 0)
            erros.Add("Peso deve ser maior que zero.");

        if (erros.Count > 0)
        {
            // Coloque os erros no TempData para mostrar na view Animais (ou trate melhor depois)
            TempData["Erros"] = string.Join("; ", erros);
            return RedirectToAction("Animais");
        }

        if (Imagem != null && Imagem.Length > 0)
        {
            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Imagem.FileName);
            var filePath = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await Imagem.CopyToAsync(stream);
            }

            novoAnimal.ImagemUrl = "/uploads/" + fileName;
        }

        _context.Animais.Add(novoAnimal);
        await _context.SaveChangesAsync();

        return RedirectToAction("Animais");
    }
}