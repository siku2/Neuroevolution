(function() {
  let timeouts = [];
  let messageName = "zero-timeout-message";

  function setZeroTimeout(fn) {
    timeouts.push(fn);
    window.postMessage(messageName, "*");
  }

  function handleMessage(event) {
    if (event.source == window && event.data == messageName) {
      event.stopPropagation();
      if (timeouts.length > 0) {
        let fn = timeouts.shift();
        fn();
      }
    }
  }

  window.addEventListener("message", handleMessage, true);

  window.setZeroTimeout = setZeroTimeout;
})();

let Neuvol;
let game;
let floorHeight;
let FPS = 60;
let maxScore = 0;

let images = {};

let speed = function(fps) {
  FPS = parseInt(fps);
}

let loadImages = function(sources, callback) {
  let nb = 0;
  let loaded = 0;
  let imgs = {};
  for (let i in sources) {
    nb++;
    imgs[i] = new Image();
    imgs[i].src = sources[i];
    imgs[i].onload = function() {
      loaded++;
      if (loaded == nb) {
        callback(imgs);
      }
    }
  }
}

let Rodent = function(json) {
  this.x = 80;
  this.width = 40;
  this.height = 30;
  this.y = floorHeight - this.height;

  this.alive = true;
  this.gravity = 0;
  this.velocity = 0.2;
  this.jumpAccel = -7;
  this.jumpTrigger = .5;

  this.init(json);
}

Rodent.prototype.init = function(json) {
  for (let i in json) {
    this[i] = json[i];
  }
}

Rodent.prototype.jump = function(res) {
  if (this.y === floorHeight - this.height) {
    if (res > this.jumpTrigger) {
      this.gravity = this.jumpAccel;
    }
  }
}

Rodent.prototype.update = function() {
  this.gravity += this.velocity;
  this.y += this.gravity;
  this.y = Math.min(this.y, floorHeight - this.height);
  if (this.y === floorHeight - this.height) {
    this.gravity = 0;
  }
}

Rodent.prototype.isDead = function(height, hurdles) {
  if (this.y >= height || this.y + this.height <= 0) {
    return true;
  }
  for (let i in hurdles) {
    if (!(
        this.x > hurdles[i].x + hurdles[i].width ||
        this.x + this.width < hurdles[i].x ||
        this.y > hurdles[i].y + hurdles[i].height ||
        this.y + this.height < hurdles[i].y
      )) {
      return true;
    }
  }
}

let Hurdle = function(json) {
  this.x = 0;
  this.y = 0;
  this.width = 24;
  this.height = 40;
  this.speed = 3;

  this.init(json);
}

Hurdle.prototype.init = function(json) {
  for (let i in json) {
    this[i] = json[i];
  }
}

Hurdle.prototype.update = function() {
  this.x -= this.speed;
}

Hurdle.prototype.isOut = function() {
  if (this.x + this.width < 0) {
    return true;
  }
}

let Game = function() {
  this.graph_canvas = document.querySelector("#graph");
  this.graph_ctx = this.graph_canvas.getContext("2d");
  this.graph = new Chart(this.graph_ctx, {
    type: "line",
    data: {
      datasets: [{
        label: "progress"
      }]
    },
    options: {
      responsive: false,
      scales: {
        xAxes: [{
          scaleLabel: {
            display: true,
            labelString: "Generation"
          }
        }],
        yAxes: [{
          scaleLabel: {
            display: true,
            labelString: "Fitness"
          }
        }]
      }
    }
  });

  this.hurdles = [];
  this.rodents = [];
  this.score = 0;
  this.canvas = document.querySelector("#game");
  this.ctx = this.canvas.getContext("2d");
  this.width = this.canvas.width;
  this.height = this.canvas.height;
  floorHeight = this.height - 10;
  this.spawnInterval = 90;
  this.interval = 0;
  this.gen = [];
  this.alives = 0;
  this.generation = 0;
  this.backgroundSpeed = 0.5;
  this.backgroundx = 0;
  this.maxScore = 0;
}

Game.prototype.start = function() {
  this.graph.data.labels.push(this.generation);
  this.graph.data.datasets.forEach(dataset => dataset.data.push(this.score));
  this.graph.update(0);

  this.interval = 0;
  this.score = 0;
  this.hurdles = [];
  this.rodents = [];

  this.gen = Neuvol.nextGeneration();
  for (let i in this.gen) {
    let b = new Rodent();
    this.rodents.push(b)
  }
  this.generation++;
  this.alives = this.rodents.length;
}

