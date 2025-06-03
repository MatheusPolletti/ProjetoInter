using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Configuration;

public class HomeController : Controller
{
    private readonly DbZoologico context;
    private readonly IConfiguration configuration;

    public HomeController(DbZoologico _context, IConfiguration _configuration)
    {
        context = _context;
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

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("apikey", supabaseApiKey);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.PostAsync(
                $"{supabaseUrl}/auth/v1/token?grant_type=password",
                content
            );

            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var json = JsonDocument.Parse(responseBody);
                var accessToken = json.RootElement.GetProperty("access_token").GetString();
                var refreshToken = json.RootElement.GetProperty("refresh_token").GetString();

                // Armazena tokens na sessão com segurança
                HttpContext.Session.SetString("SupabaseAccessToken", accessToken!);
                HttpContext.Session.SetString("SupabaseRefreshToken", refreshToken!);

                // Cria o cookie de autenticação com os claims do usuário
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, email),
                    new Claim(ClaimTypes.Email, email)
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // Redireciona para a página principal após login
                return RedirectToAction("Animais", "Animal");
            }
            else
            {
                TempData["Erro"] = "Usuário ou senha inválidos.";
                return RedirectToAction("LoginCadastro");
            }
        }
        catch (Exception)
        {
            TempData["Erro"] = "Erro ao tentar acessar o sistema.";
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
        return View();
    }
}