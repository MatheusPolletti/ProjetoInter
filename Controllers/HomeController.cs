using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using ProjetoInter.Data;
using Microsoft.EntityFrameworkCore;

public class HomeController : Controller
{
    private readonly IConfiguration configuration;
    private readonly DbZoologico _context;

    public HomeController(IConfiguration _configuration, DbZoologico context)
    {
        configuration = _configuration;
        _context = context;
    }

    [HttpGet]
    public IActionResult LoginCadastro()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AcessarSistema(string email, string senha)
    {
        try
        {
            var supabaseUrl = configuration["Supabase:Url"];
            var supabaseApiKey = configuration["Supabase:ApiKey"];

            var payload = new
            {
                email = email,
                password = senha
            };

            var content = new StringContent
            (
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            using var client = new HttpClient();

            client.DefaultRequestHeaders.Add("apikey", supabaseApiKey);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.PostAsync
            (
                $"{supabaseUrl}/auth/v1/token?grant_type=password",
                content
            );

            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var json = JsonDocument.Parse(responseBody);
                var accessToken = json.RootElement.GetProperty("access_token").GetString();
                var refreshToken = json.RootElement.GetProperty("refresh_token").GetString();
                var uidString = json.RootElement.GetProperty("user").GetProperty("id").GetString();

                if (string.IsNullOrEmpty(uidString) || !Guid.TryParse(uidString, out Guid supabaseUid))
                {
                    TempData["Erro"] = "Erro: UID do usuário inválido recebido do Supabase.";
                    return RedirectToAction("LoginCadastro");
                }

                var funcionario = await _context.Funcionarios
                                                .FirstOrDefaultAsync(f => f.AuthUserId == supabaseUid);

                if (funcionario == null)
                {
                    TempData["Erro"] = "Erro: Usuário autenticado, mas não encontrado em nosso registro de funcionários.";
                    return RedirectToAction("LoginCadastro");
                }

                // Armazena tokens na sessão com segurança
                HttpContext.Session.SetString("SupabaseAccessToken", accessToken!);
                HttpContext.Session.SetString("SupabaseRefreshToken", refreshToken!);

                // Cria o cookie de autenticação com os claims do usuário
                var claims = new List<Claim>
                {
                    // --- MUDANÇAS AQUI: ---
                    new Claim(ClaimTypes.NameIdentifier, funcionario.FuncionarioId.ToString()), // ID do funcionário
                    new Claim(ClaimTypes.Name, funcionario.Nome), // Nome do funcionário para exibição
                    // --- FIM DAS MUDANÇAS ---

                    new Claim(ClaimTypes.Email, email), // Email do funcionário
                    new Claim("uid", uidString!), // UID do Supabase, se precisar para outras operações
                    new Claim("InstituicaoId", funcionario.InstituicaoId.ToString()) // ID da instituição
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Animais", "Animal");
            }
            else
            {
                // Oculta detalhes do erro do Supabase para o usuário final, mostra mensagem genérica.
                TempData["Erro"] = "Usuário ou senha inválidos.";
                return RedirectToAction("LoginCadastro");
            }
        }
        catch (Exception ex)
        {
            // Logar o erro completo para depuração, mas mostrar mensagem genérica para o usuário.
            Console.WriteLine($"Erro ao tentar acessar o sistema: {ex.Message}");
            TempData["Erro"] = "Erro ao tentar acessar o sistema. Por favor, tente novamente.";
            return RedirectToAction("LoginCadastro");
        }
    }
    [HttpPost]
    public async Task<IActionResult> Deslogar()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        HttpContext.Session.Clear();

        return RedirectToAction("LoginCadastro", "Home");
    }

    public IActionResult SemSucessoAcessarConta()
    {
        return View("~/Views/ResetarSenha/SemSucessoAcessarConta.cshtml");
    }
}