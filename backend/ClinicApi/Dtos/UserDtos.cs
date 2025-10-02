namespace ClinicApi.Dtos;

public record UserDto(int Id, string Correo, string NombreCompleto, int? MedicoId, bool Activo, DateTime FechaCreacion);

public record CreateUserDto(string Correo, string Password, string NombreCompleto, int? MedicoId, bool Activo);

public record UpdateUserDto(string? Password, string NombreCompleto, int? MedicoId, bool Activo);
