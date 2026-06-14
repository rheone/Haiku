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
    document.documentElement.setAttribute("data-color-scheme", theme);
  },
  setPoemTheme: function (themeKey) {
    var app = document.getElementById("haiku-app");
    if (app) {
      app.setAttribute("data-theme", themeKey || "default");
    }
  },
  getCurrentThemeKey: function () {
    var app = document.getElementById("haiku-app");
    return app ? app.getAttribute("data-theme") || "default" : "default";
  }
};

(function () {
  var theme = window.haikuTheme.getTheme();
  window.haikuTheme.setTheme(theme);
})();
