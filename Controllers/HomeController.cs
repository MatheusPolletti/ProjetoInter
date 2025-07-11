using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using ProjetoInter.Data;
using Microsoft.EntityFrameworkCore;

public class HomeController : BaseController
{
    private readonly IConfiguration configuration;
    public HomeController(IConfiguration _configuration, DbZoologico _context) : base(_context)
    {
        configuration = _configuration;
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

                var funcionario = await context.Funcionarios
                                                .FirstOrDefaultAsync(f => f.AuthUserId == supabaseUid);

                if (funcionario == null)
                {
                    TempData["Erro"] = "Erro: Usuário autenticado, mas não encontrado em nosso registro de funcionários.";
                    return RedirectToAction("LoginCadastro");
                }

                if (funcionario.StatusFuncionarioId == 3 || funcionario.StatusFuncionarioId == 5)
                {
                    TempData["Erro"] = "Erro: Usuário inativo";

                    return RedirectToAction("LoginCadastro");
                }

                HttpContext.Session.SetString("SupabaseAccessToken", accessToken!);
                HttpContext.Session.SetString("SupabaseRefreshToken", refreshToken!);

                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, funcionario.FuncionarioId.ToString()),
                new Claim(ClaimTypes.Name, funcionario.Nome),
                new Claim(ClaimTypes.Email, email),
                new Claim("uid", uidString!),
                new Claim("InstituicaoId", funcionario.InstituicaoId.ToString())
            };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Animais", "Animal");
            }
            else
            {
                TempData["Erro"] = "Usuário ou senha inválidos.";
                return RedirectToAction("LoginCadastro");
            }
        }
        catch (Exception ex)
        {
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