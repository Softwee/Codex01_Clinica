namespace ClinicApi.Dtos;

public record PacienteDto(int Id, string PrimerNombre, string? SegundoNombre, string ApellidoPaterno, string ApellidoMaterno,
    string Telefono, bool Activo, DateTime FechaCreacion);

public record CreatePacienteDto(string PrimerNombre, string? SegundoNombre, string ApellidoPaterno, string ApellidoMaterno,
    string Telefono, bool Activo);

public record UpdatePacienteDto(string PrimerNombre, string? SegundoNombre, string ApellidoPaterno, string ApellidoMaterno,
    string Telefono, bool Activo);
