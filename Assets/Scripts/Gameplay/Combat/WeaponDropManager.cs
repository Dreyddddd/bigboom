using System.Collections;
using BigBoom.Gameplay.Players;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BigBoom.Gameplay.Combat
{
    public class WeaponDropManager : MonoBehaviour
    {
        [SerializeField] private WeaponDatabase weaponDatabase;
        [SerializeField] private WeaponCrate cratePrefab;
        [SerializeField] private Vector2 dropZoneMin = new(-16f, 2f);
        [SerializeField] private Vector2 dropZoneMax = new(16f, 8f);
        [SerializeField] private Vector2 airSpawnOffset = new(0f, 10f);
        [SerializeField] private float dropIntervalSeconds = 12f;

        private Coroutine _dropRoutine;
        private Vector2 _lastCombatPosition;

        public void BeginRoundDrops()
        {
            StopRoundDrops();
            _dropRoutine = StartCoroutine(DropRoutine());
        }

        public void StopRoundDrops()
        {
            if (_dropRoutine == null)
            {
                return;
            }

            StopCoroutine(_dropRoutine);
            _dropRoutine = null;
        }

        public void NotifyPotentialDropPosition(Vector2 position)
        {
            _lastCombatPosition = position;
        }

        private IEnumerator DropRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(dropIntervalSeconds);
                SpawnCrate();
            }
        }

        private void SpawnCrate()
        {
            var dropPoint = new Vector2(
                Random.Range(dropZoneMin.x, dropZoneMax.x),
                Random.Range(dropZoneMin.y, dropZoneMax.y));

            if (_lastCombatPosition != Vector2.zero && Random.value > 0.45f)
            {
                dropPoint = _lastCombatPosition + new Vector2(Random.Range(-4f, 4f), Random.Range(2f, 5f));
            }

            var crate = Instantiate(cratePrefab, dropPoint + airSpawnOffset, Quaternion.identity);
            crate.Initialize(weaponDatabase.GetRandom(), dropPoint);
        }
    }
}
