using System.ComponentModel.DataAnnotations;

namespace Eventos.Web.Models;

public class Inscripcion
{
    public int Id { get; set; }

    [Required]
    public int EventoId { get; set; }

    public Evento? Evento { get; set; }

    [Required]
    public int AsistenteId { get; set; }

    public Asistente? Asistente { get; set; }

    public DateTime FechaInscripcion { get; set; } = DateTime.Now;

    [Required]
    [StringLength(20)]
    public string Estado { get; set; } = "Confirmada"; // Confirmada o Cancelada
}
