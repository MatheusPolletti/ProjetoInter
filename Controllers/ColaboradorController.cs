using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Text.Json;
using ProjetoInter.Models;
using System.Text.RegularExpressions;

[Authorize]
public class ColaboradorController : BaseController
{
    private readonly IConfiguration configuration;

    public ColaboradorController(DbZoologico _context, IConfiguration _configuration) : base(_context)
    {
        configuration = _configuration;
    }

    private async Task<List<StatusFuncionario>> CarregarStatusFuncionario()
    {
        return await context.StatusFuncionarios.ToListAsync();
    }

    private async Task<List<Instituicao>> InstituicoesCadastradas()
    {
        return await context.Instituicoes.ToListAsync();
    }

    public async Task<IActionResult> Colaboradores(string? busca)
    {
        var _funcionarioLogado = ObterFuncionarioLogado();
        ViewBag.FuncionarioLogado = _funcionarioLogado;

        ViewBag.ListaInstituicoesCadastradas = await InstituicoesCadastradas();
        ViewBag.StatusLista = await CarregarStatusFuncionario();

        var query = context.Funcionarios
            .Include(f => f.StatusFuncionario)
            .Where(f => f.InstituicaoId == _funcionarioLogado.InstituicaoId)
            .Where(f => f.StatusFuncionarioId == 1 || f.StatusFuncionarioId == 2);

        if (!string.IsNullOrEmpty(busca))
        {
            busca = busca.ToLower();
            query = query.Where(f => f.Nome.ToLower().Contains(busca)
                || f.Cpf.ToLower().Contains(busca)
                || f.Cargo.ToLower().Contains(busca));
        }

        var funcionarios = await query.ToListAsync();

        return View(funcionarios);
    }

    public async Task<IActionResult> CarregarViewColaboradoresComMensagem()
    {
        var _funcionarioLogado = ObterFuncionarioLogado();
        ViewBag.FuncionarioLogado = _funcionarioLogado;

        ViewBag.ListaInstituicoesCadastradas = await InstituicoesCadastradas();
        ViewBag.StatusLista = await CarregarStatusFuncionario();

        var funcionarios = await context.Funcionarios
            .Include(f => f.StatusFuncionario)
            .Where(f => f.InstituicaoId == _funcionarioLogado.InstituicaoId)
            .Where(f => f.StatusFuncionarioId == 1 || f.StatusFuncionarioId == 2)
            .ToListAsync();

        return View("Colaboradores", funcionarios);
    }

