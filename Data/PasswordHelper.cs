using BCrypt.Net;

namespace INTRANET_GENERIC.Utilities;

public static class PasswordHelper
{
    /// <summary>
    /// Encripta una contraseña usando BCrypt
    /// </summary>
    /// <param name="password">Contraseña en texto plano</param>
    /// <returns>Contraseña hasheada</returns>
    public static string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("La contraseña no puede estar vacía", nameof(password));

        // BCrypt genera automáticamente el salt y usa 11 rounds por defecto
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }

    /// <summary>
    /// Verifica si una contraseña coincide con su hash
    /// </summary>
    /// <param name="password">Contraseña en texto plano</param>
    /// <param name="hashedPassword">Hash almacenado en la BD</param>
    /// <returns>True si coincide, false si no</returns>
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("La contraseña no puede estar vacía", nameof(password));

        if (string.IsNullOrWhiteSpace(hashedPassword))
            throw new ArgumentException("El hash no puede estar vacío", nameof(hashedPassword));

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        catch
        {
            // Si el hash es inválido, retornar false
            return false;
        }
    }

    /// <summary>
    /// Genera una contraseña aleatoria segura
    /// </summary>
    /// <param name="length">Longitud de la contraseña</param>
    /// <returns>Contraseña aleatoria</returns>
    public static string GenerateRandomPassword(int length = 12)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}