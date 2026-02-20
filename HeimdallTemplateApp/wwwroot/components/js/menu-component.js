document.addEventListener("DOMContentLoaded", function () {
    const currentPath = window.location.pathname.replace(/\/$/, "");
    const links = document.querySelectorAll("#navList a");

    links.forEach(link => {
        const linkPath = new URL(link.href).pathname.replace(/\/$/, "");

        if (linkPath === currentPath) {
            link.classList.add("active");
            link.setAttribute("aria-current", "page");
        }
    });
});