using UnityEngine;

namespace BigBoom.Core
{
    [CreateAssetMenu(menuName = "BigBoom/Settings/Game Settings", fileName = "GameSettings")]
    public class GameSettings : ScriptableObject
    {
        [Header("Match")]
        [SerializeField, Min(1)] private int roundsCount = 5;
        [SerializeField, Range(1, 8)] private int botCount = 4;
        [SerializeField] private GameModeType gameMode = GameModeType.Deathmatch;

        [Header("Round")]
        [SerializeField] private float preRoundDelaySeconds = 1.5f;
        [SerializeField] private float postRoundDelaySeconds = 2.5f;

        public int RoundsCount => roundsCount;
        public int BotCount => botCount;
        public GameModeType GameMode => gameMode;
        public float PreRoundDelaySeconds => preRoundDelaySeconds;
        public float PostRoundDelaySeconds => postRoundDelaySeconds;

        public void ApplySessionOverrides(int newRoundsCount, int newBotCount)
        {
            roundsCount = Mathf.Max(1, newRoundsCount);
            botCount = Mathf.Clamp(newBotCount, 1, 8);
        }
    }
}
