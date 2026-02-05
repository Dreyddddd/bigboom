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
            weaponDefinitions.Add(CreateRuntime("Banana Bomb", WeaponType.BananaBomb, "Отскакивает 3 раза и взрывается каскадом.", 1.8f, 55f, 9f, false, true, 0.35f, 1.8f, 1.4f));
            weaponDefinitions.Add(CreateRuntime("Drill Rocket", WeaponType.DrillRocket, "Бурит породу до детонации.", 2.2f, 70f, 12f, true, true, 0.6f, 2f, 1.6f));
            weaponDefinitions.Add(CreateRuntime("Gravity Grenade", WeaponType.GravityGrenade, "Подтягивает врагов и обломки к центру.", 2.5f, 35f, 7f, false, true, 0.5f, 1.7f, 1.4f));
            weaponDefinitions.Add(CreateRuntime("Plasma Disc", WeaponType.PlasmaDisc, "Рикошетит от стен и оставляет прожоги.", 1.2f, 42f, 16f, false, false, 1f, 1f, 0.1f));
            weaponDefinitions.Add(CreateRuntime("Cluster Beehive", WeaponType.ClusterBeehive, "Распадается на мини-пчёл-бомб.", 1.4f, 25f, 10f, false, true, 0.5f, 1.9f, 1.2f));
            weaponDefinitions.Add(CreateRuntime("Fire Rain Mortar", WeaponType.FireRainMortar, "Вызывает дождь из зажигательных снарядов.", 1.1f, 18f, 8f, false, true, 0.45f, 1.7f, 1.5f));
            weaponDefinitions.Add(CreateRuntime("Cryo Cannon", WeaponType.CryoCannon, "Замедляет цель и хрупко крошит грунт.", 1.6f, 33f, 11f, false, false, 1f, 1f, 0.1f));
            weaponDefinitions.Add(CreateRuntime("Sonic Boomer", WeaponType.SonicBoomer, "Ударная волна отбрасывает юнитов.", 2.8f, 30f, 9f, false, false, 1f, 1f, 0.1f));
            weaponDefinitions.Add(CreateRuntime("Shredder Laser", WeaponType.ShredderLaser, "Режет тонкий тоннель сквозь землю.", 0.9f, 48f, 22f, true, false, 1f, 1f, 0.1f));
            weaponDefinitions.Add(CreateRuntime("Slime Launcher", WeaponType.SlimeLauncher, "Липкая слизь ограничивает передвижение.", 1.3f, 20f, 8f, false, true, 0.4f, 1.5f, 1.3f));
            weaponDefinitions.Add(CreateRuntime("Thunder Javelin", WeaponType.ThunderJavelin, "Молния бьёт цепочкой по ближайшим врагам.", 1.7f, 45f, 15f, false, false, 1f, 1f, 0.1f));
            weaponDefinitions.Add(CreateRuntime("Wormhole Gun", WeaponType.WormholeGun, "Телепорт-выстрел меняет позиции стрелка и цели.", 0.8f, 22f, 14f, true, false, 1f, 1f, 0.1f));
            weaponDefinitions.Add(CreateRuntime("Junkyard Mine", WeaponType.JunkyardMine, "Мина на поверхности с задержанным подрывом.", 1.9f, 60f, 4f, false, true, 0.35f, 1.2f, 1.0f));
            weaponDefinitions.Add(CreateRuntime("Meteor Caller", WeaponType.MeteorCaller, "Через секунду вызывает метеор сверху.", 2.4f, 68f, 6f, false, true, 0.6f, 1.9f, 1.7f));
            weaponDefinitions.Add(CreateRuntime("Mini Nuke", WeaponType.NukeMini, "Маленький ядерный заряд с огромной воронкой.", 3.4f, 95f, 9f, false, true, 0.55f, 2.1f, 1.8f));
            RebuildIndex();
        }

        private WeaponDefinition CreateRuntime(string nameRu, WeaponType type, string feature, float radius, float damage, float speed, bool piercing, bool chargeSupported, float minPower, float maxPower, float maxChargeSeconds)
        {
            var definition = ScriptableObject.CreateInstance<WeaponDefinition>();
            definition.Configure(type, nameRu, feature, radius, damage, speed, piercing, chargeSupported, minPower, maxPower, maxChargeSeconds);
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
