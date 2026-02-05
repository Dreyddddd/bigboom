using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BigBoom.Core;
using BigBoom.Gameplay.Combat;
using BigBoom.Gameplay.Players;
using BigBoom.Gameplay.Terrain;
using BigBoom.Gameplay.UI;
using UnityEngine;

namespace BigBoom.Gameplay.Rounds
{
    public class RoundManager : MonoBehaviour
    {
        [SerializeField] private PlayerFactory playerFactory;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private float preRoundDelay = 1.5f;
        [SerializeField] private float postRoundDelay = 2.5f;

        private readonly List<PlayerController> _players = new();
        private readonly Dictionary<string, int> _killsByPlayer = new();

        private SessionConfig _sessionConfig;
        private TerrainGenerator _terrainGenerator;
        private LeaderboardPresenter _leaderboardPresenter;
        private WeaponDropManager _weaponDropManager;
        private int _currentRound;

        public void Initialize(
            SessionConfig sessionConfig,
            TerrainGenerator terrainGenerator,
            LeaderboardPresenter leaderboardPresenter,
            WeaponDropManager weaponDropManager)
        {
            _sessionConfig = sessionConfig;
            _terrainGenerator = terrainGenerator;
            _leaderboardPresenter = leaderboardPresenter;
            _weaponDropManager = weaponDropManager;
        }

        public void StartMatch()
        {
            StopAllCoroutines();
            _currentRound = 0;
            _killsByPlayer.Clear();
            StartCoroutine(RunMatchCoroutine());
        }

        private IEnumerator RunMatchCoroutine()
        {
            while (_currentRound < _sessionConfig.Rounds)
            {
                _currentRound++;
                yield return StartCoroutine(RunRoundCoroutine());
            }

            _leaderboardPresenter.ShowFinal(_killsByPlayer);
        }

        private IEnumerator RunRoundCoroutine()
        {
            CleanupRound();
            _terrainGenerator.GenerateRoundTerrain();

            yield return new WaitForSeconds(preRoundDelay);

            SpawnBots();
            _weaponDropManager.BeginRoundDrops();

            while (_players.Count(p => p.IsAlive) > 1)
            {
                yield return null;
            }

            _weaponDropManager.StopRoundDrops();
            AwardRoundResult();
            _leaderboardPresenter.ShowLive(_killsByPlayer, _currentRound, _sessionConfig.Rounds);

            yield return new WaitForSeconds(postRoundDelay);
        }

        private void SpawnBots()
        {
            for (var i = 0; i < _sessionConfig.Bots; i++)
            {
                var spawnPoint = spawnPoints[i % spawnPoints.Length];
                var player = playerFactory.CreateBot($"Bot_{i + 1}", spawnPoint.position);
                player.KilledEnemy += OnPlayerKilledEnemy;
                player.Died += OnPlayerDied;
                _players.Add(player);
                if (!_killsByPlayer.ContainsKey(player.PlayerId))
                {
                    _killsByPlayer[player.PlayerId] = 0;
                }
            }
        }

        private void OnPlayerKilledEnemy(PlayerController killer, PlayerController _)
        {
            _killsByPlayer[killer.PlayerId]++;
            _leaderboardPresenter.ShowLive(_killsByPlayer, _currentRound, _sessionConfig.Rounds);
        }

        private void OnPlayerDied(PlayerController victim)
        {
            _weaponDropManager.NotifyPotentialDropPosition(victim.transform.position);
        }

        private void AwardRoundResult()
        {
            var winner = _players.FirstOrDefault(player => player.IsAlive);
            if (winner == null)
            {
                return;
            }

            _killsByPlayer[winner.PlayerId]++;
        }

        private void CleanupRound()
        {
            foreach (var player in _players)
            {
                if (player != null)
                {
                    player.KilledEnemy -= OnPlayerKilledEnemy;
                    player.Died -= OnPlayerDied;
                    Destroy(player.gameObject);
                }
            }

            _players.Clear();
        }
    }
}
