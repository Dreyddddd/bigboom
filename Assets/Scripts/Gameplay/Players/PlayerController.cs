using System;
using BigBoom.Gameplay.Combat;
using BigBoom.Gameplay.Terrain;
using UnityEngine;

namespace BigBoom.Gameplay.Players
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private Transform firePoint;
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private WeaponDefinition defaultWeapon;

        private float _health;
        private Rigidbody2D _rigidbody;
        private DestructibleTerrain _terrain;

        public event Action<PlayerController, PlayerController> KilledEnemy;
        public event Action<PlayerController> Died;

        public string PlayerId { get; private set; }
        public bool IsAlive => _health > 0f;
        public WeaponInventory Inventory { get; } = new();

        private void Awake()
        {
            _health = maxHealth;
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        public void Setup(string playerId, DestructibleTerrain terrain)
        {
            PlayerId = playerId;
            _terrain = terrain;
            gameObject.name = playerId;
        }

        public void Move(float direction)
        {
            _rigidbody.velocity = new Vector2(direction * moveSpeed, _rigidbody.velocity.y);
        }

        public void ShootAt(Vector2 worldTarget)
        {
            var weapon = Inventory.PullNextOrDefault(defaultWeapon);
            var origin = firePoint != null ? (Vector2)firePoint.position : (Vector2)transform.position;
            var projectile = Instantiate(projectilePrefab, origin, Quaternion.identity);
            var direction = (worldTarget - origin).normalized;
            projectile.Launch(weapon, direction, this, _terrain);
        }

        public void ApplyDamage(float damage, PlayerController attacker)
        {
            if (!IsAlive)
            {
                return;
            }

            _health -= damage;
            if (_health > 0f)
            {
                return;
            }

            _health = 0f;
            Died?.Invoke(this);
            if (attacker != null)
            {
                attacker.KilledEnemy?.Invoke(attacker, this);
            }

            gameObject.SetActive(false);
        }
    }
}
