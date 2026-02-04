const canvas = document.getElementById("game");
const ctx = canvas.getContext("2d");

const lifeEl = document.getElementById("life");
const waveEl = document.getElementById("wave");
const killsEl = document.getElementById("kills");

const WORLD = {
  width: 320,
  height: 180,
  gravity: 0.35,
};

const palette = {
  sky: "#0b0f1c",
  moon: "#f2f1e1",
  floor: "#2b2140",
  platform: "#3a315a",
  player: "#6ef3ff",
  cloak: "#3f5d9b",
  enemy: "#c23b4b",
  enemyEye: "#ffe66d",
  hit: "#ffe066",
  text: "#e6e8f2",
};

const keys = new Set();
let gameState;

const levels = [
  [
    { x: 0, y: 160, w: 320, h: 20 },
    { x: 40, y: 120, w: 70, h: 8 },
    { x: 140, y: 92, w: 50, h: 8 },
    { x: 220, y: 120, w: 60, h: 8 },
    { x: 90, y: 60, w: 40, h: 8 },
  ],
  [
    { x: 0, y: 160, w: 320, h: 20 },
    { x: 30, y: 120, w: 50, h: 8 },
    { x: 100, y: 100, w: 60, h: 8 },
    { x: 190, y: 110, w: 60, h: 8 },
    { x: 250, y: 70, w: 40, h: 8 },
  ],
  [
    { x: 0, y: 160, w: 320, h: 20 },
    { x: 60, y: 130, w: 50, h: 8 },
    { x: 140, y: 110, w: 60, h: 8 },
    { x: 40, y: 90, w: 45, h: 8 },
    { x: 210, y: 90, w: 60, h: 8 },
    { x: 120, y: 60, w: 70, h: 8 },
  ],
];

const spawnPoints = [
  { x: 20, y: 140 },
  { x: 280, y: 140 },
  { x: 160, y: 50 },
  { x: 80, y: 100 },
];

function createPlayer() {
  return {
    x: 160,
    y: 90,
    w: 12,
    h: 18,
    vx: 0,
    vy: 0,
    speed: 1.5,
    jump: 5.5,
    grounded: false,
    facing: 1,
    attackTimer: 0,
    invuln: 0,
    life: 5,
  };
}

function createEnemy(x, y, tier) {
  return {
    x,
    y,
    w: 12,
    h: 14,
    vx: tier % 2 === 0 ? -0.6 : 0.6,
    vy: 0,
    speed: 0.6 + tier * 0.05,
    hp: 2 + Math.floor(tier / 2),
    stagger: 0,
  };
}

function resetGame() {
  gameState = {
    player: createPlayer(),
    enemies: [],
    platforms: levels[0],
    wave: 1,
    kills: 0,
    spawnTimer: 0,
    waveRemaining: 6,
    gameOver: false,
    win: false,
    messageTimer: 120,
  };
  updateHud();
}

function updateHud() {
  lifeEl.textContent = gameState.player.life;
  waveEl.textContent = gameState.wave;
  killsEl.textContent = gameState.kills;
}

function setWave(level) {
  gameState.wave = level;
  gameState.platforms = levels[(level - 1) % levels.length];
  gameState.waveRemaining = 6 + level * 2;
  gameState.spawnTimer = 40;
  gameState.messageTimer = 120;
  updateHud();
}

function spawnEnemy() {
  const spawn = spawnPoints[Math.floor(Math.random() * spawnPoints.length)];
  gameState.enemies.push(createEnemy(spawn.x, spawn.y, gameState.wave));
}

function handleInput() {
  const { player } = gameState;
  player.vx = 0;
  if (keys.has("ArrowLeft") || keys.has("a")) {
    player.vx = -player.speed;
    player.facing = -1;
  }
  if (keys.has("ArrowRight") || keys.has("d")) {
    player.vx = player.speed;
    player.facing = 1;
  }
  if ((keys.has("ArrowUp") || keys.has("z")) && player.grounded) {
    player.vy = -player.jump;
    player.grounded = false;
  }
  if ((keys.has("x") || keys.has(" ")) && player.attackTimer <= 0) {
    player.attackTimer = 12;
  }
}

