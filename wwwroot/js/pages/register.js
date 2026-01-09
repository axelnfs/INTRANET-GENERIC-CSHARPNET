$(document).ready(function () {
    console.log('Página de registro cargada');

    // Validar si ya hay sesión activa
    if (SessionManager.redirectIfAuthenticated()) {
        return;
    }

    // Click en el botón
    $('#btnRegister').on('click', registrarUsuario);

    // Validación en tiempo real
    $('#confirmarContrasena').on('input', function () {
        validarContrasenas();
    });
});

function validarContrasenas() {
    const contrasena = $('#contrasena').val();
    const confirmarContrasena = $('#confirmarContrasena').val();
    const inputConfirmar = $('#confirmarContrasena');

    if (confirmarContrasena.length > 0) {
        if (contrasena === confirmarContrasena) {
            inputConfirmar.removeClass('is-invalid').addClass('is-valid');
        } else {
            inputConfirmar.removeClass('is-valid').addClass('is-invalid');
        }
    } else {
        inputConfirmar.removeClass('is-valid is-invalid');
    }
}

async function registrarUsuario() {
    const nombreUsuario = $('#nombreUsuario').val().trim();
    const contrasena = $('#contrasena').val();
    const confirmarContrasena = $('#confirmarContrasena').val();

    // Validaciones
    if (!nombreUsuario || !contrasena || !confirmarContrasena) {
        mostrarMensaje('Complete todos los campos', 'warning');
        return;
    }

    if (contrasena !== confirmarContrasena) {
        mostrarMensaje('Las contraseñas no coinciden', 'warning');
        return;
    }

    const btnSubmit = $('#registerForm button[type="submit"]');
    const btnTextOriginal = btnSubmit.html();
    btnSubmit.prop('disabled', true).html('<i class="bi bi-hourglass-split"></i> Registrando...');

    try {
        const response = await $.ajax({
            url: '/api/UserApi/Register',
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
            await Swal.fire({
                icon: 'success',
                title: '¡Registro Exitoso!',
                text: `Usuario creado con ID: ${response.data}`,
                confirmButtonText: 'Ir a Login'
            });

            window.location.href = '/Login';
        }
    } catch (error) {
        console.error('Error:', error);
        mostrarMensaje(error.responseJSON?.message || 'Error al registrar', 'error');
    } finally {
        btnSubmit.prop('disabled', false).html(btnTextOriginal);
    }
}

function mostrarAlerta(mensaje, tipo = 'info') {
    const alertClass = {
        'info': 'alert-info',
        'success': 'alert-success',
        'warning': 'alert-warning',
        'danger': 'alert-danger'
    }[tipo];

    const iconClass = {
        'info': 'bi-info-circle',
        'success': 'bi-check-circle',
        'warning': 'bi-exclamation-triangle',
        'danger': 'bi-x-circle'
    }[tipo];

    const alert = `
        <div class="alert ${alertClass} alert-dismissible fade show position-fixed top-0 start-50 translate-middle-x mt-3" 
             style="z-index: 9999; min-width: 350px; max-width: 500px;" role="alert">
            <i class="bi ${iconClass} me-2"></i>
            ${mensaje}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;

    $('body').append(alert);

    // Auto cerrar después de 5 segundos
    setTimeout(() => {
        $('.alert').fadeOut(300, function () {
            $(this).remove();
        });
    }, 5000);
}