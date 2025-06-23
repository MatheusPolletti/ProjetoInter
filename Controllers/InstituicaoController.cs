using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using ProjetoInter.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting; // Adicione este
using Microsoft.Extensions.Configuration;

[Authorize]
public class InstituicaoController : BaseController // Assumindo que você tem um BaseController que injeta o DbContext
{
    private readonly IConfiguration _configuration; // Para acessar configurações (ex: caminhos de upload)
    private readonly IWebHostEnvironment _webHostEnvironment; // Para obter o caminho da wwwroot

    // INJETE IConfiguration e IWebHostEnvironment no construtor
    public InstituicaoController(DbZoologico context, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(context)
    {
        _configuration = configuration;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpPost]
    public async Task<IActionResult> CriarInstituicao([FromForm] Instituicao model, IFormFile Imagem) // <--- MUDANÇA AQUI: [FromForm] e IFormFile
    {
        // 1. Validação dos dados do formulário
        if (model == null)
        {
            return BadRequest(new { success = false, message = "Dados da instituição inválidos." });
        }

        if (string.IsNullOrWhiteSpace(model.Nome))
        {
            return BadRequest(new { success = false, message = "O nome da instituição é obrigatório." });
        }
        if (string.IsNullOrWhiteSpace(model.Endereco))
        {
            return BadRequest(new { success = false, message = "O endereço da instituição é obrigatório." });
        }

        try
        {
            // 2. Lógica para salvar a imagem no servidor
            if (Imagem != null && Imagem.Length > 0)
            {
                // Define a pasta onde as imagens serão salvas dentro de wwwroot
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "instituicoes"); // Ex: wwwroot/uploads/instituicoes
                
                // Cria a pasta se ela não existir
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Gera um nome único para o arquivo para evitar colisões
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Imagem.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Salva o arquivo fisicamente no servidor
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Imagem.CopyToAsync(fileStream);
                }

                // Armazena o caminho relativo (URL) da imagem no seu modelo
                model.ImagemUrl = $"/uploads/instituicoes/{uniqueFileName}"; 
            }
            else
            {
                model.ImagemUrl = null; // Garante que a URL da imagem seja nula se nenhuma imagem for enviada
            }

            // 3. Adiciona a nova instituição (com ou sem ImagemUrl) ao DbContext
            context.Instituicoes.Add(model);
            
            // 4. Salva as mudanças no banco de dados
            await context.SaveChangesAsync();

            // 5. Retorna uma resposta de sucesso
            return Ok(new { success = true, message = "Instituição cadastrada com sucesso!", instituicaoId = model.InstituicaoId });
        }
        catch (Exception ex)
        {
            // Loga a exceção para depuração
            Console.WriteLine($"Erro ao cadastrar instituição com imagem: {ex.Message}");
            return StatusCode(500, new { success = false, message = "Erro interno do servidor ao cadastrar a instituição." });
        }
    }

    // Dentro de InstituicaoController.cs

// ... (seus outros métodos, como CriarInstituicao) ...

[HttpGet]
// Se você não usa [Route] em nenhuma outra action, este pode ser apenas [HttpGet]
// e o frontend chamaria /Instituicao/ObterTodasInstituicoes
public async Task<IActionResult> ObterTodasInstituicoes()
{
    try
    {
        // Busca todas as instituições, ordenadas pelo nome
        var instituicoes = await context.Instituicoes
                                        .OrderBy(i => i.Nome)
                                        .ToListAsync();

        // Retorna a lista de instituições como JSON
        return Ok(instituicoes); 
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao obter instituições: {ex.Message}");
        return StatusCode(500, new { success = false, message = "Erro interno do servidor ao obter as instituições." });
    }
}
}