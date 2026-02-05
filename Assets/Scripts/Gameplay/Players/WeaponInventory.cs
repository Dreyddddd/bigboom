using System.Collections.Generic;
using BigBoom.Gameplay.Combat;

namespace BigBoom.Gameplay.Players
{
    public class WeaponInventory
    {
        private readonly Queue<WeaponDefinition> _weapons = new();

        public int Count => _weapons.Count;

        public void AddWeapon(WeaponDefinition weapon)
        {
            if (weapon == null)
            {
                return;
            }

            _weapons.Enqueue(weapon);
        }

        public WeaponDefinition PullNextOrDefault(WeaponDefinition fallback)
        {
            if (_weapons.Count == 0)
            {
                return fallback;
            }

            return _weapons.Dequeue();
        }
    }
}
