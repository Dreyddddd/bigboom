# Архитектура BigBoom

## 1) Core layer

- `GameBootstrap` — точка входа, инициализирует зависимости, стартует матч.
- `GameSettings` — глобальные настройки сессии и режима.
- `SessionConfig` — runtime-конфиг текущего матча.
- `GameModeType` — enum игровых режимов (сейчас только `Deathmatch`).

## 2) Match/Round flow

`RoundManager` управляет циклом:
1. Очистка прошлого раунда.
2. Генерация terrain.
3. Спавн ботов.
4. Запуск дропов оружия.
5. Ожидание победителя раунда.
6. Начисление очков, обновление лидерборда.

## 3) Terrain subsystem

- `TerrainGenerator` генерирует карту через Perlin noise.
- `DestructibleTerrain` хранит runtime texture и вырезает "дырки" (`CarveCircle`).

### Важно
Сейчас коллайдер terrain обновляется упрощенно.
Для продакшн-качества нужно добавить contour extraction (marching squares)
и rebuild `PolygonCollider2D` после деформации.

## 4) Combat subsystem

- `WeaponDefinition` (SO) — параметры оружия.
- `WeaponDatabase` — реестр оружия, случайный выбор, seed-набор из 15 видов.
- `Projectile` — полет, столкновения, урон, разрушение terrain.
- `WeaponDropManager` — периодический air-drop.
- `WeaponCrate` — коробка с подбираемым оружием.

## 5) Players + AI

- `PlayerController` — HP, движение, стрельба, смерть, события фрагов.
- `WeaponInventory` — очередь вооружения (pickup/use).
- `PlayerFactory` — спавн ботов и привязка AI.
- `BotBrain`:
  - тактический выбор ближайшей цели,
  - удержание дистанции,
  - проверка линии огня,
  - anti-stuck логика (если бот не сдвигается).

## 6) UI

- `LobbySettingsPresenter` — выбор ботов/раундов перед стартом.
- `LeaderboardPresenter` — live и final таблица фрагов.

## 7) Расширяемость

- Новые режимы: добавить в `GameModeType` + отдельный rule handler.
- Новое оружие: новые SO + при необходимости особый `WeaponEffect` слой.
- Более умный AI: utility scoring, оценка укрытий, обход воронок, прыжки.
- Мультиплеер: отделить simulation state от view и перейти к deterministic tick.