function applyPhysics(entity) {
  entity.vy += WORLD.gravity;
  entity.x += entity.vx;
  entity.y += entity.vy;
}

function resolveCollisions(entity, platforms) {
  entity.grounded = false;
  for (const platform of platforms) {
    const overlaps =
      entity.x < platform.x + platform.w &&
      entity.x + entity.w > platform.x &&
      entity.y < platform.y + platform.h &&
      entity.y + entity.h > platform.y;
    if (!overlaps) continue;

    const prevY = entity.y - entity.vy;
    if (prevY + entity.h <= platform.y) {
      entity.y = platform.y - entity.h;
      entity.vy = 0;
      entity.grounded = true;
    } else if (prevY >= platform.y + platform.h) {
      entity.y = platform.y + platform.h;
      entity.vy = 0;
    } else if (entity.x + entity.w / 2 < platform.x + platform.w / 2) {
      entity.x = platform.x - entity.w;
      entity.vx *= -0.5;
    } else {
      entity.x = platform.x + platform.w;
      entity.vx *= -0.5;
    }
  }
}

function updatePlayer() {
  const { player, platforms } = gameState;
  handleInput();
  applyPhysics(player);
  resolveCollisions(player, platforms);
  player.x = Math.max(0, Math.min(WORLD.width - player.w, player.x));
  if (player.y > WORLD.height) {
    player.life = Math.max(0, player.life - 1);
    player.x = 160;
    player.y = 90;
    player.vy = 0;
    player.invuln = 60;
  }
  if (player.attackTimer > 0) {
    player.attackTimer -= 1;
  }
  if (player.invuln > 0) {
    player.invuln -= 1;
  }
}

function enemyLogic(enemy) {
  const { player, platforms } = gameState;
  if (enemy.stagger > 0) {
    enemy.stagger -= 1;
  } else {
    enemy.vx = player.x < enemy.x ? -enemy.speed : enemy.speed;
  }
  applyPhysics(enemy);
  resolveCollisions(enemy, platforms);

  enemy.x = Math.max(0, Math.min(WORLD.width - enemy.w, enemy.x));
}

function handleCombat() {
  const { player } = gameState;
  if (player.attackTimer > 6) {
    const hitBox = {
      x: player.facing === 1 ? player.x + player.w : player.x - 10,
      y: player.y + 4,
      w: 10,
      h: 8,
    };
    for (const enemy of gameState.enemies) {
      if (
        hitBox.x < enemy.x + enemy.w &&
        hitBox.x + hitBox.w > enemy.x &&
        hitBox.y < enemy.y + enemy.h &&
        hitBox.y + hitBox.h > enemy.y
      ) {
        enemy.hp -= 1;
        enemy.stagger = 10;
      }
    }
  }

  if (player.invuln > 0) return;
  for (const enemy of gameState.enemies) {
    const overlaps =
      player.x < enemy.x + enemy.w &&
      player.x + player.w > enemy.x &&
      player.y < enemy.y + enemy.h &&
      player.y + player.h > enemy.y;
    if (overlaps) {
      player.life -= 1;
      player.invuln = 60;
      player.vy = -3;
      player.vx = player.x < enemy.x ? -2 : 2;
      updateHud();
      break;
    }
  }
}

function cleanupEnemies() {
  const alive = [];
  for (const enemy of gameState.enemies) {
    if (enemy.hp <= 0) {
      gameState.kills += 1;
    } else {
      alive.push(enemy);
    }
  }
  gameState.enemies = alive;
  updateHud();
}

function updateWaves() {
  if (gameState.enemies.length === 0 && gameState.waveRemaining <= 0) {
    if (gameState.wave >= 5) {
      gameState.win = true;
      gameState.gameOver = true;
    } else {
      setWave(gameState.wave + 1);
    }
  }
  if (gameState.waveRemaining > 0) {
    gameState.spawnTimer -= 1;
    if (gameState.spawnTimer <= 0) {
      spawnEnemy();
      gameState.waveRemaining -= 1;
      gameState.spawnTimer = Math.max(20, 50 - gameState.wave * 4);
    }
  }
}

