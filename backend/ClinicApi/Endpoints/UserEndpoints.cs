using ClinicApi.Data;
using ClinicApi.Dtos;
using ClinicApi.Models;
using ClinicApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicApi.Endpoints;

public static class UserEndpoints
{
    public static RouteGroupBuilder MapUserEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/usuarios").RequireAuthorization();

        group.MapGet("/", async (ClinicContext db) =>
            await db.Usuarios
                .Select(u => new UserDto(u.Id, u.Correo, u.NombreCompleto, u.MedicoId, u.Activo, u.FechaCreacion))
                .ToListAsync());

        group.MapGet("/{id:int}", async Task<Results<Ok<UserDto>, NotFound>> (int id, ClinicContext db) =>
        {
            var user = await db.Usuarios.FindAsync(id);
            if (user is null)
            {
                return TypedResults.NotFound();
            }

            var dto = new UserDto(user.Id, user.Correo, user.NombreCompleto, user.MedicoId, user.Activo, user.FechaCreacion);
            return TypedResults.Ok(dto);
        });

        group.MapPost("/", async Task<Results<Created<UserDto>, BadRequest<string>>> ([FromBody] CreateUserDto dto, ClinicContext db) =>
        {
            if (await db.Usuarios.AnyAsync(u => u.Correo == dto.Correo))
            {
                return TypedResults.BadRequest("El correo ya está en uso.");
            }

            var user = new Usuario
            {
                Correo = dto.Correo,
                Password = PasswordService.HashPassword(dto.Password),
                NombreCompleto = dto.NombreCompleto,
                MedicoId = dto.MedicoId,
                Activo = dto.Activo,
                FechaCreacion = DateTime.UtcNow
            };

            db.Usuarios.Add(user);
            await db.SaveChangesAsync();

            var resultDto = new UserDto(user.Id, user.Correo, user.NombreCompleto, user.MedicoId, user.Activo, user.FechaCreacion);
            return TypedResults.Created($"/api/usuarios/{user.Id}", resultDto);
        });

        group.MapPut("/{id:int}", async Task<Results<NoContent, NotFound>> (int id, [FromBody] UpdateUserDto dto, ClinicContext db) =>
        {
            var user = await db.Usuarios.FindAsync(id);
            if (user is null)
            {
                return TypedResults.NotFound();
            }

            if (!string.IsNullOrEmpty(dto.Password))
            {
                user.Password = PasswordService.HashPassword(dto.Password);
            }

            user.NombreCompleto = dto.NombreCompleto;
            user.MedicoId = dto.MedicoId;
            user.Activo = dto.Activo;

            await db.SaveChangesAsync();

            return TypedResults.NoContent();
        });

        group.MapDelete("/{id:int}", async Task<Results<NoContent, NotFound>> (int id, ClinicContext db) =>
        {
            var user = await db.Usuarios.FindAsync(id);
            if (user is null)
            {
                return TypedResults.NotFound();
            }

            db.Usuarios.Remove(user);
            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        });

        return group;
    }
}
