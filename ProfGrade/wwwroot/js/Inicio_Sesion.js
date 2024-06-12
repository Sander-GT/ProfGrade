const wrapper = document.querySelector('.wrapper');
const recuperar = document.querySelector('.formato');
const login = document.querySelector('.Link-login');
const btnInicioSesion = document.querySelector('.btnInicioSesion');
const cerrar = document.querySelector('.cerrar');

recuperar.addEventListener('click', () => {
    wrapper.classList.add('active');
});

login.addEventListener('click', () => {
    wrapper.classList.remove('active');
});

btnInicioSesion.addEventListener('click', () => {
    wrapper.classList.add('active-popup');
});

cerrar.addEventListener('click', () => {
    wrapper.classList.remove('active-popup');
});
