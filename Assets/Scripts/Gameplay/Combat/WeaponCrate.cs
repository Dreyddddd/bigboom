using BigBoom.Gameplay.Players;
using UnityEngine;

namespace BigBoom.Gameplay.Combat
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class WeaponCrate : MonoBehaviour
    {
        [SerializeField] private float pickupRadius = 1f;

        private WeaponDefinition _weaponDefinition;
        private Vector2 _targetPosition;
        private Rigidbody2D _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        public void Initialize(WeaponDefinition weaponDefinition, Vector2 targetPosition)
        {
            _weaponDefinition = weaponDefinition;
            _targetPosition = targetPosition;
            _rigidbody.velocity = Vector2.down * 5f;
        }

        private void Update()
        {
            if (Vector2.Distance(transform.position, _targetPosition) <= 0.3f)
            {
                _rigidbody.velocity = Vector2.zero;
            }

            var hit = Physics2D.OverlapCircle(transform.position, pickupRadius, LayerMask.GetMask("Player"));
            if (hit == null)
            {
                return;
            }

            if (!hit.TryGetComponent<PlayerController>(out var player))
            {
                return;
            }

            player.Inventory.AddWeapon(_weaponDefinition);
            Destroy(gameObject);
        }
    }
}
