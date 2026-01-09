document.addEventListener('DOMContentLoaded', () => {
    console.log('Página de inicio cargada');

    // Verificar autenticación
    if (!SessionManager.requireAuth()) {
        return;
    }

    // Mostrar información del usuario
    const userName = SessionManager.getCurrentUser();
    console.log('Usuario autenticado:', userName);

    // Aquí puedes agregar lógica específica de la página
    inicializarHome();
});

function inicializarHome() {
    const featureCards = document.querySelectorAll('.feature-card');

    featureCards.forEach((card, index) => {
        setTimeout(() => {
            card.style.opacity = '0';
            card.style.transform = 'translateY(20px)';
            card.style.transition = 'all 0.5s ease';

            requestAnimationFrame(() => {
                card.style.opacity = '1';
                card.style.transform = 'translateY(0)';
            });
        }, index * 100);
    });
}