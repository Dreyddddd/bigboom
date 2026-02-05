using BigBoom.Gameplay.Players;
using BigBoom.Gameplay.Terrain;
using UnityEngine;

namespace BigBoom.Gameplay.Combat
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private LayerMask hittableMask;

        private WeaponDefinition _weapon;
        private PlayerController _owner;
        private DestructibleTerrain _terrain;
        private Rigidbody2D _rigidbody;
        private float _shotPowerMultiplier = 1f;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        public void Launch(WeaponDefinition weapon, Vector2 direction, float shotPowerMultiplier, PlayerController owner, DestructibleTerrain terrain)
        {
            _weapon = weapon;
            _owner = owner;
            _terrain = terrain;
            _shotPowerMultiplier = Mathf.Max(0.1f, shotPowerMultiplier);
            _rigidbody.velocity = direction.normalized * weapon.ProjectileSpeed * _shotPowerMultiplier;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (((1 << collision.gameObject.layer) & hittableMask) == 0)
            {
                return;
            }

            Explode(collision.GetContact(0).point);
        }

        private void Explode(Vector2 point)
        {
            var explosionRadius = _weapon.ExplosionRadius * Mathf.Lerp(0.9f, 1.35f, Mathf.InverseLerp(0.2f, 2.2f, _shotPowerMultiplier));
            _terrain.CarveCircle(point, explosionRadius);

            var hits = Physics2D.OverlapCircleAll(point, explosionRadius, LayerMask.GetMask("Player"));
            foreach (var hit in hits)
            {
                if (!hit.TryGetComponent<PlayerController>(out var player))
                {
                    continue;
                }

                if (player == _owner)
                {
                    continue;
                }

                player.ApplyDamage(_weapon.Damage * _shotPowerMultiplier, _owner);
            }

            Destroy(gameObject);
        }
    }
}
