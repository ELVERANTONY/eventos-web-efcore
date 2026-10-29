using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eventos.Web.Models;

public class Evento
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(100, ErrorMessage = "El título no puede superar los 100 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha es obligatoria.")]
    public DateTime Fecha { get; set; }

    [Required(ErrorMessage = "El lugar es obligatorio.")]
    [StringLength(100, ErrorMessage = "El lugar no puede superar los 100 caracteres.")]
    public string Lugar { get; set; } = string.Empty;

    [Range(1, 5000, ErrorMessage = "El aforo debe estar entre 1 y 5000.")]
    public int Aforo { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    [Range(0, 99999999.99, ErrorMessage = "El precio debe ser un valor no negativo.")]
    public decimal Precio { get; set; }

    [Required(ErrorMessage = "La modalidad es obligatoria.")]
    [StringLength(20, ErrorMessage = "La modalidad no puede superar los 20 caracteres.")]
    public string Modalidad { get; set; } = "Presencial"; // Presencial o Virtual

    [Required(ErrorMessage = "La categoría es obligatoria.")]
    public int CategoriaId { get; set; }

    public Categoria? Categoria { get; set; }

    public bool Activo { get; set; } = true;

    public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
}
