using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class TransferenciaController : BaseController
{
    public TransferenciaController(DbZoologico context) : base(context) {}

    public IActionResult Transferencias()
    {
        return View();
    }
}