function checkGameOver() {
  if (gameState.player.life <= 0) {
    gameState.gameOver = true;
    gameState.win = false;
  }
}

function update() {
  if (gameState.gameOver) return;
  updatePlayer();
  for (const enemy of gameState.enemies) {
    enemyLogic(enemy);
  }
  handleCombat();
  cleanupEnemies();
  updateWaves();
  checkGameOver();
}

function drawBackdrop() {
  ctx.fillStyle = palette.sky;
  ctx.fillRect(0, 0, WORLD.width, WORLD.height);
  ctx.fillStyle = palette.moon;
  ctx.fillRect(240, 18, 18, 18);
  ctx.fillStyle = "#1a1d2e";
  for (let i = 0; i < 20; i += 1) {
    ctx.fillRect((i * 17) % WORLD.width, 130 - (i % 3) * 12, 18, 40);
  }
}

function drawPlatforms() {
  ctx.fillStyle = palette.platform;
  for (const platform of gameState.platforms) {
    ctx.fillRect(platform.x, platform.y, platform.w, platform.h);
  }
}

function drawPlayer() {
  const { player } = gameState;
  const blink = player.invuln > 0 && Math.floor(player.invuln / 6) % 2 === 0;
  if (blink) return;
  ctx.fillStyle = palette.cloak;
  ctx.fillRect(player.x - 2, player.y + 4, player.w + 4, player.h - 4);
  ctx.fillStyle = palette.player;
  ctx.fillRect(player.x, player.y, player.w, player.h);
  ctx.fillStyle = palette.text;
  ctx.fillRect(player.x + 3, player.y + 4, 2, 2);
  if (player.attackTimer > 6) {
    ctx.fillStyle = palette.hit;
    const slashX = player.facing === 1 ? player.x + player.w : player.x - 10;
    ctx.fillRect(slashX, player.y + 6, 10, 6);
  }
}

function drawEnemies() {
  for (const enemy of gameState.enemies) {
    ctx.fillStyle = palette.enemy;
    ctx.fillRect(enemy.x, enemy.y, enemy.w, enemy.h);
    ctx.fillStyle = palette.enemyEye;
    ctx.fillRect(enemy.x + 3, enemy.y + 4, 2, 2);
    ctx.fillRect(enemy.x + 7, enemy.y + 4, 2, 2);
  }
}

function drawOverlay() {
  if (gameState.messageTimer > 0) {
    gameState.messageTimer -= 1;
    ctx.fillStyle = "rgba(0,0,0,0.6)";
    ctx.fillRect(40, 10, 240, 30);
    ctx.fillStyle = palette.text;
    ctx.font = "10px 'Press Start 2P', sans-serif";
    ctx.fillText(`Волна ${gameState.wave} начинается!`, 60, 30);
  }
  if (gameState.gameOver) {
    ctx.fillStyle = "rgba(0,0,0,0.75)";
    ctx.fillRect(40, 50, 240, 80);
    ctx.fillStyle = palette.text;
    ctx.font = "12px 'Press Start 2P', sans-serif";
    const text = gameState.win ? "Тьма отступает!" : "Охотник пал...";
    ctx.fillText(text, 70, 90);
    ctx.font = "10px 'Press Start 2P', sans-serif";
    ctx.fillText("Нажми R чтобы снова", 60, 110);
  }
}

function render() {
  drawBackdrop();
  drawPlatforms();
  drawEnemies();
  drawPlayer();
  drawOverlay();
}

function loop() {
  update();
  render();
  requestAnimationFrame(loop);
}

window.addEventListener("keydown", (event) => {
  keys.add(event.key.toLowerCase());
  if (event.key.toLowerCase() === "r") {
    resetGame();
  }
});

window.addEventListener("keyup", (event) => {
  keys.delete(event.key.toLowerCase());
});

resetGame();
loop();
