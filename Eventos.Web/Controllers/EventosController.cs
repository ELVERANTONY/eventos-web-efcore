using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Eventos.Web.Controllers;

public class EventosController : Controller
{
    private readonly EventoRepositorio _eventoRepo;

    public EventosController(EventoRepositorio eventoRepo)
    {
        _eventoRepo = eventoRepo;
    }

    // GET: Eventos
    public async Task<IActionResult> Index(int? categoriaId)
    {
        var eventos = await _eventoRepo.ObtenerTodosActivosAsync(categoriaId);
        var categorias = await _eventoRepo.ObtenerCategoriasActivasAsync();

        ViewBag.Categorias = new SelectList(categorias, "Id", "Nombre", categoriaId);
        ViewBag.CategoriaSeleccionada = categoriaId;

        return View(eventos);
    }

    // GET: Eventos/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var evento = await _eventoRepo.ObtenerPorIdAsync(id);
        if (evento == null)
        {
            return NotFound();
        }
        return View(evento);
    }

    // GET: Eventos/Create
    public async Task<IActionResult> Create()
    {
        await CargarCategoriasYModalidadesAsync();
        return View(new Evento { Fecha = DateTime.Today.AddDays(7).AddHours(10) });
    }

    // POST: Eventos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Evento evento)
    {
        if (ModelState.IsValid)
        {
            await _eventoRepo.CrearAsync(evento);
            TempData["Mensaje"] = $"El evento '{evento.Titulo}' fue creado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        await CargarCategoriasYModalidadesAsync(evento.CategoriaId, evento.Modalidad);
        return View(evento);
    }

    // GET: Eventos/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var evento = await _eventoRepo.ObtenerParaModificarAsync(id);
        if (evento == null)
        {
            return NotFound();
        }

        await CargarCategoriasYModalidadesAsync(evento.CategoriaId, evento.Modalidad);
        return View(evento);
    }

    // POST: Eventos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Evento evento)
    {
        if (id != evento.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            await _eventoRepo.ActualizarAsync(evento);
            TempData["Mensaje"] = $"El evento '{evento.Titulo}' fue actualizado con éxito.";
            return RedirectToAction(nameof(Index));
        }

        await CargarCategoriasYModalidadesAsync(evento.CategoriaId, evento.Modalidad);
        return View(evento);
    }

    // GET: Eventos/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var evento = await _eventoRepo.ObtenerPorIdAsync(id);
        if (evento == null)
        {
            return NotFound();
        }
        return View(evento);
    }

    // POST: Eventos/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _eventoRepo.EliminarLogicoAsync(id);
        TempData["Mensaje"] = "El evento ha sido eliminado lógicamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarCategoriasYModalidadesAsync(int? categoriaSeleccionada = null, string? modalidadSeleccionada = null)
    {
        var categorias = await _eventoRepo.ObtenerCategoriasActivasAsync();
        ViewBag.CategoriaId = new SelectList(categorias, "Id", "Nombre", categoriaSeleccionada);

        var modalidades = new List<string> { "Presencial", "Virtual" };
        ViewBag.Modalidades = new SelectList(modalidades, modalidadSeleccionada ?? "Presencial");
    }
}
