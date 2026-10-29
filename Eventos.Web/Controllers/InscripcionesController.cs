using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Eventos.Web.Controllers;

public class InscripcionesController : Controller
{
    private readonly InscripcionRepositorio _inscripcionRepo;

    public InscripcionesController(InscripcionRepositorio inscripcionRepo)
    {
        _inscripcionRepo = inscripcionRepo;
    }

    // GET: Inscripciones
    public async Task<IActionResult> Index()
    {
        var inscripciones = await _inscripcionRepo.ObtenerTodasAsync();
        return View(inscripciones);
    }

    // GET: Inscripciones/Create
    public async Task<IActionResult> Create()
    {
        await CargarListasDesplegablesAsync();
        return View(new Inscripcion { FechaInscripcion = DateTime.Now });
    }

    // POST: Inscripciones/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Inscripcion inscripcion)
    {
        var evento = await _inscripcionRepo.ObtenerEventoParaValidarAsync(inscripcion.EventoId);

        // 1. Validar que el evento exista y no haya pasado
        if (evento == null)
        {
            ModelState.AddModelError("EventoId", "El evento seleccionado no existe o está inactivo.");
        }
        else if (evento.Fecha < DateTime.Now)
        {
            ModelState.AddModelError("EventoId", "No se puede realizar inscripciones a un evento cuya fecha ya ha pasado.");
        }

        // 2. Validar que el asistente no esté ya inscrito (Confirmada) en ese evento
        if (await _inscripcionRepo.YaInscritoAsync(inscripcion.EventoId, inscripcion.AsistenteId))
        {
            ModelState.AddModelError("AsistenteId", "El asistente ya cuenta con una inscripción confirmada para este evento.");
        }

        // 3. Validar que el aforo no esté completo (inscripciones Confirmada menores que el Aforo)
        if (evento != null)
        {
            int confirmadas = await _inscripcionRepo.ContarInscripcionesConfirmadasAsync(inscripcion.EventoId);
            if (confirmadas >= evento.Aforo)
            {
                ModelState.AddModelError("EventoId", "El evento ha completado su aforo máximo permitido.");
            }
        }

        if (ModelState.IsValid)
        {
            await _inscripcionRepo.CrearAsync(inscripcion);
            TempData["Mensaje"] = "Inscripción registrada con éxito en estado Confirmada.";
            return RedirectToAction(nameof(Index));
        }

        await CargarListasDesplegablesAsync(inscripcion.EventoId, inscripcion.AsistenteId);
        return View(inscripcion);
    }

    // POST: Inscripciones/Cancelar/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id)
    {
        await _inscripcionRepo.CancelarInscripcionAsync(id);
        TempData["Mensaje"] = "La inscripción ha sido cancelada exitosamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListasDesplegablesAsync(int? eventoId = null, int? asistenteId = null)
    {
        var eventos = await _inscripcionRepo.ObtenerEventosActivosDisponiblesAsync();
        var asistentes = await _inscripcionRepo.ObtenerAsistentesActivosAsync();

        ViewBag.EventoId = new SelectList(
            eventos.Select(e => new { e.Id, Info = $"{e.Titulo} ({e.Fecha:dd/MM/yyyy HH:mm} - {e.Modalidad})" }),
            "Id", "Info", eventoId);

        ViewBag.AsistenteId = new SelectList(
            asistentes.Select(a => new { a.Id, Info = $"{a.Nombres} (DNI: {a.Dni})" }),
            "Id", "Info", asistenteId);
    }
}
