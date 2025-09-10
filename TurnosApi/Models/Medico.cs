using System.ComponentModel.DataAnnotations;

namespace TurnosApi.Models;

public class Medico {
    public int Id { get; set; }

    [Required, StringLength(30)]
    public string Matricula { get; set; } = "";

    [Required, StringLength(100)]
    public string Nombre { get; set; } = "";

    [Required, StringLength(80)]
    public string Especialidad { get; set; } = "";
}
