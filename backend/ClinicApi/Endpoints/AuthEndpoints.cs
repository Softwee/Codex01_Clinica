using ClinicApi.Data;
using ClinicApi.Dtos;
using ClinicApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicApi.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/api/auth/login", async Task<Results<Ok<LoginResponse>, UnauthorizedHttpResult>> ([FromBody] LoginRequest request,
            ClinicContext db, TokenService tokenService) =>
        {
            var user = await db.Usuarios.FirstOrDefaultAsync(u => u.Correo == request.Correo);
            if (user is null)
            {
                return TypedResults.Unauthorized();
            }

            if (!PasswordService.VerifyPassword(user.Password, request.Password))
            {
                return TypedResults.Unauthorized();
            }

            if (!user.Activo)
            {
                return TypedResults.Unauthorized();
            }

            var token = tokenService.GenerateToken(user);
            var response = new LoginResponse(token, user.NombreCompleto, user.Activo, user.MedicoId);
            return TypedResults.Ok(response);
        });
    }
}
