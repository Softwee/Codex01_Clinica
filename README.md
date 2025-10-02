# Sistema de gestión para clínica

Este repositorio contiene una solución completa para una clínica que incluye:

- **Backend** desarrollado con ASP.NET Core Minimal APIs y Entity Framework Core (SQLite) para exponer CRUDs de usuarios, médicos, pacientes y consultas con autenticación JWT.
- **Frontend** construido con Vue 3 + Vite y Tailwind CSS que consume el backend y ofrece las pantallas solicitadas (inicio de sesión, dashboard, consultas y administración).

## Backend (ASP.NET Core)

Ubicación: [`backend/ClinicApi`](backend/ClinicApi)

### Características principales

- Base de datos SQLite con modelos para Usuarios, Médicos, Pacientes y Consultas.
- Seed automático de un usuario administrador (`correo: administrador`, `contraseña: 123`).
- Autenticación y autorización mediante JWT.
- Endpoints agrupados por recurso utilizando Minimal APIs.
- Documentación Swagger habilitada en entorno de desarrollo.

### Ejecución

```bash
dotnet restore
dotnet run --project backend/ClinicApi/ClinicApi.csproj
```

La API se expone en `http://localhost:5000` (por defecto de Kestrel) con rutas bajo `/api`.

## Frontend (Vue 3 + Tailwind)

Ubicación: [`frontend`](frontend)

### Características principales

- Manejo de estado con Pinia para la sesión autenticada.
- Consumo del backend mediante Axios, almacenando el token JWT.
- Tailwind CSS para la maquetación de las vistas.
- Vistas solicitadas: Inicio de sesión, Dashboard, Consultas y Administración (usuarios y médicos).

### Ejecución

```bash
cd frontend
npm install
npm run dev
```

El frontend se sirve en `http://localhost:5173` y espera que el backend esté disponible en `http://localhost:5000/api` (configurable mediante la variable `VITE_API_URL`).

## Credenciales por defecto

- **Usuario:** `administrador`
- **Contraseña:** `123`

## Notas

- Al iniciar el backend se crea automáticamente la base de datos SQLite `clinic.db` en la carpeta del proyecto si no existe.
- Los endpoints están protegidos; se debe iniciar sesión para acceder a ellos.
- Las contraseñas se almacenan usando SHA-256.
