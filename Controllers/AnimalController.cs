using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ProjetoInter.Models;

[Authorize]
public class AnimalController : BaseController
{
    private readonly IWebHostEnvironment _hostingEnvironment;

    public AnimalController(DbZoologico _context, IWebHostEnvironment hostingEnvironment) : base(_context)
    {
        _hostingEnvironment = hostingEnvironment;
    }

    // GET: /Animal/Animais?busca=texto
    public async Task<IActionResult> Animais(string busca)
    {
        var query = context.Animais
            .Include(a => a.Setor)
            .Include(a => a.Status)
            .Include(a => a.Especie)
            .Where(a => a.StatusId != 5 && a.StatusId != 6)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(busca))
        {
            busca = busca.ToLower();
            query = query.Where(a => a.Nome.ToLower().Contains(busca));
        }

        var animais = await query.ToListAsync();

        ViewBag.Especies = await context.AnimalEspecies.ToListAsync();
        ViewBag.Setores = await context.Setores.ToListAsync();

        return View(animais);
    }

    // GET: /Animal/Novo - Retorna o partial view modal
    public async Task<IActionResult> Novo()
    {
        ViewBag.Especies = await context.AnimalEspecies.ToListAsync();
        ViewBag.Setores = await context.Setores.ToListAsync();
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
            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "img", "Animais");

            if (!Directory.Exists(uploads))
            {
                try
                {
                    Directory.CreateDirectory(uploads);
                }
                catch (Exception ex)
                {
                    erros.Add("Erro no servidor ao processar imagem");
                    return Json(new { success = false, errors = erros });
                }
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Imagem.FileName);
            var filePath = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await Imagem.CopyToAsync(stream);
            }

            novoAnimal.ImagemUrl = "/img/Animais/" + fileName;
        }

        novoAnimal.StatusId = 1; // Status "Ativo"

        context.Animais.Add(novoAnimal);
        await context.SaveChangesAsync();

        return Json(new { success = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AdicionarEspecie(AnimalEspecie especie)
    {
        if (ModelState.IsValid)
        {
            context.AnimalEspecies.Add(especie);
            context.SaveChanges();

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
        var animal = await context.Animais
            .Include(a => a.Especie)
            .Include(a => a.Setor)
            .FirstOrDefaultAsync(a => a.AnimalId == id);

        if (animal == null) return NotFound();

        ViewBag.Especies = await context.AnimalEspecies.ToListAsync();
        ViewBag.Setores = await context.Setores.ToListAsync();

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
            var animalExistente = await context.Animais.FindAsync(animal.AnimalId);
            if (animalExistente == null)
            {
                erros.Add("Animal não encontrado");
                return Json(new { success = false, errors = erros });
            }

            animalExistente.Nome = animal.Nome;
            animalExistente.EspecieId = animal.EspecieId;
            animalExistente.Peso = animal.Peso;
            animalExistente.Sexo = animal.Sexo;
            animalExistente.DataNascimento = animal.DataNascimento;
            animalExistente.SetorId = animal.SetorId;

            if (Imagem != null && Imagem.Length > 0)
            {
                var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "img", "Animais");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Imagem.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Imagem.CopyToAsync(stream);
                }

                if (!string.IsNullOrEmpty(animalExistente.ImagemUrl))
                {
                    var oldFilePath = Path.Combine(
                        _hostingEnvironment.WebRootPath,
                        animalExistente.ImagemUrl.TrimStart('/')
                    );

                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                animalExistente.ImagemUrl = "/img/Animais/" + fileName;
            }

            context.Update(animalExistente);
            await context.SaveChangesAsync();

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            erros.Add($"Erro ao atualizar: {ex.Message}");
            return Json(new { success = false, errors = erros });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExcluirAnimais([FromBody] ExcluirAnimaisModel model)
    {
        try
        {
            const int STATUS_INATIVO = 6; // Status "Inativo"

            var animais = await context.Animais
                .Where(a => model.AnimalIds.Contains(a.AnimalId))
                .ToListAsync();

            foreach (var animal in animais)
            {
                // Verificar se existem tarefas pendentes para o animal
                var tarefasPendentes = await context.Procedimentos
                    .AnyAsync(p => p.AnimalId == animal.AnimalId && !p.Status);

                if (tarefasPendentes)
                {
                    return Json(new { 
                        success = false, 
                        message = $"Não é possível excluir o animal {animal.Nome} pois existem tarefas pendentes associadas a ele." 
                    });
                }

                // Verificar se existem transferências pendentes para o animal
                var transferenciasPendentes = await context.Transferencias
                    .AnyAsync(t => t.AnimalId == animal.AnimalId && t.Status);

                if (transferenciasPendentes)
                {
                    return Json(new { 
                        success = false, 
                        message = $"Não é possível excluir o animal {animal.Nome} pois existem transferências pendentes associadas a ele." 
                    });
                }

                // Excluir tarefas concluídas associadas ao animal
                var tarefasConcluidas = await context.Procedimentos
                    .Where(p => p.AnimalId == animal.AnimalId && p.Status)
                    .ToListAsync();

                if (tarefasConcluidas.Any())
                {
                    context.Procedimentos.RemoveRange(tarefasConcluidas);
                }

                animal.StatusId = STATUS_INATIVO; // Marca como inativo

                if (model.AnimalIds.Count == 1 &&
                    !string.IsNullOrEmpty(model.DataFalecimento) &&
                    DateOnly.TryParse(model.DataFalecimento, out DateOnly dataFalec))
                {
                    animal.DataFalecimento = dataFalec;
                }
            }

            await context.SaveChangesAsync();
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    public async Task<IActionResult> Detalhes(int id)
    {
        var animal = await context.Animais
            .Include(a => a.Especie)
            .Include(a => a.Setor)
            .Include(a => a.Status)
            .FirstOrDefaultAsync(a => a.AnimalId == id);

        if (animal == null) return NotFound();

        var atendimentos = await context.AtendimentosVeterinarios
            .Include(a => a.FuncionarioVeterinario)
            .Include(a => a.FuncionarioSolicitante)
            .Where(a => a.AnimalId == id)
            .OrderByDescending(a => a.Data)
            .ToListAsync();

        ViewBag.Animal = animal;
        ViewBag.Atendimentos = atendimentos;

        return View("DetalheAnimal");
    }

    public class ExcluirAnimaisModel
    {
        public List<int> AnimalIds { get; set; }
        public string DataFalecimento { get; set; }
    }
}