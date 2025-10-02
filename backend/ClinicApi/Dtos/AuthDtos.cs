namespace ClinicApi.Dtos;

public record LoginRequest(string Correo, string Password);

public record LoginResponse(string Token, string NombreCompleto, bool Activo, int? MedicoId);
