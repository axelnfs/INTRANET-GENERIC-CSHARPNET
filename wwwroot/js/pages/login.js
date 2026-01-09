$(document).ready(function () {
    console.log('Página de login cargada');

    // Validar si ya hay sesión activa
    if (SessionManager.redirectIfAuthenticated()) {
        return;
    }

    // Click en el botón
    $('#btnLogin').on('click', function(e) {
        e.preventDefault();
        iniciarSesion();
    });

    // Enter en los inputs
    $('#username, #password').on('keypress', function (e) {
        if (e.which === 13) {
            e.preventDefault();
            iniciarSesion();
        }
    });
});

async function iniciarSesion() {
    const nombreUsuario = $('#username').val().trim();
    const contrasena = $('#password').val();

    // Validación básica
    if (!nombreUsuario || !contrasena) {
        mostrarMensaje('Por favor, complete todos los campos', 'warning');
        return;
    }

    // Deshabilitar botón
    const btnSubmit = $('#btnLogin');
    const btnTextOriginal = btnSubmit.html();
    btnSubmit.prop('disabled', true).html('<i class="bi bi-hourglass-split"></i> Iniciando sesión...');

    try {
        const response = await $.ajax({
            url: '/api/UserApi/Login',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({
                nombreUsuario: nombreUsuario,
                contrasena: contrasena
            })
        });

        if (response.isError) {
            mostrarMensaje(response.message, 'error');
        } else {
            // Guardar sesión
            SessionManager.setSession(nombreUsuario);
            
            // Mostrar éxito
            mostrarMensaje('¡Bienvenido! Redirigiendo...', 'success');
            
            // Redirigir
            setTimeout(() => {
                window.location.href = '/';
            }, 1000);
        }
    } catch (error) {
        console.error('Error al iniciar sesión:', error);
        
        let mensajeError = 'Error al iniciar sesión';
        if (error.responseJSON && error.responseJSON.message) {
            mensajeError = error.responseJSON.message;
        }
        
        mostrarMensaje(mensajeError, 'error');
    } finally {
        // Rehabilitar botón
        btnSubmit.prop('disabled', false).html(btnTextOriginal);
    }
}