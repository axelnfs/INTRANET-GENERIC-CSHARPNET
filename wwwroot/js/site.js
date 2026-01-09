// ========================================
// GESTIÓN DE SESIONES
// ========================================

const SessionManager = {
    // Claves para localStorage
    STORAGE_KEYS: {
        USER: 'app_user',
        LOGIN_TIME: 'app_login_time',
        SESSION_ACTIVE: 'app_session_active'
    },

    // Tiempo de expiración de sesión (en milisegundos) - 2 horas
    SESSION_TIMEOUT: 2 * 60 * 60 * 1000,

    /**
     * Guarda la sesión del usuario
     */
    setSession(nombreUsuario) {
        const sessionData = {
            nombreUsuario: nombreUsuario,
            loginTime: new Date().toISOString(),
            isActive: true
        };

        localStorage.setItem(this.STORAGE_KEYS.USER, nombreUsuario);
        localStorage.setItem(this.STORAGE_KEYS.LOGIN_TIME, sessionData.loginTime);
        localStorage.setItem(this.STORAGE_KEYS.SESSION_ACTIVE, 'true');

        console.log('Sesión iniciada:', nombreUsuario);
    },

    /**
     * Obtiene la información de la sesión actual
     */
    getSession() {
        return {
            nombreUsuario: localStorage.getItem(this.STORAGE_KEYS.USER),
            loginTime: localStorage.getItem(this.STORAGE_KEYS.LOGIN_TIME),
            isActive: localStorage.getItem(this.STORAGE_KEYS.SESSION_ACTIVE) === 'true'
        };
    },

    /**
     * Verifica si la sesión está activa y no ha expirado
     */
    isSessionValid() {
        const session = this.getSession();

        if (!session.isActive || !session.nombreUsuario || !session.loginTime) {
            return false;
        }

        // Verificar si la sesión ha expirado
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

    /**
     * Limpia la sesión (logout)
     */
    clearSession() {
        localStorage.removeItem(this.STORAGE_KEYS.USER);
        localStorage.removeItem(this.STORAGE_KEYS.LOGIN_TIME);
        localStorage.removeItem(this.STORAGE_KEYS.SESSION_ACTIVE);
        console.log('Sesión cerrada');
    },

    /**
     * Obtiene el nombre del usuario actual
     */
    getCurrentUser() {
        if (this.isSessionValid()) {
            return this.getSession().nombreUsuario;
        }
        return null;
    },

    /**
     * Redirige a login si no hay sesión activa
     */
    requireAuth() {
        if (!this.isSessionValid()) {
            console.log('Sesión no válida, redirigiendo a login...');
            window.location.href = '/Login';
            return false;
        }
        return true;
    },

    /**
     * Redirige al home si ya hay sesión activa
     */
    redirectIfAuthenticated() {
        if (this.isSessionValid()) {
            console.log('Usuario ya autenticado, redirigiendo a home...');
            window.location.href = '/';
            return true;
        }
        return false;
    }
};

// ========================================
// INICIALIZACIÓN GLOBAL
// ========================================

$(document).ready(function () {
    console.log('Aplicación iniciada con jQuery');

    // Verificar sesión al cargar la página
    const currentPath = window.location.pathname.toLowerCase();
    const publicPages = ['/login', '/register'];
    const isPublicPage = publicPages.some(page => currentPath.includes(page));

    // Si no es una página pública, verificar autenticación
    if (!isPublicPage) {
        if (!SessionManager.isSessionValid()) {
            console.log('No hay sesión activa, redirigiendo a login...');
            window.location.href = '/Login';
            return;
        } else {
            updateNavbarWithUser();
            addLogoutButton();
        }
    }

    // ⚠️ COMENTAR ESTA REDIRECCIÓN AUTOMÁTICA
    // if (isPublicPage && SessionManager.isSessionValid()) {
    //     console.log('Usuario ya autenticado, redirigiendo a home...');
    //     mostrarMensaje('Ya tienes una sesión activa', 'info');
    //     setTimeout(() => {
    //         window.location.href = '/';
    //     }, 1000);
    //     return;
    // }

    // ❌ ELIMINAR $.ajaxSetup COMPLETAMENTE - Ya no es necesario
});

/**
 * Actualiza el navbar con información del usuario
 */
function updateNavbarWithUser() {
    const userName = SessionManager.getCurrentUser();
    if (userName) {
        const userInfo = `
            <li class="nav-item">
                <span class="nav-link">
                    <i class="bi bi-person-circle"></i> ${userName}
                </span>
            </li>
        `;
        $('.navbar-nav').append(userInfo);
    }
}

/**
 * Agrega botón de logout al navbar
 */
function addLogoutButton() {
    const logoutButton = `
        <li class="nav-item">
            <a class="nav-link" href="#" id="btnLogout">
                <i class="bi bi-box-arrow-right"></i> Cerrar Sesión
            </a>
        </li>
    `;
    $('.navbar-nav').append(logoutButton);

    // Evento para logout
    $(document).on('click', '#btnLogout', function (e) {
        e.preventDefault();
        logout();
    });
}

/**
 * Cierra la sesión del usuario
 */
function logout() {
    Swal.fire({
        title: '¿Cerrar sesión?',
        text: '¿Está seguro que desea cerrar sesión?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Sí, cerrar sesión',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            SessionManager.clearSession();

            Swal.fire({
                icon: 'success',
                title: 'Sesión cerrada',
                text: 'Has cerrado sesión exitosamente',
                timer: 1500,
                showConfirmButton: false
            }).then(() => {
                window.location.href = '/Login';
            });
        }
    });
}

// ========================================
// HELPERS
// ========================================

// Helper para mostrar mensajes con Bootstrap
function mostrarMensaje(mensaje, tipo = 'info', opciones = {}) {
    const iconos = {
        'success': 'success',
        'error': 'error',
        'warning': 'warning',
        'info': 'info',
        'question': 'question'
    };

    Swal.fire({
        icon: iconos[tipo] || 'info',
        title: mensaje,
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true,
        toast: true,
        position: 'top-end',
        ...opciones
    });
}

function mostrarAlerta(titulo, mensaje, tipo = 'info') {
    return Swal.fire({
        icon: tipo,
        title: titulo,
        text: mensaje,
        confirmButtonText: 'Aceptar',
        confirmButtonColor: '#3085d6'
    });
}

function confirmar(titulo, mensaje, textoConfirmar = 'Sí', textoCancelar = 'No') {
    return Swal.fire({
        title: titulo,
        text: mensaje,
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: textoConfirmar,
        cancelButtonText: textoCancelar
    });
}

function mostrarLoading(mensaje = 'Procesando...') {
    Swal.fire({
        title: mensaje,
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
}

function cerrarLoading() {
    Swal.close();
}

// Helper para manejar errores
function manejarError(error) {
    console.error('Error:', error);
    mostrarMensaje('Ha ocurrido un error: ' + error.message, 'error');
}

// ========================================
// UTILIDADES EXPORTADAS
// ========================================

// Hacer SessionManager disponible globalmente
window.SessionManager = SessionManager;