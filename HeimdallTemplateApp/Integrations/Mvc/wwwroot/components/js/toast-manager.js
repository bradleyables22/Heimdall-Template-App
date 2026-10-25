(function () {
    "use strict";

    function initToast(el) {
        if (!el || el.__toastInit)
            return;
        el.__toastInit = true;

        if (!window.bootstrap || !window.bootstrap.Toast)
            return;

        const t = window.bootstrap.Toast.getOrCreateInstance(el);
        el.addEventListener("hidden.bs.toast", () => el.remove(), { once: true });
        t.show();
    }

    function initAll() {
        const host = document.getElementById("toast-manager");
        if (!host)
            return;
        host.querySelectorAll(".toast").forEach(initToast);
    }

    document.addEventListener("DOMContentLoaded", () => {
        initAll();

        const host = document.getElementById("toast-manager");
        if (!host)
            return;

        new MutationObserver(muts => {
            for (const m of muts) {
                for (const n of m.addedNodes) {
                    if (n.nodeType !== 1)
                        continue;
                    if (n.matches?.(".toast"))
                        initToast(n);
                    n.querySelectorAll?.(".toast").forEach(initToast);
                }
            }
        }).observe(host, { childList: true, subtree: true });
    });
})();