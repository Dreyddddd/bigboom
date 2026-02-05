using System.Collections.Generic;
using System.Linq;
using BigBoom.Gameplay.Players;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BigBoom.Gameplay.AI
{
    public class BotBrain : MonoBehaviour
    {
        [SerializeField] private float thinkInterval = 0.4f;
        [SerializeField] private float maxFireDistance = 14f;
        [SerializeField] private LayerMask terrainMask;

        private readonly List<Vector2> _recentPositions = new();

        private PlayerController _owner;
        private float _nextThinkTime;
        private Vector2 _movementIntent;
        private float _stuckTimer;

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

            _owner.Move(_movementIntent.x);
            TrackStuckState();
        }

        private void Think()
        {
            var targets = FindObjectsOfType<PlayerController>()
                .Where(candidate => candidate != _owner && candidate.IsAlive)
                .ToArray();

            if (targets.Length == 0)
            {
                _movementIntent = Vector2.zero;
                return;
            }

            var target = SelectTacticalTarget(targets);
            var toTarget = target.transform.position - _owner.transform.position;
            var distance = toTarget.magnitude;

            _movementIntent = GetMovementVector(toTarget, distance);

            var hasLineOfFire = !Physics2D.Linecast(_owner.transform.position, target.transform.position, terrainMask);
            if (distance <= maxFireDistance && hasLineOfFire)
            {
                var inaccuracy = Random.insideUnitCircle * 0.8f;
                _owner.ShootAt((Vector2)target.transform.position + inaccuracy);
            }
        }

        private PlayerController SelectTacticalTarget(PlayerController[] targets)
        {
            return targets
                .OrderBy(target => Vector2.Distance(target.transform.position, _owner.transform.position))
                .ThenByDescending(target => target.Inventory.Count)
                .First();
        }

        private Vector2 GetMovementVector(Vector2 toTarget, float distance)
        {
            if (_stuckTimer > 1.2f)
            {
                return new Vector2(Mathf.Sign(Random.Range(-1f, 1f)), 0f);
            }

            if (distance < 5f)
            {
                return new Vector2(-Mathf.Sign(toTarget.x), 0f);
            }

            if (distance > 9f)
            {
                return new Vector2(Mathf.Sign(toTarget.x), 0f);
            }

            return Vector2.zero;
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

            var drift = Vector2.Distance(_recentPositions[0], _recentPositions[^1]);
            _stuckTimer = drift < 0.18f ? _stuckTimer + Time.deltaTime : 0f;
        }
    }
}
