using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Text.Json;
using ProjetoInter.Models;


[Authorize]
public class ColaboradorController : BaseController
{
    private readonly IConfiguration configuration;

    public ColaboradorController(DbZoologico _context, IConfiguration _configuration) : base(_context)
    {
        configuration = _configuration;
    }

    [HttpPost]
    public IActionResult AcessarTarefa()
    {
        return RedirectToAction("Tarefas", "Tarefa");
    }

    public async Task<IActionResult> Colaboradores(string? busca)
    {
        var _funcionarioLogado = ObterFuncionarioLogado();
        ViewBag.FuncionarioLogado = _funcionarioLogado;

        var _ListaInstituicoesCadastradas = await InstituicoesCadastradas();
        ViewBag.ListaInstituicoesCadastradas = _ListaInstituicoesCadastradas;

        var query = context.Funcionarios
            .Include(f => f.StatusFuncionario)
            .Where(f => f.InstituicaoId == _funcionarioLogado.InstituicaoId);

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

    [HttpPost]
    public async Task<IActionResult> NovoUsuario(string _email, string _password, string _nome, string _cpf, string _cargo, int _instituicao, IFormFile _imagem, string _telefone, bool _adm = false)
    {
        if (_password.Length < 6 || !_password.Any(char.IsDigit) || _password.Length > 20)
        {
            ViewBag.Erro = "A senha deve ter entre  6 e 20 caracteres e conter um número.";

            return await CarregarViewColaboradoresComMensagem();
        }

        var supabaseUrl = configuration["Supabase:Url"];
        var supabaseApiKey = configuration["Supabase:ApiKey"];

        using (var client = new HttpClient())
        {
            var url = $"{supabaseUrl}/auth/v1/signup";

            client.DefaultRequestHeaders.Add("apikey", supabaseApiKey);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {supabaseApiKey}");

            var body = new
            {
                email = _email,
                password = _password
            };

            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var supabaseResponse = JsonDocument.Parse(responseContent);
                var uuid = supabaseResponse.RootElement
                    .GetProperty("user")
                    .GetProperty("id")
                    .GetString();

                if (Guid.TryParse(uuid, out Guid supabaseGuid))
                {
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
                        StatusFuncionarioId = 1,
                        ImagemUrl = null
                    };

                    context.Funcionarios.Add(novoFuncionario);

                    await context.SaveChangesAsync();

                    ViewBag.Sucesso = "Funcionário criado com sucesso!";
                }
                else
                {
                    ViewBag.Erro = "UUID inválido retornado pelo Supabase.";
                }
            }
            else
            {
                ViewBag.Erro = $"Erro no Supabase: {responseContent}";
            }
        }

        return await CarregarViewColaboradoresComMensagem();
    }

    private async Task<IActionResult> CarregarViewColaboradoresComMensagem()
    {
        var _funcionarioLogado = ObterFuncionarioLogado();
        ViewBag.FuncionarioLogado = _funcionarioLogado;

        var _ListaInstituicoesCadastradas = await InstituicoesCadastradas();
        ViewBag.ListaInstituicoesCadastradas = _ListaInstituicoesCadastradas;

        var funcionarios = await context.Funcionarios
            .Include(f => f.StatusFuncionario)
            .Where(f => f.InstituicaoId == _funcionarioLogado.InstituicaoId)
            .ToListAsync();

        return View("Colaboradores", funcionarios);
    }

    [HttpGet]
    public async Task<IActionResult> DetalhesColaborador(int id)
    {
        var funcionario = await context.Funcionarios
            .Include(f => f.StatusFuncionario)
            .FirstOrDefaultAsync(f => f.FuncionarioId == id);

        if (funcionario == null)
        {
            return NotFound();
        }

        return View(funcionario);
    }
}