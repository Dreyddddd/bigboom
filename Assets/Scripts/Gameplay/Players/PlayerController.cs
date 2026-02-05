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
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float jumpForce = 6f;
        [SerializeField] private float fallBoost = 1.5f;
        [SerializeField] private float groundCheckRadius = 0.2f;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private Transform firePoint;
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private WeaponDefinition defaultWeapon;

        private float _health;
        private Rigidbody2D _rigidbody;
        private DestructibleTerrain _terrain;
        private Camera _cachedCamera;

        private Vector2 _aimWorldTarget;
        private float _moveInput;
        private bool _jumpRequested;
        private bool _fallRequested;
        private bool _isFireCharging;
        private float _fireChargeStartTime;
        private WeaponDefinition _preparedWeapon;

        public event Action<PlayerController, PlayerController> KilledEnemy;
        public event Action<PlayerController> Died;

        public string PlayerId { get; private set; }
        public bool IsAlive => _health > 0f;
        public bool IsHumanControlled { get; private set; }
        public WeaponInventory Inventory { get; } = new();

        private void Awake()
        {
            _health = maxHealth;
            _rigidbody = GetComponent<Rigidbody2D>();
            _cachedCamera = Camera.main;
            _aimWorldTarget = transform.position + Vector3.right;
        }

        private void Update()
        {
            if (!IsAlive || !IsHumanControlled)
            {
                return;
            }

            HandleHumanInput();
        }

        private void FixedUpdate()
        {
            if (!IsAlive)
            {
                return;
            }

            ApplyMovement();
        }

        public void Setup(string playerId, DestructibleTerrain terrain, bool humanControlled)
        {
            PlayerId = playerId;
            _terrain = terrain;
            IsHumanControlled = humanControlled;
            gameObject.name = playerId;
        }

        public void SetMoveInput(float moveInput)
        {
            _moveInput = Mathf.Clamp(moveInput, -1f, 1f);
        }

        public void RequestJump()
        {
            _jumpRequested = true;
        }

        public void RequestFastFall(bool enabled)
        {
            _fallRequested = enabled;
        }

        public void SetAimTarget(Vector2 worldTarget)
        {
            _aimWorldTarget = worldTarget;
            RotateToAim();
        }

        public void BeginFireCharge()
        {
            if (!IsAlive || _isFireCharging)
            {
                return;
            }

            _preparedWeapon = Inventory.PullNextOrDefault(defaultWeapon);
            if (_preparedWeapon == null)
            {
                return;
            }

            _isFireCharging = true;
            _fireChargeStartTime = Time.time;

            if (!_preparedWeapon.SupportsCharge)
            {
                ReleaseFire();
            }
        }

        public void ReleaseFire()
        {
            if (!IsAlive || !_isFireCharging || _preparedWeapon == null)
            {
                return;
            }

            var heldSeconds = Time.time - _fireChargeStartTime;
            var powerMultiplier = _preparedWeapon.EvaluatePowerMultiplier(heldSeconds);

            var origin = firePoint != null ? (Vector2)firePoint.position : (Vector2)transform.position;
            var projectile = Instantiate(projectilePrefab, origin, Quaternion.identity);
            var direction = (_aimWorldTarget - origin).normalized;
            projectile.Launch(_preparedWeapon, direction, powerMultiplier, this, _terrain);

            _isFireCharging = false;
            _preparedWeapon = null;
        }

        public void CancelFireCharge()
        {
            _isFireCharging = false;
            if (_preparedWeapon != null)
            {
                Inventory.AddWeapon(_preparedWeapon);
                _preparedWeapon = null;
            }
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

        private void HandleHumanInput()
        {
            var horizontal = 0f;
            if (Input.GetKey(KeyCode.A))
            {
                horizontal -= 1f;
            }

            if (Input.GetKey(KeyCode.D))
            {
                horizontal += 1f;
            }

            SetMoveInput(horizontal);

            if (Input.GetKeyDown(KeyCode.W))
            {
                RequestJump();
            }

            RequestFastFall(Input.GetKey(KeyCode.S));

            if (_cachedCamera == null)
            {
                _cachedCamera = Camera.main;
            }

            if (_cachedCamera != null)
            {
                var mousePosition = _cachedCamera.ScreenToWorldPoint(Input.mousePosition);
                SetAimTarget(new Vector2(mousePosition.x, mousePosition.y));
            }

            if (Input.GetMouseButtonDown(0))
            {
                BeginFireCharge();
            }

            if (Input.GetMouseButtonUp(0))
            {
                ReleaseFire();
            }
        }

        private void ApplyMovement()
        {
            _rigidbody.velocity = new Vector2(_moveInput * moveSpeed, _rigidbody.velocity.y);

            if (_jumpRequested && IsGrounded())
            {
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, jumpForce);
            }

            _jumpRequested = false;

            if (_fallRequested && _rigidbody.velocity.y > -10f)
            {
                _rigidbody.velocity += Vector2.down * fallBoost * Time.fixedDeltaTime;
            }
        }

        private bool IsGrounded()
        {
            var point = groundCheck != null ? groundCheck.position : transform.position;
            return Physics2D.OverlapCircle(point, groundCheckRadius, groundMask) != null;
        }

        private void RotateToAim()
        {
            var pivot = firePoint != null ? firePoint : transform;
            var direction = _aimWorldTarget - (Vector2)pivot.position;
            if (direction.sqrMagnitude < 0.0001f)
            {
                return;
            }

            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            pivot.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}
