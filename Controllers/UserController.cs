using INTRANET_GENERIC.Models.DTO.Request;
using INTRANET_GENERIC.Models.DTO.Response;
using INTRANET_GENERIC.Services;

namespace INTRANET_GENERIC.Controllers;

public static class UserController
{
    /// <summary>
    /// Registra un nuevo usuario
    /// </summary>
    public static async Task<SimpleResponseModel> RegistrarUsuario(UserRequestModel request)
    {
        try
        {
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(request.NombreUsuario))
            {
                return new SimpleResponseModel
                {
                    IsError = true,
                    Message = "El nombre de usuario es requerido",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(request.Contrasena))
            {
                return new SimpleResponseModel
                {
                    IsError = true,
                    Message = "La contraseña es requerida",
                    Data = null
                };
            }

            // Llamar al servicio para registrar usuario
            var nuevoUsuarioId = await UserService.RegistrarUsuarioAsync(request);

            // Retornar respuesta exitosa
            return new SimpleResponseModel
            {
                IsError = false,
                Message = "Usuario registrado exitosamente",
                Data = nuevoUsuarioId
            };
        }
        catch (Exception ex)
        {
            // Manejar errores
            return new SimpleResponseModel
            {
                IsError = true,
                Message = $"Error al registrar usuario: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// Valida las credenciales del usuario para login
    /// </summary>
    public static async Task<SimpleResponseModel> ValidarLogin(UserRequestModel request)
    {
        try
        {
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(request.NombreUsuario))
            {
                return new SimpleResponseModel
                {
                    IsError = true,
                    Message = "El nombre de usuario es requerido",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(request.Contrasena))
            {
                return new SimpleResponseModel
                {
                    IsError = true,
                    Message = "La contraseña es requerida",
                    Data = null
                };
            }

            // Llamar al servicio para validar credenciales
            var credencialesValidas = await UserService.ValidarCredencialesAsync(request);

            if (!credencialesValidas)
            {
                return new SimpleResponseModel
                {
                    IsError = true,
                    Message = "Credenciales inválidas. Verifique su usuario y contraseña",
                    Data = null
                };
            }

            // Retornar respuesta exitosa
            return new SimpleResponseModel
            {
                IsError = false,
                Message = "Login exitoso",
                Data = request.NombreUsuario
            };
        }
        catch (Exception ex)
        {
            // Manejar errores
            return new SimpleResponseModel
            {
                IsError = true,
                Message = $"Error al validar login: {ex.Message}",
                Data = null
            };
        }
    }
}