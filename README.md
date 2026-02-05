# BigBoom (Unity 2D Side-View Prototype)

Прототип одиночной 2D-игры в стиле Worms/Terraria с **real-time** боем, разрушаемым ландшафтом, ботами, раундами, дропами оружия и лидербордом.

## Что реализовано

- **Одиночный режим Deathmatch** (каждый сам за себя).
- **Настройка сессии**: число ботов и число раундов.
- **Раундовая система**:
  - генерация terrain в начале каждого раунда,
  - спавн игрока и ботов,
  - случайные аирдропы оружия,
  - live/final leaderboard по фрагам.
- **Real-time управление игроком**:
  - `A/D` — движение,
  - `W` — прыжок,
  - `S` — ускоренное падение,
  - наведение оружия мышью,
  - `ЛКМ` — выстрел, удержание регулирует силу для charge-оружия.
- **Разрушаемый terrain**:
  - процедурная генерация через Perlin noise,
  - карвинг отверстий от взрывов.
- **15 типов оружия** с разными фишками (через `WeaponDefinition`).
- **Боты с тактикой в real-time**:
  - выбор цели,
  - дистанционирование,
  - line-of-fire check,
  - зарядка/отпуск выстрела,
  - anti-stuck + прыжки через препятствия.

## Структура проекта

```text
Assets/Scripts/
  Core/                # Bootstrap, game settings, mode/session
  Gameplay/
    Rounds/            # Match/round lifecycle
    Terrain/           # Procedural + destructible map
    Combat/            # Weapons, projectiles, airdrop crates
    Players/           # Human/bot player controller, inventory, factory
    AI/                # Bot brain/tactics
    UI/                # Lobby config and leaderboard
Docs/
  ARCHITECTURE.md      # Детали архитектуры и расширения
```

## Быстрый старт в Unity

1. Создайте новый 2D проект в Unity (рекомендуется 2021.3+).
2. Скопируйте `Assets/Scripts` и `Docs` в проект.
3. В сцене создайте и свяжите через Inspector:
   - `GameBootstrap`
   - `RoundManager`
   - `TerrainGenerator` + `DestructibleTerrain`
   - `WeaponDatabase`
   - `WeaponDropManager`
   - `LeaderboardPresenter`
   - `PlayerFactory`
4. Назначьте слой `Player` объектам игрока/ботов.
5. Для `WeaponDatabase` вызовите **Populate 15 Starter Weapons**.
6. Убедитесь, что у игрока назначены `groundMask`, `groundCheck`, `firePoint`.
7. Запустите сцену.

## Дальнейшие улучшения

- Пересборка `PolygonCollider2D` по реальному контуру после деформации terrain.
- Более точная баллистика и уникальные эффекты для каждого оружия.
- Продвинутый AI (оценка укрытий, пути обхода, прыжковые маршруты).
- Добавление командных режимов и сетевого мультиплеера.
