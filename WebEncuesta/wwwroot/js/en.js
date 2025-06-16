document.addEventListener('DOMContentLoaded', function() {
    const menu = document.getElementById("menu");
    const btnAbrir = document.querySelector(".btn-menu");
    const btnCerrar = document.querySelector(".btn-cerrar");
    
    btnAbrir.addEventListener("click", (e) => {
        e.stopPropagation();
        menu.classList.toggle("nav_show");
    });
    
    btnCerrar.addEventListener("click", (e) => {
        e.stopPropagation();
        menu.classList.remove("nav_show");
    });
    
    document.addEventListener("click", (e) => {
        if (!menu.contains(e.target) ){
            menu.classList.remove("nav_show");
        }
    });
});

document.addEventListener('DOMContentLoaded', function() {
    document.querySelectorAll('.btn-editar').forEach(btn => {
      btn.addEventListener('click', function(e) {
        e.stopPropagation();
        const tarjeta = this.closest('.tarjeta');
      });
    });
  
    document.querySelectorAll('.btn-eliminar').forEach(btn => {
      btn.addEventListener('click', function(e) {
        e.stopPropagation();
        const tarjeta = this.closest('.tarjeta');

      });
    });
  });

          
function showModalEliminar() {
    window.location.href = '#openModalEliminar';
 }
 
 function hideModal2() {
    window.location.href = '#';
 }
 function hideModal() {
    window.location.href = '#';
 }