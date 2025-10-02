using System.Text;
using ClinicApi.Configuration;
using ClinicApi.Data;
using ClinicApi.Endpoints;
using ClinicApi.Models;
using ClinicApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddDbContext<ClinicContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=clinic.db"));

builder.Services.AddSingleton<TokenService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>() ?? new JwtSettings();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ClinicContext>();
    await db.Database.EnsureCreatedAsync();

    if (!await db.Usuarios.AnyAsync(u => u.Correo == "administrador"))
    {
        var admin = new Usuario
        {
            Correo = "administrador",
            Password = PasswordService.HashPassword("123"),
            NombreCompleto = "Administrador",
            MedicoId = null,
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };

        db.Usuarios.Add(admin);
        await db.SaveChangesAsync();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapUserEndpoints();
app.MapMedicoEndpoints();
app.MapPacienteEndpoints();
app.MapConsultaEndpoints();

app.Run();
