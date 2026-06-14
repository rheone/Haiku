window.haikuScrollDriver = {
  observer: null,
  poemCards: [],
  animFrameId: null,
  running: false,

  init: function () {
    if (this.running) return;
    this.running = true;
    var self = this;
    this.poemCards = [];

    var cards = document.querySelectorAll("[data-theme-card]");
    cards.forEach(function (card) {
      self.poemCards.push(card);
    });

    if (this.poemCards.length === 0) {
      this.running = false;
      return;
    }

    if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) {
      this.handleReducedMotion();
      return;
    }

    this.observer = new IntersectionObserver(
      function (entries) {
        entries.forEach(function (entry) {
          var card = entry.target;
          card._intersectionRatio = entry.intersectionRatio;
        });
        self.scheduleBlend();
      },
      { threshold: [0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0] }
    );

    this.poemCards.forEach(function (card) {
      card._intersectionRatio = 0;
      self.observer.observe(card);
    });

    this.scheduleBlend();
  },

  scheduleBlend: function () {
    var self = this;
    if (this.animFrameId) return;
    this.animFrameId = requestAnimationFrame(function () {
      self.blendThemes();
      self.animFrameId = null;
    });
  },

  blendThemes: function () {
    var dominant = null;
    var maxRatio = 0;

    this.poemCards.forEach(function (card) {
      var ratio = card._intersectionRatio || 0;
      if (ratio > maxRatio) {
        maxRatio = ratio;
        dominant = card;
      }
    });

    if (!dominant) return;

    var themeKey = dominant.getAttribute("data-theme-card") || "default";
    var app = document.getElementById("haiku-app");
    if (app) {
      app.setAttribute("data-theme", themeKey);
    }
  },

  handleReducedMotion: function () {
    var self = this;
    this.observer = new IntersectionObserver(
      function (entries) {
        entries.forEach(function (entry) {
          if (entry.intersectionRatio > 0.5) {
            var card = entry.target;
            var themeKey = card.getAttribute("data-theme-card") || "default";
            var app = document.getElementById("haiku-app");
            if (app) {
              app.setAttribute("data-theme", themeKey);
            }
          }
        });
      },
      { threshold: 0.5 }
    );
    this.poemCards.forEach(function (card) {
      self.observer.observe(card);
    });
  },

  destroy: function () {
    if (this.observer) {
      this.observer.disconnect();
      this.observer = null;
    }
    if (this.animFrameId) {
      cancelAnimationFrame(this.animFrameId);
      this.animFrameId = null;
    }
    this.poemCards = [];
    this.running = false;
  }
};