Game.prototype.update = function() {
  this.backgroundx += this.backgroundSpeed;
  let nextHurdleDist = 0;
  let nextHurdleHeight = 0;
  if (this.rodents.length > 0) {
    for (let i = 0; i < this.hurdles.length; i += 2) {
      if (this.hurdles[i].x + this.hurdles[i].width > this.rodents[0].x) {
        nextHurdleDist = this.hurdles[i].x / this.width;
        nextHurdleHeight = this.hurdles[i].y / this.height;
        break;
      }
    }
  }

  let inputs = [
    1 - nextHurdleDist,
    nextHurdleHeight
  ];

  for (let i in this.rodents) {
    if (this.rodents[i].alive) {
      let res = this.gen[i].compute(inputs);
      this.rodents[i].jump(res);

      this.rodents[i].update();
      if (this.rodents[i].isDead(this.height, this.hurdles)) {
        this.rodents[i].alive = false;
        this.alives--;
        //console.log(this.alives);
        Neuvol.networkScore(this.gen[i], this.score);
        if (this.isItEnd()) {
          this.start();
        }
      }
    }
  }

  for (let i = 0; i < this.hurdles.length; i++) {
    this.hurdles[i].update();
    if (this.hurdles[i].isOut()) {
      this.hurdles.splice(i, 1);
      i--;
    }
  }

  if (this.interval == 0) {
    let minHeight = 20;
    let maxHeight = 115;
    let hollPosition = Math.round(this.height - (Math.random() * (maxHeight - minHeight)) - minHeight);
    this.hurdles.push(new Hurdle({
      x: this.width,
      y: hollPosition,
      height: this.height
    }));
  }

  this.interval++;
  if (this.interval == this.spawnInterval) {
    this.interval = 0;
  }

  this.score++;
  this.maxScore = (this.score > this.maxScore) ? this.score : this.maxScore;
  let self = this;

  if (FPS == 0) {
    setZeroTimeout(function() {
      self.update();
    });
  } else {
    setTimeout(function() {
      self.update();
    }, 1000 / FPS);
  }
}


Game.prototype.isItEnd = function() {
  for (let i in this.rodents) {
    if (this.rodents[i].alive) {
      return false;
    }
  }
  return true;
}

Game.prototype.display = function() {
  this.ctx.clearRect(0, 0, this.width, this.height);
  for (let i = 0; i < Math.ceil(this.width / images.background.width) + 1; i++) {
    this.ctx.drawImage(images.background, i * images.background.width - Math.floor(this.backgroundx % images.background.width), this.height - images.background.height);
  }

  for (let i in this.hurdles) {
      this.ctx.drawImage(images.hurdle, this.hurdles[i].x, this.hurdles[i].y, this.hurdles[i].width, images.hurdle.height);
  }

  this.ctx.fillStyle = "#01032d";
  this.ctx.fillRect(0, floorHeight, this.width, this.height);

  this.ctx.fillStyle = "#FFC600";
  this.ctx.strokeStyle = "#CE9E00";
  for (let i in this.rodents) {
    if (this.rodents[i].alive) {
      this.ctx.save();
      this.ctx.translate(this.rodents[i].x + this.rodents[i].width / 2, this.rodents[i].y + this.rodents[i].height / 2);
      this.ctx.rotate(Math.PI / 2 * this.rodents[i].gravity / 20);
      this.ctx.drawImage(images.rodent, -this.rodents[i].width / 2, -this.rodents[i].height / 2, this.rodents[i].width, this.rodents[i].height);
      this.ctx.restore();
    }
  }

  this.ctx.fillStyle = "black";
  this.ctx.font = "20px Oswald, sans-serif";
  this.ctx.fillText("Fitness : " + this.score, 10, 25);
  this.ctx.fillText("Highest Fitness : " + this.maxScore, 10, 50);
  this.ctx.fillText("Generation : " + this.generation, 10, 75);
  this.ctx.fillText("Alive : " + this.alives + " / " + Neuvol.options.population, 10, 100);

  let self = this;
  requestAnimationFrame(function() {
    self.display();
  });
}

window.onload = function() {
  let sprites = {
    rodent: "./images/rodent.png",
    background: "./images/background.png",
    hurdle: "./images/hurdle.png"
  }

  let start = function() {
    Neuvol = new Neuroevolution({
      population: 50,
      network: [2, [2], 1],
    });
    game = new Game();
    game.start();
    game.update();
    game.display();
  }


  loadImages(sprites, function(imgs) {
    images = imgs;
    start();
  })
}
