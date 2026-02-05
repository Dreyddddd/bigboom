# BigBoom (Unity 2D Side-View Prototype)

Прототип одиночной 2D-игры в стиле Worms/Terraria с разрушаемым ландшафтом, ботами, раундами, дропами оружия и лидербордом.

## Что реализовано

- **Одиночный режим Deathmatch** (FFA).
- **Настройка сессии**: число ботов и число раундов.
- **Раундовая система**:
  - старт раунда,
  - процедурная генерация ландшафта,
  - спавн ботов,
  - дропы оружия,
  - подсчет побед и фрагов,
  - итоговый лидерборд.
- **Разрушаемый terrain**:
  - генерация карты через шум,
  - карвинг отверстий от взрывов.
- **15 типов оружия** с разными фишками (база для расширения через ScriptableObject).
- **Боты с базовой тактикой**:
  - выбор цели,
  - дистанционирование,
  - проверка линии огня,
  - антизастревание.

## Структура проекта

```text
Assets/Scripts/
  Core/                # Bootstrap, game settings, mode/session
  Gameplay/
    Rounds/            # Match/round lifecycle
    Terrain/           # Procedural + destructible map
    Combat/            # Weapons, projectiles, airdrop crates
    Players/           # Player controller, inventory, factory
    AI/                # Bot brain/tactics
    UI/                # Lobby config and leaderboard
Docs/
  ARCHITECTURE.md      # Детали архитектуры и расширения
```

## Быстрый старт в Unity

1. Создайте новый 2D проект в Unity (рекомендуется 2021.3+).
2. Скопируйте папки `Assets/Scripts` и `Docs` в проект.
3. Создайте сцену `Main` и объекты:
   - `GameBootstrap`
   - `RoundManager`
   - `TerrainGenerator` + `DestructibleTerrain`
   - `WeaponDatabase`
   - `WeaponDropManager`
   - `LeaderboardPresenter`
   - `PlayerFactory`
4. Свяжите ссылки в инспекторе (`SerializeField`).
5. Установите layer `Player` и назначьте его игрокам/ботам.
6. Для `WeaponDatabase` вызовите context menu **Populate 15 Starter Weapons**.
7. Запустите сцену.

## Дальше можно добавить

- Пошаговые ходы (как в Worms), ветер, баллистику.
- Полноценную пересборку коллайдера terrain по альфа-контру.
- Улучшенную навигацию AI (граф поверхности, прыжки/тросы).
- Графику, VFX, SFX, анимации, UI/UX-полировку.
- Новые режимы (Teams, King of the Hill, Capture).
