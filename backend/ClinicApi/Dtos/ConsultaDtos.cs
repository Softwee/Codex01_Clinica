namespace ClinicApi.Dtos;

public record ConsultaDto(int Id, int MedicoId, int PacienteId, string Sintomas, string Recomendaciones, string Diagnostico,
    string MedicoNombreCompleto, string PacienteNombreCompleto);

public record CreateConsultaDto(int MedicoId, int PacienteId, string Sintomas, string Recomendaciones, string Diagnostico);

public record UpdateConsultaDto(int MedicoId, int PacienteId, string Sintomas, string Recomendaciones, string Diagnostico);
