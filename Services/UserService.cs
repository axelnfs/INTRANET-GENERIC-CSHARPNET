using INTRANET_GENERIC.Data;
using INTRANET_GENERIC.Models.DTO.Request;
using INTRANET_GENERIC.Utilities;

namespace INTRANET_GENERIC.Services;

public static class UserService
{
    /// <summary>
    /// Registra un nuevo usuario con contraseña encriptada
    /// </summary>
    public static async Task<int> RegistrarUsuarioAsync(UserRequestModel request)
    {
        try
        {
            // Validar que la contraseña no esté vacía
            if (string.IsNullOrWhiteSpace(request.Contrasena))
                throw new ArgumentException("La contraseña es requerida");

            // Encriptar la contraseña antes de guardarla
            string contrasenaEncriptada = PasswordHelper.HashPassword(request.Contrasena);

            // Llamar al stored procedure UsuariosCrear con la contraseña encriptada
            var nuevoId = await DatabaseHelper.ExecuteStoredProcedureScalarAsync<int>(
                "sp_UsuariosCrear",
                new
                {
                    NombreUsuario = request.NombreUsuario,
                    Contraseña = contrasenaEncriptada
                }
            );

            return nuevoId;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al registrar usuario: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Valida las credenciales del usuario (para futuro login)
    /// </summary>
    public static async Task<bool> ValidarCredencialesAsync(UserRequestModel request)
    {
        try
        {
            // Obtener el hash almacenado en la BD
            var hashAlmacenado = await DatabaseHelper.ExecuteStoredProcedureScalarAsync<string>(
                "sp_UsuarioObtenerHashContrasena",
                new
                {
                    NombreUsuario = request.NombreUsuario,
                }
            );

            if (string.IsNullOrEmpty(hashAlmacenado))
                return false;

            // Verificar la contraseña contra el hash
            return PasswordHelper.VerifyPassword(request.Contrasena, hashAlmacenado);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al validar credenciales: {ex.Message}", ex);
        }
    }
}