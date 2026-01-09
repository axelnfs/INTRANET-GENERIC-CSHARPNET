# Sistema de Intranet Genérico

Sistema base para desarrollo de intranets y portales web utilizando ASP.NET Core 10 con arquitectura en capas sin Entity Framework.

![.NET Version](https://img.shields.io/badge/.NET-10.0-purple)
![C# Version](https://img.shields.io/badge/C%23-14.0-blue)
![License](https://img.shields.io/badge/license-MIT-green)

---

## ?? Tabla de Contenidos

1. [Características Principales](#-características-principales)
2. [Arquitectura del Proyecto](#-arquitectura-del-proyecto)
3. [Tecnologías Utilizadas](#-tecnologías-utilizadas)
4. [Requisitos Previos](#-requisitos-previos)
5. [Instalación](#-instalación)
6. [Configuración](#-configuración)
7. [Estructura de Carpetas](#-estructura-de-carpetas)
8. [Guía de Desarrollo](#-guía-de-desarrollo)
   - [Crear un Endpoint](#crear-un-endpoint)
   - [Crear una Vista](#crear-una-vista)
9. [Seguridad](#-seguridad)
10. [Gestión de Sesiones](#-gestión-de-sesiones)
11. [API Reference](#-api-reference)
12. [Ejemplos de Uso](#-ejemplos-de-uso)
13. [Testing](#-testing)
14. [Troubleshooting](#-troubleshooting)
15. [Contribución](#-contribución)
16. [Licencia](#-licencia)

---

## ?? Características Principales

- ? **Arquitectura en Capas**: Separación clara de responsabilidades
- ? **Sin Entity Framework**: Acceso directo a base de datos con Dapper
- ? **Sin Inyección de Dependencias**: Métodos y clases estáticos
- ? **API REST + MVC**: Soporte para endpoints y vistas web
- ? **Gestión de Sesiones**: Sistema de autenticación con LocalStorage
- ? **Seguridad**: Encriptación BCrypt para contraseñas
- ? **Stored Procedures**: Lógica de base de datos encapsulada
- ? **Responsive**: Bootstrap 5 para diseño adaptable
- ? **Modular**: Fácil de extender y mantener

---

## ??? Arquitectura del Proyecto

### Diagrama de Capas

```
???????????????????????????????????????????????????????????
?                CAPA DE PRESENTACIÓN                     ?
?  ???????????????????????   ????????????????????????   ?
?  ? JavaScript (jQuery) ????? Views (Razor Pages)  ?   ?
?  ???????????????????????   ????????????????????????   ?
???????????????????????????????????????????????????????????
                        ?
                        ? AJAX
???????????????????????????????????????????????????????????
?                    CAPA API REST                        ?
?            Api/UserApiController.cs                     ?
?          [ApiController] con endpoints                  ?
???????????????????????????????????????????????????????????
                        ?
                        ? Llama
???????????????????????????????????????????????????????????
?                CAPA DE CONTROLADORES                    ?
?            Controllers/UserController.cs                ?
?        Validaciones y orquestación                      ?
???????????????????????????????????????????????????????????
                        ?
                        ? Llama
???????????????????????????????????????????????????????????
?                  CAPA DE SERVICIOS                      ?
?              Services/UserService.cs                    ?
?        Lógica de negocio y encriptación                ?
???????????????????????????????????????????????????????????
                        ?
                        ? Llama
???????????????????????????????????????????????????????????
?               CAPA DE ACCESO A DATOS                    ?
?             Data/DatabaseHelper.cs                      ?
?         Ejecución de Stored Procedures                  ?
???????????????????????????????????????????????????????????
                        ?
                        ? Ejecuta
???????????????????????????????????????????????????????????
?                  BASE DE DATOS                          ?
?           SQL Server (Stored Procedures)                ?
???????????????????????????????????????????????????????????
```

### Flujo de Datos

```
Usuario ? JavaScript ? API Controller ? Controller ? Service ? DatabaseHelper ? SQL Server
                                                                                      ?
Usuario ? JSON Response ? API Response ? DTO Response ? Result ? SP Result ??????????
```

---

## ??? Tecnologías Utilizadas

### Backend
- **.NET 10** - Framework principal
- **C# 14.0** - Lenguaje de programación
- **ASP.NET Core MVC** - Framework web
- **Dapper** - Micro ORM para acceso a datos
- **BCrypt.Net-Next** - Encriptación de contraseñas
- **System.Data.SqlClient** - Proveedor de SQL Server

### Frontend
- **Razor Pages** - Motor de vistas
- **jQuery 3.7.1** - Manipulación del DOM y AJAX
- **Bootstrap 5.3.3** - Framework CSS
- **Bootstrap Icons 1.11.3** - Iconografía

### Base de Datos
- **SQL Server** - Sistema de gestión de base de datos
- **Stored Procedures** - Lógica de base de datos

---

## ?? Requisitos Previos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (2019 o superior)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) o [VS Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/) (opcional)

---

## ?? Instalación

### 1. Clonar el repositorio

```bash
git clone https://github.com/tu-usuario/intranet-generic.git
cd intranet-generic
```

### 2. Restaurar paquetes NuGet

```bash
dotnet restore
```

### 3. Instalar paquetes adicionales (si es necesario)

```bash
dotnet add package Dapper
dotnet add package System.Data.SqlClient
dotnet add package BCrypt.Net-Next
```

### 4. Compilar el proyecto

```bash
dotnet build
```

---

## ?? Configuración

### 1. Configurar Connection String

Edita el archivo `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=IntranetDB;Integrated Security=true;TrustServerCertificate=true;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

#### Opciones de Connection String

**Windows Authentication:**
```
Server=localhost;Database=IntranetDB;Integrated Security=true;TrustServerCertificate=true;
```

**SQL Server Authentication:**
```
Server=localhost;Database=IntranetDB;User Id=sa;Password=TuPassword;TrustServerCertificate=true;
```

**Instancia Nombrada (SQLEXPRESS):**
```
Server=localhost\\SQLEXPRESS;Database=IntranetDB;Integrated Security=true;TrustServerCertificate=true;
```

### 2. Crear la Base de Datos

Ejecuta el siguiente script SQL:

```sql
-- Crear base de datos
CREATE DATABASE IntranetDB;
GO

USE IntranetDB;
GO

-- Crear tabla de usuarios
CREATE TABLE Usuarios (
    UsuarioID INT IDENTITY(1,1) PRIMARY KEY,
    NombreUsuario VARCHAR(50) NOT NULL UNIQUE,
    Contrasena VARCHAR(255) NOT NULL,
    Tecnologia VARCHAR(20),
    FechaCreacion DATETIME DEFAULT GETDATE(),
    Activo BIT DEFAULT 1
);
GO

-- Crear índices
CREATE INDEX IX_Usuarios_NombreUsuario ON Usuarios(NombreUsuario);
GO
```

### 3. Crear Stored Procedures

```sql
-- Procedimiento para crear usuario
CREATE PROCEDURE dbo.UsuariosCrear
    @NombreUsuario VARCHAR(50),
    @Contraseña VARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Verificar si el usuario ya existe
    IF EXISTS (SELECT 1 FROM Usuarios WHERE NombreUsuario = @NombreUsuario)
    BEGIN
        RAISERROR('El usuario ya existe', 16, 1);
        RETURN;
    END
    
    INSERT INTO dbo.Usuarios (NombreUsuario, Contrasena, Tecnologia)
    VALUES (@NombreUsuario, @Contraseña, 'CSHARP');
    
    SELECT SCOPE_IDENTITY() AS UsuarioID;
END;
GO

-- Procedimiento para obtener hash de contraseña (login)
CREATE PROCEDURE sp_UsuarioObtenerHashContrasena
    @NombreUsuario VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT Contrasena
    FROM dbo.Usuarios
    WHERE NombreUsuario = @NombreUsuario
        AND Activo = 1;
END;
GO
```

### 4. Ejecutar el proyecto

```bash
dotnet run
```

O con recarga automática:

```bash
dotnet watch run
```

### 5. Acceder a la aplicación

Abre tu navegador en:
- **Home**: https://localhost:5001/
- **Login**: https://localhost:5001/Login
- **Register**: https://localhost:5001/Register

---

## ?? Estructura de Carpetas

```
INTRANET_GENERIC/
?
??? Api/                              # Endpoints API REST
?   ??? UserApiController.cs          # Controlador de API para usuarios
?
??? Controllers/                      # Controladores de lógica de negocio
?   ??? UserController.cs             # Lógica de usuarios (estático)
?   ??? HomeController.cs             # Controlador MVC para Home
?   ??? LoginController.cs            # Controlador MVC para Login
?   ??? RegisterController.cs         # Controlador MVC para Register
?
??? Services/                         # Servicios de negocio
?   ??? UserService.cs                # Servicio de usuarios (llamadas a DB)
?
??? Data/                             # Acceso a base de datos
?   ??? DatabaseHelper.cs             # Wrapper de Dapper (métodos estáticos)
?   ??? PasswordHelper.cs             # Utilidades de encriptación BCrypt
?
??? Models/                           # DTOs y modelos
?   ??? DTO/
?   ?   ??? Request/
?   ?   ?   ??? UserRequestModel.cs   # DTO de entrada
?   ?   ??? Response/
?   ?       ??? SimpleResponseModel.cs # DTO de salida
?   ??? Entities/                     # Entidades de dominio (opcional)
?
??? Views/                            # Vistas Razor
?   ??? Shared/
?   ?   ??? _Layout.cshtml            # Layout principal
?   ??? Home/
?   ?   ??? Index.cshtml              # Página de inicio
?   ??? Login/
?   ?   ??? Index.cshtml              # Página de login
?   ??? Register/
?   ?   ??? Index.cshtml              # Página de registro
?   ??? _ViewStart.cshtml             # Configuración de layout por defecto
?
??? wwwroot/                          # Contenido estático
?   ??? css/
?   ?   ??? site.css                  # Estilos globales
?   ?   ??? login.css                 # Estilos de login/register
?   ??? js/
?   ?   ??? site.js                   # SessionManager y utilidades globales
?   ?   ??? pages/
?   ?       ??? home.js               # JavaScript de Home
?   ?       ??? login.js              # JavaScript de Login
?   ?       ??? register.js           # JavaScript de Register
?   ??? images/                       # Imágenes
?   ??? lib/                          # Librerías locales
?
??? appsettings.json                  # Configuración de aplicación
??? Program.cs                        # Punto de entrada de la aplicación
??? README.md                         # Este archivo
??? INTRANET_GENERIC.csproj          # Archivo de proyecto
```

---

## ?? Guía de Desarrollo

### Crear un Endpoint

#### Paso 1: Crear el Stored Procedure

```sql
-- Database/StoredProcedures/ProductosObtener.sql
CREATE PROCEDURE dbo.ProductosObtener
AS
BEGIN
    SELECT ProductoID, Nombre, Precio, Stock
    FROM Productos
    WHERE Activo = 1;
END;
GO
```

#### Paso 2: Crear los DTOs

**Request Model:**
```csharp
// Models/DTO/Request/ProductoRequestModel.cs
namespace INTRANET_GENERIC.Models.DTO.Request;

public class ProductoRequestModel
{
    public string Nombre { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}
```

**Response Model:**
```csharp
// Models/DTO/Response/ProductoResponseModel.cs
namespace INTRANET_GENERIC.Models.DTO.Response;

public class ProductoResponseModel
{
    public int ProductoID { get; set; }
    public string Nombre { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}
```

#### Paso 3: Crear el Servicio

```csharp
// Services/ProductoService.cs
using INTRANET_GENERIC.Data;
using INTRANET_GENERIC.Models.DTO.Response;

namespace INTRANET_GENERIC.Services;

public static class ProductoService
{
    public static async Task<IEnumerable<ProductoResponseModel>> ObtenerTodosAsync()
    {
        try
        {
            var productos = await DatabaseHelper.ExecuteStoredProcedureAsync<ProductoResponseModel>(
                "ProductosObtener"
            );
            
            return productos;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al obtener productos: {ex.Message}", ex);
        }
    }
}
```

#### Paso 4: Crear el Controlador

```csharp
// Controllers/ProductoController.cs
using INTRANET_GENERIC.Models.DTO.Response;
using INTRANET_GENERIC.Services;

namespace INTRANET_GENERIC.Controllers;

public static class ProductoController
{
    public static async Task<SimpleResponseModel> ObtenerTodos()
    {
        try
        {
            var productos = await ProductoService.ObtenerTodosAsync();
            
            return new SimpleResponseModel
            {
                IsError = false,
                Message = "Productos obtenidos exitosamente",
                Data = productos
            };
        }
        catch (Exception ex)
        {
            return new SimpleResponseModel
            {
                IsError = true,
                Message = $"Error al obtener productos: {ex.Message}",
                Data = null
            };
        }
    }
}
```

#### Paso 5: Crear el API Controller

```csharp
// Api/ProductoApiController.cs
using Microsoft.AspNetCore.Mvc;
using INTRANET_GENERIC.Controllers;
using INTRANET_GENERIC.Models.DTO.Response;

namespace INTRANET_GENERIC.Api;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductoApiController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(SimpleResponseModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var resultado = await ProductoController.ObtenerTodos();
        
        if (resultado.IsError)
            return BadRequest(resultado);
            
        return Ok(resultado);
    }
}
```

#### Paso 6: Consumir desde JavaScript

```javascript
// wwwroot/js/pages/productos.js
async function cargarProductos() {
    try {
        const response = await $.ajax({
            url: '/api/ProductoApi',
            method: 'GET'
        });

        if (response.isError) {
            mostrarMensaje(response.message, 'error');
            return;
        }

        mostrarProductos(response.data);
    } catch (error) {
        console.error('Error:', error);
        mostrarMensaje('Error al cargar productos', 'error');
    }
}

function mostrarProductos(productos) {
    const tbody = $('#tablaProductos tbody');
    tbody.empty();

    productos.forEach(producto => {
        tbody.append(`
            <tr>
                <td>${producto.productoID}</td>
                <td>${producto.nombre}</td>
                <td>$${producto.precio.toFixed(2)}</td>
                <td>${producto.stock}</td>
            </tr>
        `);
    });
}
```

---

### Crear una Vista

#### Paso 1: Crear el Controlador MVC

```csharp
// Controllers/ProductosController.cs
using Microsoft.AspNetCore.Mvc;

namespace INTRANET_GENERIC.Controllers;

public class ProductosController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
```

#### Paso 2: Crear la Vista Razor

```razor
<!-- Views/Productos/Index.cshtml -->
@{
    ViewData["Title"] = "Productos";
}

<div class="container mt-4">
    <h1>
        <i class="bi bi-box"></i> Gestión de Productos
    </h1>

    <div class="card mt-3">
        <div class="card-body">
            <button id="btnNuevo" class="btn btn-primary mb-3">
                <i class="bi bi-plus-circle"></i> Nuevo Producto
            </button>

            <table id="tablaProductos" class="table table-striped">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Nombre</th>
                        <th>Precio</th>
                        <th>Stock</th>
                        <th>Acciones</th>
                    </tr>
                </thead>
                <tbody>
                    <!-- Se llena con JavaScript -->
                </tbody>
            </table>
        </div>
    </div>
</div>

@section Scripts {
    <script src="~/js/pages/productos.js"></script>
}
```

#### Paso 3: Crear el JavaScript

```javascript
// wwwroot/js/pages/productos.js
$(document).ready(function() {
    // Verificar autenticación
    if (!SessionManager.requireAuth()) {
        return;
    }

    cargarProductos();

    $('#btnNuevo').on('click', mostrarModalNuevo);
});

async function cargarProductos() {
    try {
        const response = await $.ajax({
            url: '/api/ProductoApi',
            method: 'GET'
        });

        if (!response.isError) {
            mostrarProductos(response.data);
        }
    } catch (error) {
        console.error('Error:', error);
        mostrarMensaje('Error al cargar productos', 'error');
    }
}
```

---

## ?? Seguridad

### Encriptación de Contraseñas

El sistema utiliza **BCrypt** para encriptar contraseñas de forma segura.

#### Características de BCrypt:
- **Salt automático**: Cada hash es único
- **Work Factor**: Configurable (por defecto 12)
- **One-way**: No se puede desencriptar
- **Resistente a ataques**: Lento intencionalmente

#### Uso del PasswordHelper:

```csharp
// Encriptar contraseña al registrar
string contrasenaEncriptada = PasswordHelper.HashPassword("password123");
// Resultado: $2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/...

// Verificar contraseña al hacer login
bool esValida = PasswordHelper.VerifyPassword("password123", hashAlmacenado);
// Resultado: true o false
```

#### Implementación:

```csharp
// Utilities/PasswordHelper.cs
using BCrypt.Net;

namespace INTRANET_GENERIC.Utilities;

public static class PasswordHelper
{
    /// <summary>
    /// Encripta una contraseña usando BCrypt con work factor 12
    /// </summary>
    public static string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("La contraseña no puede estar vacía");

        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }

    /// <summary>
    /// Verifica si una contraseña coincide con su hash
    /// </summary>
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashedPassword))
            return false;

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        catch
        {
            return false;
        }
    }
}
```

### Buenas Prácticas de Seguridad

? **Nunca guardar contraseñas en texto plano**
```csharp
// ? INCORRECTO
await DatabaseHelper.ExecuteAsync("INSERT INTO Usuarios VALUES (@Pass)", new { Pass = password });

// ? CORRECTO
var hash = PasswordHelper.HashPassword(password);
await DatabaseHelper.ExecuteAsync("INSERT INTO Usuarios VALUES (@Pass)", new { Pass = hash });
```

? **Validar siempre en el servidor**
```csharp
// Validaciones en Controller antes de llamar al Service
if (string.IsNullOrWhiteSpace(request.NombreUsuario))
    return new SimpleResponseModel { IsError = true, Message = "Usuario requerido" };
```

? **Usar HTTPS en producción**
```csharp
// Program.cs
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}
```

---

## ?? Gestión de Sesiones

El sistema implementa un gestor de sesiones en JavaScript usando `localStorage`.

### SessionManager

#### Características:
- ? Almacenamiento en `localStorage`
- ? Expiración automática (2 horas)
- ? Validación en cada carga de página
- ? Protección de rutas
- ? Redirección automática

#### API del SessionManager:

```javascript
// Guardar sesión
SessionManager.setSession('nombreUsuario');

// Verificar si la sesión es válida
if (SessionManager.isSessionValid()) {
    console.log('Sesión activa');
}

// Obtener usuario actual
const usuario = SessionManager.getCurrentUser();

// Requerir autenticación (redirige a login si no hay sesión)
if (!SessionManager.requireAuth()) {
    return;
}

// Redirigir a home si ya hay sesión (para login/register)
if (SessionManager.redirectIfAuthenticated()) {
    return;
}

// Cerrar sesión
SessionManager.clearSession();
```

#### Implementación Completa:

```javascript
// wwwroot/js/site.js
const SessionManager = {
    STORAGE_KEYS: {
        USER: 'app_user',
        LOGIN_TIME: 'app_login_time',
        SESSION_ACTIVE: 'app_session_active'
    },

    SESSION_TIMEOUT: 2 * 60 * 60 * 1000, // 2 horas

    setSession(nombreUsuario) {
        localStorage.setItem(this.STORAGE_KEYS.USER, nombreUsuario);
        localStorage.setItem(this.STORAGE_KEYS.LOGIN_TIME, new Date().toISOString());
        localStorage.setItem(this.STORAGE_KEYS.SESSION_ACTIVE, 'true');
        console.log('Sesión iniciada:', nombreUsuario);
    },

    getSession() {
        return {
            nombreUsuario: localStorage.getItem(this.STORAGE_KEYS.USER),
            loginTime: localStorage.getItem(this.STORAGE_KEYS.LOGIN_TIME),
            isActive: localStorage.getItem(this.STORAGE_KEYS.SESSION_ACTIVE) === 'true'
        };
    },

    isSessionValid() {
        const session = this.getSession();
        if (!session.isActive || !session.nombreUsuario || !session.loginTime) {
            return false;
        }

        const loginTime = new Date(session.loginTime);
        const currentTime = new Date();
        const timeDiff = currentTime - loginTime;

        if (timeDiff > this.SESSION_TIMEOUT) {
            console.log('Sesión expirada');
            this.clearSession();
            return false;
        }

        return true;
    },

    clearSession() {
        localStorage.removeItem(this.STORAGE_KEYS.USER);
        localStorage.removeItem(this.STORAGE_KEYS.LOGIN_TIME);
        localStorage.removeItem(this.STORAGE_KEYS.SESSION_ACTIVE);
        console.log('Sesión cerrada');
    },

    getCurrentUser() {
        if (this.isSessionValid()) {
            return this.getSession().nombreUsuario;
        }
        return null;
    },

    requireAuth() {
        if (!this.isSessionValid()) {
            console.log('Sesión no válida, redirigiendo a login...');
            window.location.href = '/Login';
            return false;
        }
        return true;
    },

    redirectIfAuthenticated() {
        if (this.isSessionValid()) {
            console.log('Usuario ya autenticado, redirigiendo a home...');
            window.location.href = '/';
            return true;
        }
        return false;
    }
};

// Exportar globalmente
window.SessionManager = SessionManager;
```

### Protección de Rutas

#### Protección Global (site.js):

```javascript
$(document).ready(function() {
    const currentPath = window.location.pathname.toLowerCase();
    const publicPages = ['/login', '/register'];
    const isPublicPage = publicPages.some(page => currentPath.includes(page));

    // Redirigir a login si no hay sesión en páginas privadas
    if (!isPublicPage && !SessionManager.isSessionValid()) {
        window.location.href = '/Login';
        return;
    }

    // Redirigir a home si hay sesión en páginas públicas
    if (isPublicPage && SessionManager.isSessionValid()) {
        window.location.href = '/';
        return;
    }
});
```

#### Protección por Página:

```javascript
// wwwroot/js/pages/home.js
document.addEventListener('DOMContentLoaded', () => {
    // Verificar autenticación
    if (!SessionManager.requireAuth()) {
        return; // Redirige automáticamente
    }

    // Código de la página
    const usuario = SessionManager.getCurrentUser();
    console.log('Usuario:', usuario);
});
```

---

## ?? API Reference

### Endpoints Disponibles

#### **UserApi**

##### POST `/api/UserApi/Register`
Registra un nuevo usuario en el sistema.

**Request Body:**
```json
{
  "nombreUsuario": "string",
  "contrasena": "string"
}
```

**Response 200 OK:**
```json
{
  "isError": false,
  "message": "Usuario registrado exitosamente",
  "data": 1
}
```

**Response 400 Bad Request:**
```json
{
  "isError": true,
  "message": "El nombre de usuario ya existe",
  "data": null
}
```

##### POST `/api/UserApi/Login`
Valida las credenciales del usuario.

**Request Body:**
```json
{
  "nombreUsuario": "string",
  "contrasena": "string"
}
```

**Response 200 OK:**
```json
{
  "isError": false,
  "message": "Login exitoso",
  "data": "nombreUsuario"
}
```

**Response 401 Unauthorized:**
```json
{
  "isError": true,
  "message": "Credenciales inválidas. Verifique su usuario y contraseña",
  "data": null
}
```

##### GET `/api/UserApi/Ping`
Verifica que el API esté funcionando.

**Response 200 OK:**
```json
{
  "isError": false,
  "message": "User API está funcionando correctamente",
  "data": "2024-01-15T10:30:00Z"
}
```

---

## ?? Ejemplos de Uso

### Ejemplo 1: Registrar un Usuario con cURL

```bash
curl -X POST https://localhost:5001/api/UserApi/Register \
  -H "Content-Type: application/json" \
  -d '{
    "nombreUsuario": "johndoe",
    "contrasena": "SecurePass123!"
  }'
```

### Ejemplo 2: Login con Fetch API

```javascript
async function login() {
    try {
        const response = await fetch('/api/UserApi/Login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                nombreUsuario: 'johndoe',
                contrasena: 'SecurePass123!'
            })
        });

        const data = await response.json();

        if (!data.isError) {
            SessionManager.setSession(data.data);
            window.location.href = '/';
        } else {
            alert(data.message);
        }
    } catch (error) {
        console.error('Error:', error);
    }
}
```

### Ejemplo 3: Llamar API desde Postman

1. Abrir Postman
2. Crear nueva request POST
3. URL: `https://localhost:5001/api/UserApi/Register`
4. Headers:
   - `Content-Type`: `application/json`
5. Body (raw JSON):
```json
{
  "nombreUsuario": "testuser",
  "contrasena": "password123"
}
```
6. Send

---

## ?? Testing

### Probar Endpoints con Postman

#### Collection de Postman:

```json
{
  "info": {
    "name": "Intranet Generic API",
    "schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
  },
  "item": [
    {
      "name": "User",
      "item": [
        {
          "name": "Register",
          "request": {
            "method": "POST",
            "header": [
              {
                "key": "Content-Type",
                "value": "application/json"
              }
            ],
            "body": {
              "mode": "raw",
              "raw": "{\n  \"nombreUsuario\": \"testuser\",\n  \"contrasena\": \"password123\"\n}"
            },
            "url": {
              "raw": "https://localhost:5001/api/UserApi/Register",
              "protocol": "https",
              "host": ["localhost"],
              "port": "5001",
              "path": ["api", "UserApi", "Register"]
            }
          }
        },
        {
          "name": "Login",
          "request": {
            "method": "POST",
            "header": [
              {
                "key": "Content-Type",
                "value": "application/json"
              }
            ],
            "body": {
              "mode": "raw",
              "raw": "{\n  \"nombreUsuario\": \"testuser\",\n  \"contrasena\": \"password123\"\n}"
            },
            "url": {
              "raw": "https://localhost:5001/api/UserApi/Login",
              "protocol": "https",
              "host": ["localhost"],
              "port": "5001",
              "path": ["api", "UserApi", "Login"]
            }
          }
        }
      ]
    }
  ]
}
```

### Probar Manualmente

1. **Registro de Usuario:**
   - Ir a: https://localhost:5001/Register
   - Completar formulario
   - Clic en "Register"
   - Verificar redirección a Login

2. **Login:**
   - Ir a: https://localhost:5001/Login
   - Ingresar credenciales
   - Clic en "Log in"
   - Verificar redirección a Home

3. **Verificar Sesión:**
   - Abrir DevTools (F12)
   - Ir a Application ? Local Storage
   - Verificar claves: `app_user`, `app_login_time`, `app_session_active`

4. **Protección de Rutas:**
   - Con sesión activa, intentar acceder a `/Login` ? Redirige a Home
   - Cerrar sesión
   - Intentar acceder a `/` ? Redirige a Login

---

## ?? Troubleshooting

### Error: "SessionManager is not defined"

**Causa:** `site.js` no se carga antes de los scripts de página.

**Solución:** En las vistas sin layout, cargar `site.js` antes:

```html
<!-- ?? Orden correcto -->
<script src="~/js/site.js"></script>
<script src="~/js/pages/login.js"></script>
```

### Error: "Cannot connect to SQL Server"

**Causa:** Connection string incorrecto o SQL Server no está ejecutándose.

**Solución:**
1. Verificar que SQL Server esté ejecutándose:
```bash
# Verificar servicios
services.msc
# Buscar: SQL Server (SQLEXPRESS o MSSQLSERVER)
```

2. Verificar connection string en `appsettings.json`:
```json
"DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=IntranetDB;Integrated Security=true;TrustServerCertificate=true;"
```

3. Probar conexión con SSMS (SQL Server Management Studio)

### Error: "Stored procedure not found"

**Causa:** El procedimiento almacenado no existe o tiene un nombre diferente.

**Solución:**
1. Verificar que el SP existe:
```sql
SELECT * FROM sys.procedures WHERE name = 'UsuariosCrear';
```

2. Verificar el nombre en el código:
```csharp
await DatabaseHelper.ExecuteStoredProcedureScalarAsync<int>(
    "UsuariosCrear",  // ? Nombre debe coincidir exactamente
    parameters
);
```

### Error 401: Unauthorized en API

**Causa:** La sesión expiró o no existe.

**Solución:** Verificar que hay sesión activa en localStorage y hacer login nuevamente.

### Error: "BCrypt verification failed"

**Causa:** El hash almacenado no es válido o la contraseña es incorrecta.

**Solución:**
1. Verificar que la contraseña se está encriptando al registrar:
```csharp
var hash = PasswordHelper.HashPassword(password);
```

2. Verificar que el campo en la BD sea VARCHAR(255) o mayor

---

## ?? Contribución

### Cómo Contribuir

1. **Fork** el repositorio
2. Crear una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abrir un **Pull Request**

### Convenciones de Código

#### C# (PascalCase):
```csharp
// Clases
public class UserService { }

// Métodos
public static async Task<int> RegistrarUsuarioAsync() { }

// Propiedades
public string NombreUsuario { get; set; }
```

#### JavaScript (camelCase):
```javascript
// Funciones
async function iniciarSesion() { }

// Variables
const nombreUsuario = 'test';

// Constantes
const SESSION_TIMEOUT = 7200000;
```

#### SQL (snake_case para procedimientos):
```sql
CREATE PROCEDURE sp_UsuariosCrear
```

### Estructura de Commits

```
feat: agregar funcionalidad de recuperación de contraseña
fix: corregir error en validación de sesión
docs: actualizar README con ejemplos de uso
refactor: mejorar estructura de DatabaseHelper
style: aplicar formato consistente en Controllers
test: agregar pruebas unitarias para UserService
```

---

## ?? Licencia

Este proyecto está bajo la Licencia MIT - ver el archivo [LICENSE](LICENSE) para más detalles.

```
MIT License

Copyright (c) 2024 [Tu Nombre]

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

---

## ?? Contacto y Soporte

- **Documentación:** [README.md](README.md)
- **Issues:** [GitHub Issues](https://github.com/tu-usuario/intranet-generic/issues)
- **Discussions:** [GitHub Discussions](https://github.com/tu-usuario/intranet-generic/discussions)

### Recursos Útiles

- [Documentación ASP.NET Core](https://docs.microsoft.com/aspnet/core)
- [Documentación Dapper](https://github.com/DapperLib/Dapper)
- [Documentación BCrypt.Net](https://github.com/BcryptNet/bcrypt.net)
- [Bootstrap 5 Docs](https://getbootstrap.com/docs/5.3)
- [jQuery API](https://api.jquery.com/)

---

## ?? Roadmap

### Version 1.0 (Actual)
- ? Sistema de autenticación
- ? Gestión de sesiones
- ? Encriptación de contraseñas
- ? API REST básica
- ? Vistas con Razor Pages

### Version 1.1 (Próximo Release)
- [ ] Recuperación de contraseña
- [ ] Perfiles de usuario
- [ ] Sistema de roles y permisos
- [ ] Auditoría de acciones
- [ ] API de notificaciones

### Version 2.0 (Futuro)
- [ ] Autenticación con OAuth2
- [ ] Two-Factor Authentication (2FA)
- [ ] Dashboard analytics
- [ ] Sistema de archivos
- [ ] Chat en tiempo real (SignalR)

---

## ?? Diagrama de Base de Datos

```sql
???????????????????????????????????
?          Usuarios               ?
???????????????????????????????????
? UsuarioID       INT PK          ?
? NombreUsuario   VARCHAR(50) UQ  ?
? Contrasena      VARCHAR(255)    ?
? Tecnologia      VARCHAR(20)     ?
? FechaCreacion   DATETIME        ?
? Activo          BIT             ?
???????????????????????????????????
```

---

## ?? Agradecimientos

- **Microsoft** por ASP.NET Core
- **Marc Gravell** por Dapper
- **BCrypt.Net** contributors
- **Bootstrap Team** por el framework CSS
- **jQuery Foundation** por jQuery
- Comunidad de desarrolladores por su feedback

---

## ?? Changelog

### [1.0.0] - 2024-01-15

#### Added
- Sistema base de autenticación con login/register
- Gestión de sesiones con localStorage
- Encriptación BCrypt para contraseñas
- API REST con endpoints básicos
- Vistas Razor con Bootstrap 5
- DatabaseHelper para acceso a datos con Dapper
- SessionManager para control de sesiones
- Documentación completa en README.md

#### Security
- Implementación de BCrypt para hash de contraseñas
- Validación de sesiones con expiración automática
- Protección de rutas públicas y privadas

---

<div align="center">

**? Si este proyecto te fue útil, considera darle una estrella en GitHub ?**

Hecho con ?? y ? por desarrolladores para desarrolladores

[Reportar Bug](https://github.com/tu-usuario/intranet-generic/issues) · [Solicitar Feature](https://github.com/tu-usuario/intranet-generic/issues) · [Contribuir](CONTRIBUTING.md)

</div>
