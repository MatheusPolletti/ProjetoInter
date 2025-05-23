using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;

public class HomeController : Controller
{
    private readonly DbZoologico _context;

    public HomeController(DbZoologico context)
    {
        _context = context;
    }

    public async Task<IActionResult> LoginCadastro()
    {
        return View();
    }

    [HttpPost]
    public IActionResult AcessarSistema()
    {
        return RedirectToAction("Animais", "Animal");
    }

    public IActionResult SemSucessoAcessarConta()
    {
        return View();
    }
}