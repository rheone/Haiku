window.haikuTheme = {
    getTheme: function () {
        var saved = localStorage.getItem("haiku-theme");
        if (saved === "light" || saved === "dark") return saved;
        var prefersDark = window.matchMedia("(prefers-color-scheme: dark)").matches;
        return prefersDark ? "dark" : "light";
    },
    setTheme: function (theme) {
        localStorage.setItem("haiku-theme", theme);
        document.documentElement.setAttribute("data-bs-theme", theme);
    }
};

(function () {
    var theme = window.haikuTheme.getTheme();
    window.haikuTheme.setTheme(theme);
})();