    [HttpPost]
    public async Task<IActionResult> NovoUsuario(string _email, string _password, string _nome, string _cpf, string _cargo, int _instituicao, IFormFile _imagem, string _telefone, bool _adm = false)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_email) || string.IsNullOrWhiteSpace(_password))
            {
                ViewBag.Erro = "Email e senha são obrigatórios.";
                ViewBag.MostrarModal = true;
                return await CarregarViewColaboradoresComMensagem();
            }

            if (_password.Length < 6 || !_password.Any(char.IsDigit) || _password.Length > 20)
            {
                ViewBag.Erro = "A senha deve ter entre 6 e 20 caracteres e conter um número.";
                ViewBag.MostrarModal = true;
                return await CarregarViewColaboradoresComMensagem();
            }

            if (_nome.Length < 2 || _nome.Any(char.IsDigit) || _nome.Any(ch => !char.IsLetter(ch) && !char.IsWhiteSpace(ch)))
            {
                ViewBag.Erro = "O nome deve ter pelo menos 2 caracteres e não pode conter números ou caracteres especiais.";
                ViewBag.MostrarModal = true;
                return await CarregarViewColaboradoresComMensagem();
            }

            if (!Regex.IsMatch(_cpf, @"^\d{3}\.\d{3}\.\d{3}-\d{2}$"))
            {
                ViewBag.Erro = "O CPF deve estar no formato 000.000.000-00.";
                ViewBag.MostrarModal = true;
                return await CarregarViewColaboradoresComMensagem();
            }

            if (_cargo.Length < 3 || _cargo.Length > 50 || _cargo.Any(c => !char.IsLetter(c) && !char.IsWhiteSpace(c)))
            {
                ViewBag.Erro = "O cargo deve ter entre 3 e 50 letras, sem caracteres especiais.";
                ViewBag.MostrarModal = true;
                return await CarregarViewColaboradoresComMensagem();
            }

            if (!Regex.IsMatch(_telefone, @"^\(\d{2}\) \d{4}-\d{4}$"))
            {
                ViewBag.Erro = "O telefone deve estar no formato (00) 0000-0000.";
                ViewBag.MostrarModal = true;
                return await CarregarViewColaboradoresComMensagem();
            }

            var supabaseUrl = configuration["Supabase:Url"];
            var supabaseApiKey = configuration["Supabase:ApiKey"];

            using (var client = new HttpClient())
            {
                var url = $"{supabaseUrl}/auth/v1/signup";
                client.DefaultRequestHeaders.Add("apikey", supabaseApiKey);
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {supabaseApiKey}");

                var body = new { email = _email, password = _password };
                var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Erro = $"Erro no Supabase: {responseContent}";
                    ViewBag.MostrarModal = true;
                    return await CarregarViewColaboradoresComMensagem();
                }

                var supabaseResponse = JsonDocument.Parse(responseContent);
                var uuid = supabaseResponse.RootElement.GetProperty("user").GetProperty("id").GetString();

                if (!Guid.TryParse(uuid, out Guid supabaseGuid))
                {
                    ViewBag.Erro = "UUID inválido retornado pelo Supabase.";
                    ViewBag.MostrarModal = true;
                    return await CarregarViewColaboradoresComMensagem();
                }

                var funcionarioLogado = ObterFuncionarioLogado();
                var novoFuncionario = new Funcionario
                {
                    AuthUserId = supabaseGuid,
                    Nome = _nome,
                    Cpf = _cpf,
                    Cargo = _cargo,
                    Telefone = _telefone,
                    IsAdmin = _adm,
                    InstituicaoId = _instituicao,
                    StatusFuncionarioId = 1
                };

                if (_imagem != null && _imagem.Length > 0)
                {
                    var imagemNome = $"{Guid.NewGuid()}{Path.GetExtension(_imagem.FileName)}";
                    var caminhoImagem = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "Funcionarios", imagemNome);

                    using (var stream = new FileStream(caminhoImagem, FileMode.Create))
                    {
                        await _imagem.CopyToAsync(stream);
                    }

                    novoFuncionario.ImagemUrl = $"img/Funcionarios/{imagemNome}";
                }

                context.Funcionarios.Add(novoFuncionario);
                await context.SaveChangesAsync();

                ViewBag.Sucesso = "Funcionário criado com sucesso!";
                ViewBag.MostrarModal = false;

                return await CarregarViewColaboradoresComMensagem();
            }
        }
        catch (Exception ex)
        {
            ViewBag.Erro = "Erro inesperado: " + ex.Message;
            ViewBag.MostrarModal = true;
            return await CarregarViewColaboradoresComMensagem();
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditarUsuario(int _funcionarioId, string _nome, string _cpf, string _cargo, string _telefone, int _statusFuncionario, IFormFile _imagem, bool _adm = false)
    {
        try
        {
            if (_nome.Length < 2 || _nome.Any(char.IsDigit) || _nome.Any(ch => !char.IsLetter(ch) && !char.IsWhiteSpace(ch)))
            {
                ViewBag.Erro = "O nome deve ter pelo menos 2 caracteres e não pode conter números ou caracteres especiais.";

                return await CarregarViewColaboradoresComMensagem();
            }

            if (!Regex.IsMatch(_cpf, @"^\d{3}\.\d{3}\.\d{3}-\d{2}$"))
            {
                ViewBag.Erro = "O CPF deve estar no formato 000.000.000-00.";

                return await CarregarViewColaboradoresComMensagem();
            }

            if (_cargo.Length < 3 || _cargo.Length > 50 || _cargo.Any(c => !char.IsLetter(c) && !char.IsWhiteSpace(c)))
            {
                ViewBag.Erro = "O cargo deve ter entre 3 e 50 letras, sem caracteres especiais.";

                return await CarregarViewColaboradoresComMensagem();
            }

            if (!Regex.IsMatch(_telefone, @"^\(\d{2}\) \d{4}-\d{4}$"))
            {
                ViewBag.Erro = "O telefone deve estar no formato (00) 0000-0000.";

                return await CarregarViewColaboradoresComMensagem();
            }

            var funcionario = await context.Funcionarios.FindAsync(_funcionarioId);

            if (funcionario == null)
            {
                ViewBag.Erro = "Funcionário não encontrado.";

                return await CarregarViewColaboradoresComMensagem();
            }

            funcionario.Nome = _nome;
            funcionario.Cpf = _cpf;
            funcionario.Cargo = _cargo;
            funcionario.Telefone = _telefone;
            funcionario.IsAdmin = _adm;
            funcionario.StatusFuncionarioId = _statusFuncionario;

            if (_imagem != null && _imagem.Length > 0)
            {
                if (!string.IsNullOrEmpty(funcionario.ImagemUrl) && funcionario.ImagemUrl != "img/Funcionarios/Padrao.png")
                {
                    var caminhoImagemAntiga = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", funcionario.ImagemUrl);
                    if (System.IO.File.Exists(caminhoImagemAntiga))
                    {
                        System.IO.File.Delete(caminhoImagemAntiga);
                    }
                }

                var imagemNome = $"{Guid.NewGuid()}{Path.GetExtension(_imagem.FileName)}";
                var caminhoImagem = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "Funcionarios", imagemNome);

                using (var stream = new FileStream(caminhoImagem, FileMode.Create))
                {
                    await _imagem.CopyToAsync(stream);
                }

                funcionario.ImagemUrl = $"img/Funcionarios/{imagemNome}";
            }

            context.Funcionarios.Update(funcionario);
            await context.SaveChangesAsync();

            ViewBag.Sucesso = "Funcionário atualizado com sucesso!";

            return await CarregarViewColaboradoresComMensagem();
        }
        catch (Exception ex)
        {
            ViewBag.Erro = "Erro inesperado: " + ex.Message;

            return await CarregarViewColaboradoresComMensagem();
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> ExcluirUsuario(int _funcionarioId)
    {
        try
        {
            var funcionario = await context.Funcionarios.FindAsync(_funcionarioId);

            if (funcionario == null)
            {
                ViewBag.Erro = "Funcionário não encontrado.";

                return await CarregarViewColaboradoresComMensagem();
            }

            funcionario.StatusFuncionarioId = 5;

            context.Funcionarios.Update(funcionario);
            await context.SaveChangesAsync();

            ViewBag.Sucesso = "Funcionário excluído com sucesso!";

            return await CarregarViewColaboradoresComMensagem();
        }
        catch (Exception ex)
        {
            ViewBag.Erro = "Erro ao excluir: " + ex.Message;
            
            return await CarregarViewColaboradoresComMensagem();
        }
    }
}