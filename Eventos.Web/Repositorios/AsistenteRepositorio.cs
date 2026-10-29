using Eventos.Web.Data;
using Eventos.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Repositorios;

public class AsistenteRepositorio
{
    private readonly EventosContext _context;

    public AsistenteRepositorio(EventosContext context)
    {
        _context = context;
    }

    public async Task<List<Asistente>> ObtenerTodosActivosAsync()
    {
        return await _context.Asistentes
            .AsNoTracking()
            .Where(a => a.Activo)
            .OrderBy(a => a.Nombres)
            .ToListAsync();
    }

    public async Task<Asistente?> ObtenerPorIdAsync(int id)
    {
        return await _context.Asistentes
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id && a.Activo);
    }

    public async Task<bool> ExisteDniAsync(string dni, int? excluirId = null)
    {
        return await _context.Asistentes
            .AnyAsync(a => a.Dni == dni && (excluirId == null || a.Id != excluirId));
    }

    public async Task<bool> ExisteCorreoAsync(string correo, int? excluirId = null)
    {
        return await _context.Asistentes
            .AnyAsync(a => a.Correo.ToLower() == correo.ToLower() && (excluirId == null || a.Id != excluirId));
    }

    public async Task CrearAsync(Asistente asistente)
    {
        asistente.Activo = true;
        _context.Asistentes.Add(asistente);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarLogicoAsync(int id)
    {
        var asistente = await _context.Asistentes.FirstOrDefaultAsync(a => a.Id == id);
        if (asistente != null)
        {
            asistente.Activo = false;
            await _context.SaveChangesAsync();
        }
    }
}

