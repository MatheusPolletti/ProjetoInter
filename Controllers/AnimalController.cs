using Microsoft.AspNetCore.Mvc;
using ProjetoInter.Data;

public class AnimalController : Controller
{
    private readonly DbZoologico _context;

    public AnimalController(DbZoologico context)
    {
        _context = context;
    }

     public IActionResult Index()
    {
        return View();
    }
}