namespace ClinicApi.Models;

public class Paciente
{
    public int Id { get; set; }
    public string PrimerNombre { get; set; } = null!;
    public string? SegundoNombre { get; set; }
    public string ApellidoPaterno { get; set; } = null!;
    public string ApellidoMaterno { get; set; } = null!;
    public string Telefono { get; set; } = null!;
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }

    public ICollection<Consulta> Consultas { get; set; } = new List<Consulta>();
}
