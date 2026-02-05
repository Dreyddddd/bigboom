using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BigBoom.Gameplay.Combat
{
    public class WeaponDatabase : MonoBehaviour
    {
        [SerializeField] private List<WeaponDefinition> weaponDefinitions = new();

        private readonly Dictionary<WeaponType, WeaponDefinition> _index = new();

        public IReadOnlyList<WeaponDefinition> Weapons => weaponDefinitions;

        private void Awake()
        {
            RebuildIndex();
        }

        public WeaponDefinition Get(WeaponType weaponType)
        {
            if (_index.TryGetValue(weaponType, out var definition))
            {
                return definition;
            }

            throw new ArgumentOutOfRangeException(nameof(weaponType), $"Weapon type '{weaponType}' is not present in WeaponDatabase.");
        }

        public WeaponDefinition GetRandom()
        {
            if (weaponDefinitions.Count == 0)
            {
                throw new InvalidOperationException("WeaponDatabase has no weapon definitions.");
            }

            return weaponDefinitions[Random.Range(0, weaponDefinitions.Count)];
        }

        [ContextMenu("Populate 15 Starter Weapons")]
        public void PopulateDefaultSet()
        {
            weaponDefinitions.Clear();
            weaponDefinitions.Add(CreateRuntime("Banana Bomb", WeaponType.BananaBomb, "Отскакивает 3 раза и взрывается каскадом.", 1.8f, 55f, 9f, false));
            weaponDefinitions.Add(CreateRuntime("Drill Rocket", WeaponType.DrillRocket, "Бурит породу до детонации.", 2.2f, 70f, 12f, true));
            weaponDefinitions.Add(CreateRuntime("Gravity Grenade", WeaponType.GravityGrenade, "Подтягивает врагов и обломки к центру.", 2.5f, 35f, 7f, false));
            weaponDefinitions.Add(CreateRuntime("Plasma Disc", WeaponType.PlasmaDisc, "Рикошетит от стен и оставляет прожоги.", 1.2f, 42f, 16f, false));
            weaponDefinitions.Add(CreateRuntime("Cluster Beehive", WeaponType.ClusterBeehive, "Распадается на мини-пчёл-бомб.", 1.4f, 25f, 10f, false));
            weaponDefinitions.Add(CreateRuntime("Fire Rain Mortar", WeaponType.FireRainMortar, "Вызывает дождь из зажигательных снарядов.", 1.1f, 18f, 8f, false));
            weaponDefinitions.Add(CreateRuntime("Cryo Cannon", WeaponType.CryoCannon, "Замедляет цель и хрупко крошит грунт.", 1.6f, 33f, 11f, false));
            weaponDefinitions.Add(CreateRuntime("Sonic Boomer", WeaponType.SonicBoomer, "Ударная волна отбрасывает юнитов.", 2.8f, 30f, 9f, false));
            weaponDefinitions.Add(CreateRuntime("Shredder Laser", WeaponType.ShredderLaser, "Режет тонкий тоннель сквозь землю.", 0.9f, 48f, 22f, true));
            weaponDefinitions.Add(CreateRuntime("Slime Launcher", WeaponType.SlimeLauncher, "Липкая слизь ограничивает передвижение.", 1.3f, 20f, 8f, false));
            weaponDefinitions.Add(CreateRuntime("Thunder Javelin", WeaponType.ThunderJavelin, "Молния бьёт цепочкой по ближайшим врагам.", 1.7f, 45f, 15f, false));
            weaponDefinitions.Add(CreateRuntime("Wormhole Gun", WeaponType.WormholeGun, "Телепорт-выстрел меняет позиции стрелка и цели.", 0.8f, 22f, 14f, true));
            weaponDefinitions.Add(CreateRuntime("Junkyard Mine", WeaponType.JunkyardMine, "Мина на поверхности с задержанным подрывом.", 1.9f, 60f, 4f, false));
            weaponDefinitions.Add(CreateRuntime("Meteor Caller", WeaponType.MeteorCaller, "Через секунду вызывает метеор сверху.", 2.4f, 68f, 6f, false));
            weaponDefinitions.Add(CreateRuntime("Mini Nuke", WeaponType.NukeMini, "Маленький ядерный заряд с огромной воронкой.", 3.4f, 95f, 9f, false));
            RebuildIndex();
        }

        private WeaponDefinition CreateRuntime(string nameRu, WeaponType type, string feature, float radius, float damage, float speed, bool piercing)
        {
            var definition = ScriptableObject.CreateInstance<WeaponDefinition>();
            definition.Configure(type, nameRu, feature, radius, damage, speed, piercing);
            return definition;
        }

        private void RebuildIndex()
        {
            _index.Clear();
            foreach (var weaponDefinition in weaponDefinitions)
            {
                if (weaponDefinition == null)
                {
                    continue;
                }

                _index[weaponDefinition.WeaponType] = weaponDefinition;
            }
        }
    }
}
