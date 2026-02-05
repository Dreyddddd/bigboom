# Архитектура BigBoom

## 1) Core layer

- `GameBootstrap` — точка входа, инициализация runtime-конфига, запуск матча.
- `GameSettings` — параметры сессии (раунды, боты, режим).
- `SessionConfig` — runtime DTO текущей игры.
- `GameModeType` — enum режимов (сейчас `Deathmatch`).

## 2) Match / Round flow

`RoundManager` управляет циклом раундов:
1. Очистка предыдущего раунда.
2. Генерация terrain.
3. Спавн **human player + N bots**.
4. Запуск airdrop-оружия.
5. Ожидание победителя в real-time.
6. Обновление фрагов и лидерборда.

## 3) Player control (real-time)

`PlayerController` поддерживает единый API управления как для человека, так и для AI:
- `SetMoveInput`, `RequestJump`, `RequestFastFall`.
- `SetAimTarget` (наведение в world-space).
- `BeginFireCharge` / `ReleaseFire` (charge shot).

Human input (`WASD + мышь + ЛКМ`) обрабатывается внутри `PlayerController`,
если `IsHumanControlled = true`.

## 4) Terrain subsystem

- `TerrainGenerator` создает карту на основе Perlin noise.
- `DestructibleTerrain` хранит runtime texture и вырезает отверстия (`CarveCircle`) при взрывах.

> Сейчас collider обновляется упрощенно; для продакшна нужен rebuild контура
> (marching squares / outline tracing) после каждой деформации.

## 5) Combat subsystem

- `WeaponDefinition` (SO): характеристики оружия + параметры charge shot.
- `WeaponDatabase`: реестр оружия и стартовый набор из 15 единиц.
- `Projectile`: полет, столкновения, урон и деформация terrain с учетом силы выстрела.
- `WeaponDropManager` / `WeaponCrate`: airdrop и pickup оружия.

## 6) AI subsystem

`BotBrain` использует те же команды, что и игрок:
- выбирает тактическую цель,
- держит комфортную дистанцию,
- проверяет line-of-fire,
- заряжает и отпускает выстрел в зависимости от дистанции,
- прыгает при препятствиях и антизастревает.

## 7) UI

- `LobbySettingsPresenter` — настройка ботов/раундов.
- `LeaderboardPresenter` — live/final таблица фрагов.

## 8) Расширяемость

- Новые режимы: расширить `GameModeType` + отдельные rule handlers.
- Новое оружие: новые `WeaponDefinition` + спец-эффекты.
- Мультиплеер: выделить simulation слой и перейти на deterministic tick.
