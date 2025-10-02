namespace ClinicApi.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Correo { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string NombreCompleto { get; set; } = null!;
    public int? MedicoId { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }

    public Medico? Medico { get; set; }
}
