using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

public class SemSucessoAcessarContaController : Controller
{
    private readonly IConfiguration configuration;

    public SemSucessoAcessarContaController(IConfiguration _configuration)
    {
        configuration = _configuration;
    }

    [HttpPost]
    public async Task<IActionResult> EnviarEmailRedefinicao(string email)
    {
        using var httpClient = new HttpClient();

        var url = $"{configuration["Supabase:Url"]}/auth/v1/recover";
        var supabaseApiKey = configuration["Supabase:ApiKey"];

        var body = JsonSerializer.Serialize(new { email });
        var content = new StringContent(body, Encoding.UTF8, "application/json");

        // Adiciona o header "apikey" que o Supabase exige
        httpClient.DefaultRequestHeaders.Add("apikey", supabaseApiKey);

        var response = await httpClient.PostAsync(url, content);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            ViewBag.Mensagem = "Um link foi enviado para seu e-mail!";
        }
        else
        {
            ViewBag.Mensagem = $"Erro ao enviar e-mail: {response.StatusCode} - {responseContent}";
        }

        return View("~/Views/ResetarSenha/SemSucessoAcessarConta.cshtml");
    }
}