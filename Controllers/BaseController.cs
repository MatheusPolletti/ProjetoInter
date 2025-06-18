using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProjetoInter.Data;
using ProjetoInter.Models;

public class BaseController : Controller
{
    protected readonly DbZoologico context;

    public BaseController(DbZoologico _context)
    {
        context = _context;
    }

    public override void OnActionExecuting(ActionExecutingContext _context)
    {
        var uid = User.FindFirst("uid")?.Value;

        if (!string.IsNullOrEmpty(uid))
        {
            var funcionario = context.Funcionarios.FirstOrDefault(f => f.AuthUserId.ToString() == uid);

            if (funcionario != null)
            {
                ViewBag.NomeUsuario = funcionario.Nome;
                ViewBag.FotoUsuario = funcionario.ImagemUrl;
            }
        }

        base.OnActionExecuting(_context);
    }

    protected Funcionario? ObterFuncionarioLogado()
    {
        var uid = User.FindFirst("uid")?.Value;

        if (string.IsNullOrEmpty(uid)) return null;

        var funcionario = context.Funcionarios.FirstOrDefault(f => f.AuthUserId.ToString() == uid);

        return funcionario;
    }
}