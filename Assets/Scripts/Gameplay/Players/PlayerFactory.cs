using BigBoom.Gameplay.AI;
using BigBoom.Gameplay.Terrain;
using UnityEngine;

namespace BigBoom.Gameplay.Players
{
    public class PlayerFactory : MonoBehaviour
    {
        [SerializeField] private PlayerController botPrefab;
        [SerializeField] private BotBrain botBrainPrefab;
        [SerializeField] private DestructibleTerrain terrain;

        public PlayerController CreateBot(string botId, Vector2 position)
        {
            var player = Instantiate(botPrefab, position, Quaternion.identity);
            player.Setup(botId, terrain);

            var brain = Instantiate(botBrainPrefab, player.transform);
            brain.Initialize(player);

            return player;
        }
    }
}
