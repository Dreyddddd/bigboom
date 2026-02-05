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

        [Header("Shot control")]
        [SerializeField] private bool supportsCharge;
        [SerializeField, Min(0.1f)] private float minPowerMultiplier = 0.5f;
        [SerializeField, Min(0.1f)] private float maxPowerMultiplier = 1.8f;
        [SerializeField, Min(0.05f)] private float maxChargeSeconds = 1.25f;

        public WeaponType WeaponType => weaponType;
        public string DisplayName => displayName;
        public string FeatureDescription => featureDescription;
        public float ExplosionRadius => explosionRadius;
        public float Damage => damage;
        public float ProjectileSpeed => projectileSpeed;
        public bool TerrainPiercing => terrainPiercing;
        public bool SupportsCharge => supportsCharge;
        public float MinPowerMultiplier => minPowerMultiplier;
        public float MaxPowerMultiplier => maxPowerMultiplier;
        public float MaxChargeSeconds => maxChargeSeconds;

        public void Configure(
            WeaponType type,
            string name,
            string feature,
            float radius,
            float valueDamage,
            float speed,
            bool piercing,
            bool chargeSupported,
            float minPower,
            float maxPower,
            float chargeSeconds)
        {
            weaponType = type;
            displayName = name;
            featureDescription = feature;
            explosionRadius = radius;
            damage = valueDamage;
            projectileSpeed = speed;
            terrainPiercing = piercing;
            supportsCharge = chargeSupported;
            minPowerMultiplier = minPower;
            maxPowerMultiplier = maxPower;
            maxChargeSeconds = chargeSeconds;
        }

        public float EvaluatePowerMultiplier(float chargeSecondsHeld)
        {
            if (!supportsCharge)
            {
                return 1f;
            }

            var t = Mathf.Clamp01(chargeSecondsHeld / Mathf.Max(0.05f, maxChargeSeconds));
            return Mathf.Lerp(minPowerMultiplier, maxPowerMultiplier, t);
        }
    }
}
