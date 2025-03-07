document.addEventListener("DOMContentLoaded", function () {
    const toggleIcons = document.querySelectorAll('.toggle-icon');
    const menuToggles = document.querySelectorAll('.section_header-subtitle');
    const submenuToggles = document.querySelectorAll('.menu_tab-list_item > .main');

    function toggleMenu(event) {
        const target = event.currentTarget;
        const menuList = target.nextElementSibling;
        const icon = target.querySelector('.toggle-icon');

        menuList.classList.toggle('show');

        if (menuList.classList.contains('show')) {
            icon.textContent = '▲'; // Açık durumdayken ok yukarıya doğru gösterilir
        } else {
            icon.textContent = '▼'; // Kapalı durumdayken ok aşağıya doğru gösterilir
        }
    }

    function toggleSubMenu(event) {
        const target = event.currentTarget;
        const submenuList = target.nextElementSibling;
        const icon = target.querySelector('.toggle-icon');

        submenuList.classList.toggle('show');

        if (submenuList.classList.contains('show')) {
            icon.textContent = '▲'; // Açık durumdayken ok yukarıya doğru gösterilir
        } else {
            icon.textContent = '▼'; // Kapalı durumdayken ok aşağıya doğru gösterilir
        }
    }

    menuToggles.forEach(toggle => {
        toggle.addEventListener('click', toggleMenu);
    });

    submenuToggles.forEach(toggle => {
        toggle.addEventListener('click', toggleSubMenu);
    });
});