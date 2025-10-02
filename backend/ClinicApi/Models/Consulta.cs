namespace ClinicApi.Models;

public class Consulta
{
    public int Id { get; set; }
    public int MedicoId { get; set; }
    public int PacienteId { get; set; }
    public string Sintomas { get; set; } = null!;
    public string Recomendaciones { get; set; } = null!;
    public string Diagnostico { get; set; } = null!;

    public Medico? Medico { get; set; }
    public Paciente? Paciente { get; set; }
}
