using ClinicApi.Data;
using ClinicApi.Dtos;
using ClinicApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicApi.Endpoints;

public static class ConsultaEndpoints
{
    public static RouteGroupBuilder MapConsultaEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/consultas").RequireAuthorization();

        group.MapGet("/", async (ClinicContext db) =>
            await db.Consultas
                .Include(c => c.Medico)
                .Include(c => c.Paciente)
                .Select(c => new ConsultaDto(
                    c.Id,
                    c.MedicoId,
                    c.PacienteId,
                    c.Sintomas,
                    c.Recomendaciones,
                    c.Diagnostico,
                    c.Medico != null ? $"{c.Medico.PrimerNombre} {c.Medico.ApellidoPaterno}" : string.Empty,
                    c.Paciente != null ? $"{c.Paciente.PrimerNombre} {c.Paciente.ApellidoPaterno}" : string.Empty))
                .ToListAsync());

        group.MapGet("/{id:int}", async Task<Results<Ok<ConsultaDto>, NotFound>> (int id, ClinicContext db) =>
        {
            var consulta = await db.Consultas
                .Include(c => c.Medico)
                .Include(c => c.Paciente)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (consulta is null)
            {
                return TypedResults.NotFound();
            }

            var dto = new ConsultaDto(
                consulta.Id,
                consulta.MedicoId,
                consulta.PacienteId,
                consulta.Sintomas,
                consulta.Recomendaciones,
                consulta.Diagnostico,
                consulta.Medico != null ? $"{consulta.Medico.PrimerNombre} {consulta.Medico.ApellidoPaterno}" : string.Empty,
                consulta.Paciente != null ? $"{consulta.Paciente.PrimerNombre} {consulta.Paciente.ApellidoPaterno}" : string.Empty);

            return TypedResults.Ok(dto);
        });

        group.MapPost("/", async Task<Results<Created<ConsultaDto>, BadRequest<string>>> ([FromBody] CreateConsultaDto dto, ClinicContext db) =>
        {
            var medicoExists = await db.Medicos.AnyAsync(m => m.Id == dto.MedicoId);
            var pacienteExists = await db.Pacientes.AnyAsync(p => p.Id == dto.PacienteId);

            if (!medicoExists || !pacienteExists)
            {
                return TypedResults.BadRequest("El médico o el paciente especificado no existe.");
            }

            var consulta = new Consulta
            {
                MedicoId = dto.MedicoId,
                PacienteId = dto.PacienteId,
                Sintomas = dto.Sintomas,
                Recomendaciones = dto.Recomendaciones,
                Diagnostico = dto.Diagnostico
            };

            db.Consultas.Add(consulta);
            await db.SaveChangesAsync();

            var resultDto = await db.Consultas
                .Include(c => c.Medico)
                .Include(c => c.Paciente)
                .Where(c => c.Id == consulta.Id)
                .Select(c => new ConsultaDto(
                    c.Id,
                    c.MedicoId,
                    c.PacienteId,
                    c.Sintomas,
                    c.Recomendaciones,
                    c.Diagnostico,
                    c.Medico != null ? $"{c.Medico.PrimerNombre} {c.Medico.ApellidoPaterno}" : string.Empty,
                    c.Paciente != null ? $"{c.Paciente.PrimerNombre} {c.Paciente.ApellidoPaterno}" : string.Empty))
                .FirstAsync();

            return TypedResults.Created($"/api/consultas/{consulta.Id}", resultDto);
        });

        group.MapPut("/{id:int}", async Task<Results<NoContent, NotFound, BadRequest<string>>> (int id, [FromBody] UpdateConsultaDto dto, ClinicContext db) =>
        {
            var consulta = await db.Consultas.FindAsync(id);
            if (consulta is null)
            {
                return TypedResults.NotFound();
            }

            var medicoExists = await db.Medicos.AnyAsync(m => m.Id == dto.MedicoId);
            var pacienteExists = await db.Pacientes.AnyAsync(p => p.Id == dto.PacienteId);

            if (!medicoExists || !pacienteExists)
            {
                return TypedResults.BadRequest("El médico o el paciente especificado no existe.");
            }

            consulta.MedicoId = dto.MedicoId;
            consulta.PacienteId = dto.PacienteId;
            consulta.Sintomas = dto.Sintomas;
            consulta.Recomendaciones = dto.Recomendaciones;
            consulta.Diagnostico = dto.Diagnostico;

            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        });

        group.MapDelete("/{id:int}", async Task<Results<NoContent, NotFound>> (int id, ClinicContext db) =>
        {
            var consulta = await db.Consultas.FindAsync(id);
            if (consulta is null)
            {
                return TypedResults.NotFound();
            }

            db.Consultas.Remove(consulta);
            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        });

        return group;
    }
}
