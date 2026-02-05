using System.Collections.Generic;
using System.Linq;
using BigBoom.Gameplay.Players;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BigBoom.Gameplay.AI
{
    public class BotBrain : MonoBehaviour
    {
        [SerializeField] private float thinkInterval = 0.25f;
        [SerializeField] private float maxFireDistance = 16f;
        [SerializeField] private LayerMask terrainMask;
        [SerializeField] private float jumpDecisionCooldown = 0.9f;

        private readonly List<Vector2> _recentPositions = new();

        private PlayerController _owner;
        private float _nextThinkTime;
        private float _nextJumpTime;
        private float _stuckTimer;
        private float _strafeInput;

        private bool _chargingShot;
        private float _chargeEndTime;

        public void Initialize(PlayerController owner)
        {
            _owner = owner;
            _nextThinkTime = Time.time + Random.Range(0f, thinkInterval);
        }

        private void Update()
        {
            if (_owner == null || !_owner.IsAlive)
            {
                return;
            }

            if (Time.time >= _nextThinkTime)
            {
                Think();
                _nextThinkTime = Time.time + thinkInterval;
            }

            _owner.SetMoveInput(_strafeInput);
            TrackStuckState();

            if (_chargingShot && Time.time >= _chargeEndTime)
            {
                _owner.ReleaseFire();
                _chargingShot = false;
            }
        }

        private void Think()
        {
            var targets = FindObjectsOfType<PlayerController>()
                .Where(candidate => candidate != _owner && candidate.IsAlive)
                .ToArray();

            if (targets.Length == 0)
            {
                _strafeInput = 0f;
                return;
            }

            var target = SelectTacticalTarget(targets);
            var targetPos = (Vector2)target.transform.position;
            var ownerPos = (Vector2)_owner.transform.position;
            var toTarget = targetPos - ownerPos;
            var distance = toTarget.magnitude;

            _owner.SetAimTarget(targetPos + Random.insideUnitCircle * 0.65f);
            _strafeInput = ComputeStrafeInput(toTarget, distance);
            TryJumpObstacle(toTarget);
            TryShoot(targetPos, distance);
        }

        private PlayerController SelectTacticalTarget(PlayerController[] targets)
        {
            return targets
                .OrderBy(target => Vector2.Distance(target.transform.position, _owner.transform.position))
                .ThenByDescending(target => target.Inventory.Count)
                .First();
        }

        private float ComputeStrafeInput(Vector2 toTarget, float distance)
        {
            if (_stuckTimer > 0.9f)
            {
                return Mathf.Sign(Random.Range(-1f, 1f));
            }

            if (distance < 4.5f)
            {
                return -Mathf.Sign(toTarget.x);
            }

            if (distance > 10f)
            {
                return Mathf.Sign(toTarget.x);
            }

            return 0f;
        }

        private void TryJumpObstacle(Vector2 toTarget)
        {
            if (Time.time < _nextJumpTime)
            {
                return;
            }

            var origin = (Vector2)_owner.transform.position + Vector2.up * 0.25f;
            var direction = new Vector2(Mathf.Sign(toTarget.x), 0f);
            if (direction == Vector2.zero)
            {
                direction = Vector2.right;
            }

            var blockedAhead = Physics2D.Raycast(origin, direction, 0.9f, terrainMask);
            if (!blockedAhead)
            {
                return;
            }

            _owner.RequestJump();
            _nextJumpTime = Time.time + jumpDecisionCooldown;
        }

        private void TryShoot(Vector2 targetPos, float distance)
        {
            if (_chargingShot)
            {
                return;
            }

            if (distance > maxFireDistance)
            {
                return;
            }

            var blocked = Physics2D.Linecast(_owner.transform.position, targetPos, terrainMask);
            if (blocked)
            {
                return;
            }

            _owner.BeginFireCharge();

            var desiredCharge = Mathf.Clamp(distance / maxFireDistance, 0.15f, 0.95f);
            _chargeEndTime = Time.time + Mathf.Lerp(0.1f, 1.2f, desiredCharge) + Random.Range(0f, 0.2f);
            _chargingShot = true;
        }

        private void TrackStuckState()
        {
            _recentPositions.Add(_owner.transform.position);
            if (_recentPositions.Count > 8)
            {
                _recentPositions.RemoveAt(0);
            }

            if (_recentPositions.Count < 8)
            {
                return;
            }

            var drift = Vector2.Distance(_recentPositions[0], _recentPositions[_recentPositions.Count - 1]);
            _stuckTimer = drift < 0.18f ? _stuckTimer + Time.deltaTime : 0f;
        }
    }
}
