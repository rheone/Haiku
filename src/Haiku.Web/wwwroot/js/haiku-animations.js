window.haikuAnimations = {
  instances: [],
  running: false,

  start: function (animationKey, containerId) {
    if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) return;
    if (!animationKey) return;

    var container = document.getElementById(containerId);
    if (!container) return;

    this.stop(containerId);

    switch (animationKey) {
      case "snowfall": this.startSnowfall(container); break;
      case "leaves": this.startLeaves(container); break;
      case "petals": this.startPetals(container); break;
      case "heatshimmer": this.startHeatShimmer(container); break;
      case "fireflies": this.startFireflies(container); break;
    }
  },

  stop: function (containerId) {
    this.instances = this.instances.filter(function (inst) {
      if (inst.containerId === containerId) {
        if (inst.animFrameId) cancelAnimationFrame(inst.animFrameId);
        if (inst.canvas && inst.canvas.parentNode) {
          inst.canvas.parentNode.removeChild(inst.canvas);
        }
        return false;
      }
      return true;
    });
  },

  stopAll: function () {
    var self = this;
    this.instances.forEach(function (inst) {
      if (inst.animFrameId) cancelAnimationFrame(inst.animFrameId);
      if (inst.canvas && inst.canvas.parentNode) {
        inst.canvas.parentNode.removeChild(inst.canvas);
      }
    });
    this.instances = [];
  },

  createCanvas: function (container) {
    var canvas = document.createElement("canvas");
    canvas.style.position = "absolute";
    canvas.style.top = "-200px";
    canvas.style.left = "-200px";
    canvas.style.width = (container.offsetWidth + 400) + "px";
    canvas.style.height = (container.offsetHeight + 400) + "px";
    canvas.style.pointerEvents = "none";
    canvas.style.opacity = "0.18";
    canvas.width = container.offsetWidth + 400;
    canvas.height = container.offsetHeight + 400;
    container.style.position = "relative";
    container.appendChild(canvas);
    return canvas;
  },

  startSnowfall: function (container) {
    var canvas = this.createCanvas(container);
    var ctx = canvas.getContext("2d");
    var particles = [];
    var w = canvas.width, h = canvas.height;
    var self = this;
    var lastTime = 0;
    var fps = 30;
    var interval = 1000 / fps;

    for (var i = 0; i < 40; i++) {
      particles.push({
        x: Math.random() * w,
        y: Math.random() * h,
        r: 2 + Math.random() * 10,
        speed: 0.3 + Math.random() * 0.5,
        drift: (Math.random() - 0.5) * 0.3,
        opacity: 0.1 + Math.random() * 0.08
      });
    }

    function animate(time) {
      var elapsed = time - lastTime;
      if (elapsed < interval) { inst.animFrameId = requestAnimationFrame(animate); return; }
      lastTime = time - (elapsed % interval);

      ctx.clearRect(0, 0, w, h);
      particles.forEach(function (p) {
        p.y += p.speed;
        p.x += p.drift + Math.sin(p.y * 0.01) * 0.2;
        if (p.y > h) { p.y = -10; p.x = Math.random() * w; }
        if (p.x > w) p.x = 0;
        if (p.x < 0) p.x = w;
        ctx.beginPath();
        ctx.arc(p.x, p.y, p.r, 0, Math.PI * 2);
        ctx.fillStyle = "rgba(255,255,255," + p.opacity + ")";
        ctx.fill();
      });

      inst.animFrameId = requestAnimationFrame(animate);
    }

    var inst = { containerId: container.id, canvas: canvas, animFrameId: requestAnimationFrame(animate) };
    this.instances.push(inst);
  },

  startLeaves: function (container) {
    var canvas = this.createCanvas(container);
    var ctx = canvas.getContext("2d");
    var particles = [];
    var w = canvas.width, h = canvas.height;
    var self = this;
    var lastTime = 0;
    var fps = 30;
    var interval = 1000 / fps;

    for (var i = 0; i < 15; i++) {
      particles.push({
        x: Math.random() * w,
        y: Math.random() * h,
        size: 4 + Math.random() * 8,
        speed: 0.5 + Math.random() * 0.8,
        rotation: Math.random() * Math.PI * 2,
        rotSpeed: (Math.random() - 0.5) * 0.02,
        drift: (Math.random() - 0.5) * 0.5,
        color: Math.random() > 0.5 ? "rgba(180,100,40," : "rgba(200,150,50,",
        opacity: 0.08 + Math.random() * 0.1
      });
    }

    function animate(time) {
      var elapsed = time - lastTime;
      if (elapsed < interval) { inst.animFrameId = requestAnimationFrame(animate); return; }
      lastTime = time - (elapsed % interval);

      ctx.clearRect(0, 0, w, h);
      particles.forEach(function (p) {
        p.y += p.speed;
        p.x += p.drift + Math.sin(p.y * 0.02) * 0.3;
        p.rotation += p.rotSpeed;
        if (p.y > h) { p.y = -20; p.x = Math.random() * w; }
        if (p.x > w) p.x = -10;
        if (p.x < -10) p.x = w;

        ctx.save();
        ctx.translate(p.x, p.y);
        ctx.rotate(p.rotation);
        ctx.fillStyle = p.color + p.opacity + ")";
        ctx.beginPath();
        ctx.moveTo(0, -p.size / 2);
        ctx.quadraticCurveTo(p.size / 2, 0, 0, p.size / 2);
        ctx.quadraticCurveTo(-p.size / 2, 0, 0, -p.size / 2);
        ctx.fill();
        ctx.restore();
      });

      inst.animFrameId = requestAnimationFrame(animate);
    }

    var inst = { containerId: container.id, canvas: canvas, animFrameId: requestAnimationFrame(animate) };
    this.instances.push(inst);
  },

  startPetals: function (container) {
    var canvas = this.createCanvas(container);
    var ctx = canvas.getContext("2d");
    var particles = [];
    var w = canvas.width, h = canvas.height;
    var self = this;
    var lastTime = 0;
    var fps = 30;
    var interval = 1000 / fps;
    var spawnCounter = 0;

    for (var i = 0; i < 8; i++) {
      particles.push(this.createPetal(w, h));
    }

    function animate(time) {
      var elapsed = time - lastTime;
      if (elapsed < interval) { inst.animFrameId = requestAnimationFrame(animate); return; }
      lastTime = time - (elapsed % interval);

      spawnCounter++;
      if (spawnCounter % 30 === 0 && particles.length < 20) {
        particles.push(self.createPetal(w, h));
      }

      ctx.clearRect(0, 0, w, h);
      particles = particles.filter(function (p) {
        p.y += p.speed;
        p.x += Math.sin(p.y * 0.02) * 0.15;
        p.rotation += p.rotSpeed;
        if (p.y > h + 20) return false;

        ctx.save();
        ctx.translate(p.x, p.y);
        ctx.rotate(p.rotation);
        ctx.fillStyle = "rgba(255,150,180," + p.opacity + ")";
        ctx.beginPath();
        ctx.ellipse(0, 0, p.w / 2, p.h / 2, 0, 0, Math.PI * 2);
        ctx.fill();
        ctx.restore();
        return true;
      });

      inst.animFrameId = requestAnimationFrame(animate);
    }

    var inst = { containerId: container.id, canvas: canvas, animFrameId: requestAnimationFrame(animate) };
    this.instances.push(inst);
  },

  createPetal: function (w, h) {
    return {
      x: Math.random() * w,
      y: -20,
      w: 4 + Math.random() * 6,
      h: 2 + Math.random() * 3,
      speed: 0.3 + Math.random() * 0.4,
      rotation: Math.random() * Math.PI * 2,
      rotSpeed: (Math.random() - 0.5) * 0.01,
      opacity: 0.06 + Math.random() * 0.06
    };
  },

  startHeatShimmer: function (container) {
    var canvas = this.createCanvas(container);
    var ctx = canvas.getContext("2d");
    var w = canvas.width, h = canvas.height;
    var time = 0;
    var self = this;
    var lastFrame = 0;
    var fps = 30;
    var interval = 1000 / fps;

    function animate(timestamp) {
      var elapsed = timestamp - lastFrame;
      if (elapsed < interval) { inst.animFrameId = requestAnimationFrame(animate); return; }
      lastFrame = timestamp - (elapsed % interval);

      time += 0.02;
      ctx.clearRect(0, 0, w, h);
      ctx.globalAlpha = 0.12;

      for (var x = 0; x < w; x += 4) {
        var waveY = h - 40 + Math.sin(x * 0.02 + time * 2) * 8 + Math.sin(x * 0.01 + time) * 4;
        ctx.fillStyle = "rgba(200,180,120,0.15)";
        ctx.fillRect(x, waveY, 2, h - waveY);
      }

      ctx.globalAlpha = 1;
      inst.animFrameId = requestAnimationFrame(animate);
    }

    var inst = { containerId: container.id, canvas: canvas, animFrameId: requestAnimationFrame(animate) };
    this.instances.push(inst);
  },

  startFireflies: function (container) {
    var canvas = this.createCanvas(container);
    var ctx = canvas.getContext("2d");
    var particles = [];
    var w = canvas.width, h = canvas.height;
    var self = this;
    var lastTime = 0;
    var fps = 30;
    var interval = 1000 / fps;

    for (var i = 0; i < 12; i++) {
      particles.push(this.createFirefly(w, h));
    }

    function animate(time) {
      var elapsed = time - lastTime;
      if (elapsed < interval) { inst.animFrameId = requestAnimationFrame(animate); return; }
      lastTime = time - (elapsed % interval);

      ctx.clearRect(0, 0, w, h);
      particles.forEach(function (p) {
        p.x += Math.sin(p.phase + time * 0.001) * 0.3;
        p.y += Math.cos(p.phase + time * 0.0015) * 0.3;
        p.brightness = 0.05 + Math.abs(Math.sin(time * 0.002 + p.phase)) * 0.13;

        if (p.x < 0) p.x = w;
        if (p.x > w) p.x = 0;
        if (p.y < 0) p.y = h;
        if (p.y > h) p.y = 0;

        var gradient = ctx.createRadialGradient(p.x, p.y, 0, p.x, p.y, 8);
        gradient.addColorStop(0, "rgba(255,220,150," + p.brightness + ")");
        gradient.addColorStop(1, "rgba(255,200,100,0)");
        ctx.fillStyle = gradient;
        ctx.beginPath();
        ctx.arc(p.x, p.y, 8, 0, Math.PI * 2);
        ctx.fill();
      });

      inst.animFrameId = requestAnimationFrame(animate);
    }

    var inst = { containerId: container.id, canvas: canvas, animFrameId: requestAnimationFrame(animate) };
    this.instances.push(inst);
  },

  createFirefly: function (w, h) {
    return {
      x: Math.random() * w,
      y: Math.random() * h,
      phase: Math.random() * Math.PI * 2,
      brightness: 0.05 + Math.random() * 0.13
    };
  }
};
