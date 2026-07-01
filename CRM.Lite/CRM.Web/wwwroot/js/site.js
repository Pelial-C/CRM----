(function () {
    function normalizePath(path) {
        if (!path) return "/";
        return path.length > 1 ? path.replace(/\/$/, "").toLowerCase() : path.toLowerCase();
    }

    document.addEventListener("DOMContentLoaded", function () {
        var currentPath = normalizePath(window.location.pathname);

        document.querySelectorAll(".sidebar-link[href]").forEach(function (link) {
            var href = link.getAttribute("href");
            if (!href || href === "#" || link.classList.contains("disabled")) return;

            var linkPath;
            try {
                linkPath = normalizePath(new URL(href, window.location.origin).pathname);
            } catch {
                return;
            }

            if (currentPath === linkPath) {
                link.classList.add("active");
            }
        });

        var mobileSidebar = document.getElementById("mobileSidebar");
        if (mobileSidebar && window.bootstrap) {
            mobileSidebar.querySelectorAll(".sidebar-link:not(.disabled)").forEach(function (link) {
                link.addEventListener("click", function () {
                    var instance = bootstrap.Offcanvas.getInstance(mobileSidebar);
                    if (instance) instance.hide();
                });
            });
        }

        document.querySelectorAll("[data-confirm]").forEach(function (element) {
            element.addEventListener("click", function (event) {
                var message = element.getAttribute("data-confirm") || "确认执行该操作？";
                if (!window.confirm(message)) {
                    event.preventDefault();
                    event.stopPropagation();
                }
            });
        });

        document.querySelectorAll("form[data-confirm]").forEach(function (form) {
            form.addEventListener("submit", function (event) {
                var message = form.getAttribute("data-confirm") || "确认执行该操作？";
                if (!window.confirm(message)) {
                    event.preventDefault();
                }
            });
        });
    });
})();
