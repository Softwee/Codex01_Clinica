namespace ClinicApi.Dtos;

public record MedicoDto(int Id, string PrimerNombre, string? SegundoNombre, string ApellidoPaterno, string ApellidoMaterno,
    string Cedula, string Telefono, string Especialidad, string Email, bool Activo, DateTime FechaCreacion);

public record CreateMedicoDto(string PrimerNombre, string? SegundoNombre, string ApellidoPaterno, string ApellidoMaterno,
    string Cedula, string Telefono, string Especialidad, string Email, bool Activo);

public record UpdateMedicoDto(string PrimerNombre, string? SegundoNombre, string ApellidoPaterno, string ApellidoMaterno,
    string Cedula, string Telefono, string Especialidad, string Email, bool Activo);
