using UnityEngine;

namespace BigBoom.Gameplay.Combat
{
    [CreateAssetMenu(menuName = "BigBoom/Combat/Weapon Definition", fileName = "WeaponDefinition")]
    public class WeaponDefinition : ScriptableObject
    {
        [SerializeField] private WeaponType weaponType;
        [SerializeField] private string displayName;
        [SerializeField, TextArea] private string featureDescription;
        [SerializeField] private float explosionRadius;
        [SerializeField] private float damage;
        [SerializeField] private float projectileSpeed;
        [SerializeField] private bool terrainPiercing;

        public WeaponType WeaponType => weaponType;
        public string DisplayName => displayName;
        public string FeatureDescription => featureDescription;
        public float ExplosionRadius => explosionRadius;
        public float Damage => damage;
        public float ProjectileSpeed => projectileSpeed;
        public bool TerrainPiercing => terrainPiercing;

        public void Configure(WeaponType type, string name, string feature, float radius, float valueDamage, float speed, bool piercing)
        {
            weaponType = type;
            displayName = name;
            featureDescription = feature;
            explosionRadius = radius;
            damage = valueDamage;
            projectileSpeed = speed;
            terrainPiercing = piercing;
        }
    }
}
