using System.ComponentModel.DataAnnotations;

namespace TurnosApi.Models;

public class Paciente {
    public int Id { get; set; }

    [Required, StringLength(20)]
    public string DNI { get; set; } = "";

    [Required, StringLength(100)]
    public string Nombre { get; set; } = "";

    [Required, EmailAddress, StringLength(120)]
    public string Email { get; set; } = "";
}
