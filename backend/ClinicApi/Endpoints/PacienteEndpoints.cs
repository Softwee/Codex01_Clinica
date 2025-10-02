using ClinicApi.Data;
using ClinicApi.Dtos;
using ClinicApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicApi.Endpoints;

public static class PacienteEndpoints
{
    public static RouteGroupBuilder MapPacienteEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/pacientes").RequireAuthorization();

        group.MapGet("/", async (ClinicContext db) =>
            await db.Pacientes
                .Select(p => new PacienteDto(p.Id, p.PrimerNombre, p.SegundoNombre, p.ApellidoPaterno, p.ApellidoMaterno,
                    p.Telefono, p.Activo, p.FechaCreacion))
                .ToListAsync());

        group.MapGet("/{id:int}", async Task<Results<Ok<PacienteDto>, NotFound>> (int id, ClinicContext db) =>
        {
            var paciente = await db.Pacientes.FindAsync(id);
            if (paciente is null)
            {
                return TypedResults.NotFound();
            }

            var dto = new PacienteDto(paciente.Id, paciente.PrimerNombre, paciente.SegundoNombre, paciente.ApellidoPaterno,
                paciente.ApellidoMaterno, paciente.Telefono, paciente.Activo, paciente.FechaCreacion);
            return TypedResults.Ok(dto);
        });

        group.MapPost("/", async Task<Created<PacienteDto>> ([FromBody] CreatePacienteDto dto, ClinicContext db) =>
        {
            var paciente = new Paciente
            {
                PrimerNombre = dto.PrimerNombre,
                SegundoNombre = dto.SegundoNombre,
                ApellidoPaterno = dto.ApellidoPaterno,
                ApellidoMaterno = dto.ApellidoMaterno,
                Telefono = dto.Telefono,
                Activo = dto.Activo,
                FechaCreacion = DateTime.UtcNow
            };

            db.Pacientes.Add(paciente);
            await db.SaveChangesAsync();

            var resultDto = new PacienteDto(paciente.Id, paciente.PrimerNombre, paciente.SegundoNombre, paciente.ApellidoPaterno,
                paciente.ApellidoMaterno, paciente.Telefono, paciente.Activo, paciente.FechaCreacion);
            return TypedResults.Created($"/api/pacientes/{paciente.Id}", resultDto);
        });

        group.MapPut("/{id:int}", async Task<Results<NoContent, NotFound>> (int id, [FromBody] UpdatePacienteDto dto, ClinicContext db) =>
        {
            var paciente = await db.Pacientes.FindAsync(id);
            if (paciente is null)
            {
                return TypedResults.NotFound();
            }

            paciente.PrimerNombre = dto.PrimerNombre;
            paciente.SegundoNombre = dto.SegundoNombre;
            paciente.ApellidoPaterno = dto.ApellidoPaterno;
            paciente.ApellidoMaterno = dto.ApellidoMaterno;
            paciente.Telefono = dto.Telefono;
            paciente.Activo = dto.Activo;

            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        });

        group.MapDelete("/{id:int}", async Task<Results<NoContent, NotFound>> (int id, ClinicContext db) =>
        {
            var paciente = await db.Pacientes.Include(p => p.Consultas).FirstOrDefaultAsync(p => p.Id == id);
            if (paciente is null)
            {
                return TypedResults.NotFound();
            }

            if (paciente.Consultas.Any())
            {
                db.Consultas.RemoveRange(paciente.Consultas);
            }

            db.Pacientes.Remove(paciente);
            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        });

        return group;
    }
}
