# Documentación de pantallas

Este documento describe el funcionamiento y los flujos principales de las pantallas del frontend relacionadas con la administración de datos maestros y el registro de consultas médicas.

## Administración

La vista de administración agrupa la gestión de **usuarios**, **médicos** y **pacientes**. Cada bloque repite un patrón consistente de formulario + tabla para facilitar la edición de registros existentes.

### Barra superior
- Título "Administración" con un texto de contexto.
- Botón "Volver al dashboard" que redirige a `/dashboard` utilizando `<router-link>`.

### Gestión de usuarios
- Formulario para crear o actualizar usuarios con los campos:
  - **Correo** (`userForm.correo`), bloqueado durante la edición para evitar cambios de identificador.
  - **Contraseña** (`userForm.password`), opcional al editar (permite dejar la actual).
  - **Nombre completo** (`userForm.nombreCompleto`).
  - **Médico asociado** (`userForm.medicoId`) con selector que lista médicos activos.
  - **Activo** (`userForm.activo`) mediante checkbox.
- Las acciones del formulario delegan en `submitUser` para crear (`POST /usuarios`) o actualizar (`PUT /usuarios/:id`). Maneja errores con `userError`.
- Botón contextual "Cancelar" cuando se edita (`editingUserId`).
- Tabla con columnas correo, nombre, médico asociado, estado y acciones.
  - Las etiquetas de estado utilizan estilos condicionales para "Sí" / "No".
  - Acciones incluyen editar (`startEditUser`) y eliminar (`deleteUser` con confirmación y `DELETE /usuarios/:id`).
  - Cuando no existen registros se muestra el mensaje "No hay usuarios registrados.".

### Gestión de médicos
- Formulario con datos personales y profesionales del médico (`medicoForm`). Campos obligatorios:
  - Nombres y apellidos.
  - Cédula profesional.
  - Teléfono y especialidad.
  - Correo electrónico y estado activo.
- `submitMedico` ejecuta `POST /medicos` o `PUT /medicos/:id`, refrescando además la lista de usuarios para reflejar cambios en asignaciones.
- Tabla de médicos con columnas nombre, cédula, especialidad, estado y acciones.
  - Permite editar y eliminar (confirmación y `DELETE /medicos/:id`).
  - Mensaje vacío: "No hay médicos registrados.".

### Gestión de pacientes
- Formulario básico (`pacienteForm`) con nombres, apellidos, teléfono y estado.
- `submitPaciente` publica o actualiza (`POST`/`PUT` en `/pacientes`) y recarga el listado.
- Tabla de pacientes con nombre completo calculado (`getPacienteNombre`), teléfono, estado y acciones.
  - Acciones de editar y eliminar (`DELETE /pacientes/:id`) con confirmación.
  - Mensaje vacío: "No hay pacientes registrados.".

### Lógica común
- `onMounted` asegura autenticación (`ensureAuth`) y carga inicial paralela de usuarios, médicos y pacientes.
- Formularios usan `reactive`/`ref` para el estado y funciones `reset*` para limpiar campos tras operar.

## Consultas médicas

La vista de consultas permite registrar una nueva atención y revisar el historial reciente.

### Estructura general
- Encabezado con título, subtítulo y navegación de regreso al dashboard.
- Distribución en dos columnas (`lg:grid-cols-[320px_1fr]`): formulario a la izquierda e historial a la derecha.

### Registro de nueva consulta
- Formulario controlado por `form` (`reactive`) con los campos:
  - **Médico** (`form.medicoId`): selector que carga médicos activos desde `GET /medicos` (filtrados por `activo`).
  - **Paciente** (`form.pacienteId`): selector con pacientes activos desde `GET /pacientes`.
  - **Síntomas**, **Recomendaciones** y **Diagnóstico** como áreas de texto requeridas.
- Validaciones básicas mediante atributos `required` y deshabilitado del botón mientras `loading` es `true`.
- `handleSubmit` transforma IDs a numéricos, hace `POST /consultas` y recarga el historial.
- Errores capturados en `error` y mostrados en rojo bajo el formulario.

### Historial de consultas
- Listado vertical (`v-for`) de consultas con tarjeta individual:
  - Encabezado muestra nombre completo del médico y una insignia con el paciente.
  - Cuerpo detalla síntomas, recomendaciones y diagnóstico.
- Botón "Actualizar" refresca datos (`fetchConsultas`).
- Mensaje por defecto "No hay consultas registradas." cuando la lista está vacía.

### Ciclo de vida y seguridad
- `onMounted` comprueba autenticación y realiza llamadas iniciales en paralelo a médicos, pacientes y consultas.
- La vista reutiliza `useAuthStore` para confirmar `isAuthenticated` y redirigir a `/login` si la sesión expiró.

## Consideraciones compartidas
- Ambas vistas dependen del cliente Axios centralizado en `src/services/api.js`, que adjunta el token JWT almacenado en Pinia.
- Todas las acciones de eliminación solicitan confirmación del usuario para evitar borrados accidentales.
- Las etiquetas de estado utilizan Tailwind CSS para transmitir visualmente si un registro está activo.
- Las vistas están diseñadas para pantallas amplias, pero la disposición de formularios y tablas se adapta mediante utilidades responsive (`grid`, `flex`, `gap`).
