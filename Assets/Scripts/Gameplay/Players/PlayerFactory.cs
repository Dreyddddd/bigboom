using BigBoom.Gameplay.AI;
using BigBoom.Gameplay.Terrain;
using UnityEngine;

namespace BigBoom.Gameplay.Players
{
    public class PlayerFactory : MonoBehaviour
    {
        [SerializeField] private PlayerController playerPrefab;
        [SerializeField] private BotBrain botBrainPrefab;
        [SerializeField] private DestructibleTerrain terrain;

        public PlayerController CreateHuman(string playerId, Vector2 position)
        {
            var player = Instantiate(playerPrefab, position, Quaternion.identity);
            player.Setup(playerId, terrain, true);
            return player;
        }

        public PlayerController CreateBot(string botId, Vector2 position)
        {
            var player = Instantiate(playerPrefab, position, Quaternion.identity);
            player.Setup(botId, terrain, false);

            var brain = Instantiate(botBrainPrefab, player.transform);
            brain.Initialize(player);

            return player;
        }
    }
}
