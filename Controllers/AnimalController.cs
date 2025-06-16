using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ProjetoInter.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;

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
            return Json(new { success = false, errors = erros });
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

        return Json(new { success = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AdicionarEspecie(AnimalEspecie especie)
    {
        if (ModelState.IsValid)
        {
            _context.AnimalEspecies.Add(especie);
            _context.SaveChanges();

            return Json(new
            {
                success = true,
                especie = new
                {
                    animalEspecieId = especie.AnimalEspecieId,
                    descricao = especie.Descricao
                }
            });
        }

        return Json(new { success = false, message = "Descrição inválida." });
    }

    // GET: /Animal/Editar/5
    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var animal = await _context.Animais
            .Include(a => a.Especie)
            .Include(a => a.Setor)
            .FirstOrDefaultAsync(a => a.AnimalId == id);

        if (animal == null) return NotFound();

        ViewBag.Especies = await _context.AnimalEspecies.ToListAsync();
        ViewBag.Setores = await _context.Setores.ToListAsync();

        return PartialView("_ModalEditarAnimal", animal);
    }

    // POST: /Animal/EditarAnimal
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarAnimal(Animal animal, IFormFile Imagem)
    {
        var erros = new List<string>();

        try
        {
            var animalExistente = await _context.Animais.FindAsync(animal.AnimalId); // Corrigido para AnimalId
            if (animalExistente == null)
            {
                erros.Add("Animal não encontrado");
                return Json(new { success = false, errors = erros });
            }

            // Atualiza propriedades
            animalExistente.Nome = animal.Nome;
            animalExistente.EspecieId = animal.EspecieId;
            animalExistente.Peso = animal.Peso;
            animalExistente.Sexo = animal.Sexo;
            animalExistente.DataNascimento = animal.DataNascimento;
            animalExistente.SetorId = animal.SetorId;

            // Tratamento de imagem
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

                // Remove imagem antiga se existir
                if (!string.IsNullOrEmpty(animalExistente.ImagemUrl))
                {
                    var oldFilePath = Path.Combine(_hostingEnvironment.WebRootPath, animalExistente.ImagemUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                animalExistente.ImagemUrl = "/uploads/" + fileName;
            }

            _context.Update(animalExistente);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            erros.Add($"Erro ao atualizar: {ex.Message}");
            return Json(new { success = false, errors = erros });
        }
    }
}