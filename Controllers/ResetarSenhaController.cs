using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class ResetarSenhaController : Controller
{
    private readonly IConfiguration _configuration;

    public ResetarSenhaController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Index(string access_token)
    {
        if (string.IsNullOrEmpty(access_token))
        {
            ViewBag.Mensagem = "Token inválido ou ausente.";
            return View();
        }

        try
        {
            var decodedUrl = System.Net.WebUtility.UrlDecode(access_token);

            var uri = new Uri(decodedUrl);

            var queryParams = QueryHelpers.ParseQuery(uri.Query);

            if (!queryParams.TryGetValue("token", out var token) || string.IsNullOrEmpty(token))
            {
                ViewBag.Mensagem = "Token JWT não encontrado na URL.";
                return View();
            }

            ViewBag.AccessToken = token.ToString();
        }
        catch (Exception)
        {
            ViewBag.Mensagem = "Formato do token inválido.";
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(string accessToken, string novaSenha, string confirmaSenha)
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            ViewBag.Mensagem = "Token ausente.";
            return View();
        }

        if (string.IsNullOrEmpty(novaSenha) || string.IsNullOrEmpty(confirmaSenha))
        {
            ViewBag.Mensagem = "Preencha todos os campos.";
            ViewBag.AccessToken = accessToken;
            return View();
        }

        if (novaSenha != confirmaSenha)
        {
            ViewBag.Mensagem = "As senhas não conferem.";
            ViewBag.AccessToken = accessToken;
            return View();
        }

        var supabaseUrl = _configuration["Supabase:Url"];
        var supabaseApiKey = _configuration["Supabase:ApiKey"];

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("apikey", supabaseApiKey);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var body = JsonSerializer.Serialize(new
        {
            password = novaSenha
        });

        var content = new StringContent(body, Encoding.UTF8, "application/json");

        var response = await httpClient.PutAsync($"{supabaseUrl}/auth/v1/user", content);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            ViewBag.Mensagem = "Senha alterada com sucesso!";
        }
        else
        {
            ViewBag.Mensagem = $"Erro ao alterar senha: {response.StatusCode} - {responseContent}";
        }

        ViewBag.AccessToken = accessToken;
        return View();
    }
}