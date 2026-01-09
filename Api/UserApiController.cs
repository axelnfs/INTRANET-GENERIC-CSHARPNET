using Microsoft.AspNetCore.Mvc;
using INTRANET_GENERIC.Controllers;
using INTRANET_GENERIC.Models.DTO.Request;
using INTRANET_GENERIC.Models.DTO.Response;

namespace INTRANET_GENERIC.Api;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UserApiController : ControllerBase
{
    /// <summary>
    /// Registra un nuevo usuario
    /// POST: api/UserApi/Register
    /// </summary>
    [HttpPost("Register")]
    [ProducesResponseType(typeof(SimpleResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(SimpleResponseModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] UserRequestModel request)
    {
        try
        {
            // Validar que el request no sea nulo
            if (request == null)
            {
                return BadRequest(new SimpleResponseModel
                {
                    IsError = true,
                    Message = "Los datos del usuario son requeridos",
                    Data = null
                });
            }

            // Llamar al controlador
            var resultado = await UserController.RegistrarUsuario(request);

            // Retornar respuesta según el resultado
            if (resultado.IsError)
            {
                return BadRequest(resultado);
            }

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new SimpleResponseModel
            {
                IsError = true,
                Message = $"Error interno del servidor: {ex.Message}",
                Data = null
            });
        }
    }

    /// <summary>
    /// Valida las credenciales del usuario y realiza login
    /// POST: api/UserApi/Login
    /// </summary>
    [HttpPost("Login")]
    [ProducesResponseType(typeof(SimpleResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(SimpleResponseModel), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] UserRequestModel request)
    {
        try
        {
            // Validar que el request no sea nulo
            if (request == null)
            {
                return BadRequest(new SimpleResponseModel
                {
                    IsError = true,
                    Message = "Los datos de login son requeridos",
                    Data = null
                });
            }

            // Llamar al controlador para validar login
            var resultado = await UserController.ValidarLogin(request);

            // Retornar respuesta según el resultado
            if (resultado.IsError)
            {
                return Unauthorized(resultado);
            }

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new SimpleResponseModel
            {
                IsError = true,
                Message = $"Error interno del servidor: {ex.Message}",
                Data = null
            });
        }
    }

    /// <summary>
    /// Endpoint de prueba para verificar que la API está funcionando
    /// GET: api/UserApi/Ping
    /// </summary>
    [HttpGet("Ping")]
    public IActionResult Ping()
    {
        return Ok(new SimpleResponseModel
        {
            IsError = false,
            Message = "User API está funcionando correctamente",
            Data = DateTime.Now
        });
    }
}