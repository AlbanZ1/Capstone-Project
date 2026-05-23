// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(() => {
    const countdowns = Array.from(document.querySelectorAll(".js-auction-countdown"));

    if (!countdowns.length) {
        return;
    }

    const pad = (value) => value.toString().padStart(2, "0");

    const formatDuration = (milliseconds) => {
        const totalSeconds = Math.max(0, Math.floor(milliseconds / 1000));
        const days = Math.floor(totalSeconds / 86400);
        const hours = Math.floor((totalSeconds % 86400) / 3600);
        const minutes = Math.floor((totalSeconds % 3600) / 60);
        const seconds = totalSeconds % 60;

        return `${days}d ${pad(hours)}h ${pad(minutes)}m ${pad(seconds)}s`;
    };

    const parseDate = (value) => {
        const parsed = value ? new Date(value) : null;
        return parsed && !Number.isNaN(parsed.getTime()) ? parsed : null;
    };

    const setState = (element, stateClass) => {
        element.classList.remove("countdown-live", "countdown-pending", "countdown-closed");
        element.classList.add(stateClass);
    };

    const updateCountdown = (element, now) => {
        const status = (element.dataset.status || "").toLowerCase();
        const startTime = parseDate(element.dataset.startTime);
        const endTime = parseDate(element.dataset.endTime);

        if (status === "closed" || !endTime || now >= endTime) {
            element.textContent = "Closed";
            setState(element, "countdown-closed");
            return;
        }

        if (status === "pending" && startTime && now < startTime) {
            element.textContent = `Starts in ${formatDuration(startTime - now)}`;
            setState(element, "countdown-pending");
            return;
        }

        element.textContent = formatDuration(endTime - now);
        setState(element, "countdown-live");
    };

    const tick = () => {
        const now = new Date();
        countdowns.forEach((element) => updateCountdown(element, now));
    };

    tick();
    window.setInterval(tick, 1000);
})();
