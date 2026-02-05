using BigBoom.Gameplay.Combat;
using BigBoom.Gameplay.Rounds;
using BigBoom.Gameplay.Terrain;
using BigBoom.Gameplay.UI;
using UnityEngine;

namespace BigBoom.Core
{
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private GameSettings gameSettings;
        [SerializeField] private RoundManager roundManager;
        [SerializeField] private TerrainGenerator terrainGenerator;
        [SerializeField] private LeaderboardPresenter leaderboardPresenter;
        [SerializeField] private WeaponDropManager weaponDropManager;

        private SessionConfig _sessionConfig;

        private void Awake()
        {
            _sessionConfig = new SessionConfig(gameSettings.RoundsCount, gameSettings.BotCount, gameSettings.GameMode);
            roundManager.Initialize(_sessionConfig, terrainGenerator, leaderboardPresenter, weaponDropManager);
        }

        private void Start()
        {
            StartSession();
        }

        public void StartSession()
        {
            roundManager.StartMatch();
        }

        public void ApplyLobbyConfiguration(int rounds, int bots)
        {
            gameSettings.ApplySessionOverrides(rounds, bots);
            _sessionConfig = new SessionConfig(rounds, bots, gameSettings.GameMode);
            roundManager.Initialize(_sessionConfig, terrainGenerator, leaderboardPresenter, weaponDropManager);
        }
    }
}
