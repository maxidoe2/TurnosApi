using System.ComponentModel.DataAnnotations;

namespace TurnosApi.Models;

public class Turno {
    public int Id { get; set; }

    [Required] public int PacienteId { get; set; }
    [Required] public int MedicoId   { get; set; }

    [Required] public DateTime FechaHora { get; set; }

    [Required, StringLength(20)]
    public string Estado { get; set; } = "pendiente"; // pendiente|confirmado|cancelado

    // Navegación (útil para includes)
    public Paciente? Paciente { get; set; }
    public Medico?   Medico   { get; set; }
}
