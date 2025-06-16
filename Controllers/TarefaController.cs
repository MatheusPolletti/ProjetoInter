using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;
using Microsoft.AspNetCore.Authorization;

namespace ProjetoInter.Controllers
{
    [Authorize]
    public class TarefaController : BaseController
    {
        public TarefaController(DbZoologico _context) : base(_context) {}

        [HttpPost]
        public IActionResult AcessarTarefa()
        {
            var funcionario = ObterFuncionarioLogado();

            if (funcionario == null)
            {
                TempData["Erro"] = "Funcionário não encontrado.";
                return RedirectToAction("LoginCadastro", "Home");
            }

            return RedirectToAction("Tarefas");
        }

        public IActionResult Tarefas()
        {
            var funcionario = ObterFuncionarioLogado();

            if (funcionario == null)
            {
                TempData["Erro"] = "Funcionário não encontrado.";
                return RedirectToAction("LoginCadastro", "Home");
            }

            return View(funcionario);
        }
    }
}