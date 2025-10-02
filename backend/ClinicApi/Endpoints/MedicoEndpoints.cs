using ClinicApi.Data;
using ClinicApi.Dtos;
using ClinicApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicApi.Endpoints;

public static class MedicoEndpoints
{
    public static RouteGroupBuilder MapMedicoEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/medicos").RequireAuthorization();

        group.MapGet("/", async (ClinicContext db) =>
            await db.Medicos
                .Select(m => new MedicoDto(m.Id, m.PrimerNombre, m.SegundoNombre, m.ApellidoPaterno, m.ApellidoMaterno,
                    m.Cedula, m.Telefono, m.Especialidad, m.Email, m.Activo, m.FechaCreacion))
                .ToListAsync());

        group.MapGet("/{id:int}", async Task<Results<Ok<MedicoDto>, NotFound>> (int id, ClinicContext db) =>
        {
            var medico = await db.Medicos.FindAsync(id);
            if (medico is null)
            {
                return TypedResults.NotFound();
            }

            var dto = new MedicoDto(medico.Id, medico.PrimerNombre, medico.SegundoNombre, medico.ApellidoPaterno,
                medico.ApellidoMaterno, medico.Cedula, medico.Telefono, medico.Especialidad, medico.Email, medico.Activo,
                medico.FechaCreacion);
            return TypedResults.Ok(dto);
        });

        group.MapPost("/", async Task<Results<Created<MedicoDto>, BadRequest<string>>> ([FromBody] CreateMedicoDto dto, ClinicContext db) =>
        {
            if (await db.Medicos.AnyAsync(m => m.Cedula == dto.Cedula))
            {
                return TypedResults.BadRequest("La cédula ya está en uso.");
            }

            var medico = new Medico
            {
                PrimerNombre = dto.PrimerNombre,
                SegundoNombre = dto.SegundoNombre,
                ApellidoPaterno = dto.ApellidoPaterno,
                ApellidoMaterno = dto.ApellidoMaterno,
                Cedula = dto.Cedula,
                Telefono = dto.Telefono,
                Especialidad = dto.Especialidad,
                Email = dto.Email,
                Activo = dto.Activo,
                FechaCreacion = DateTime.UtcNow
            };

            db.Medicos.Add(medico);
            await db.SaveChangesAsync();

            var resultDto = new MedicoDto(medico.Id, medico.PrimerNombre, medico.SegundoNombre, medico.ApellidoPaterno,
                medico.ApellidoMaterno, medico.Cedula, medico.Telefono, medico.Especialidad, medico.Email, medico.Activo,
                medico.FechaCreacion);
            return TypedResults.Created($"/api/medicos/{medico.Id}", resultDto);
        });

        group.MapPut("/{id:int}", async Task<Results<NoContent, NotFound>> (int id, [FromBody] UpdateMedicoDto dto, ClinicContext db) =>
        {
            var medico = await db.Medicos.FindAsync(id);
            if (medico is null)
            {
                return TypedResults.NotFound();
            }

            medico.PrimerNombre = dto.PrimerNombre;
            medico.SegundoNombre = dto.SegundoNombre;
            medico.ApellidoPaterno = dto.ApellidoPaterno;
            medico.ApellidoMaterno = dto.ApellidoMaterno;
            medico.Cedula = dto.Cedula;
            medico.Telefono = dto.Telefono;
            medico.Especialidad = dto.Especialidad;
            medico.Email = dto.Email;
            medico.Activo = dto.Activo;

            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        });

        group.MapDelete("/{id:int}", async Task<Results<NoContent, NotFound>> (int id, ClinicContext db) =>
        {
            var medico = await db.Medicos.Include(m => m.Consultas).FirstOrDefaultAsync(m => m.Id == id);
            if (medico is null)
            {
                return TypedResults.NotFound();
            }

            if (medico.Consultas.Any())
            {
                db.Consultas.RemoveRange(medico.Consultas);
            }

            db.Medicos.Remove(medico);
            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        });

        return group;
    }
}
