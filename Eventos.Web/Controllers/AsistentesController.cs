using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Eventos.Web.Controllers;

public class AsistentesController : Controller
{
    private readonly AsistenteRepositorio _asistenteRepo;

    public AsistentesController(AsistenteRepositorio asistenteRepo)
    {
        _asistenteRepo = asistenteRepo;
    }

    // GET: Asistentes
    public async Task<IActionResult> Index()
    {
        var asistentes = await _asistenteRepo.ObtenerTodosActivosAsync();
        return View(asistentes);
    }

    // GET: Asistentes/Create
    public IActionResult Create()
    {
        return View(new Asistente());
    }

    // POST: Asistentes/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Asistente asistente)
    {
        if (await _asistenteRepo.ExisteDniAsync(asistente.Dni))
        {
            ModelState.AddModelError("Dni", "El DNI ingresado ya se encuentra registrado.");
        }

        if (await _asistenteRepo.ExisteCorreoAsync(asistente.Correo))
        {
            ModelState.AddModelError("Correo", "El correo ingresado ya se encuentra registrado.");
        }

        if (ModelState.IsValid)
        {
            await _asistenteRepo.CrearAsync(asistente);
            TempData["Mensaje"] = $"El asistente '{asistente.Nombres}' fue registrado con éxito.";
            return RedirectToAction(nameof(Index));
        }

        return View(asistente);
    }
}

