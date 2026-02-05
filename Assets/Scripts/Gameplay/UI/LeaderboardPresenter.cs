using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace BigBoom.Gameplay.UI
{
    public class LeaderboardPresenter : MonoBehaviour
    {
        [SerializeField] private Text leaderboardText;

        public void ShowLive(Dictionary<string, int> kills, int round, int roundsTotal)
        {
            leaderboardText.text = BuildTable(kills, $"Round {round}/{roundsTotal}");
        }

        public void ShowFinal(Dictionary<string, int> kills)
        {
            leaderboardText.text = BuildTable(kills, "Final Leaderboard");
        }

        private string BuildTable(Dictionary<string, int> kills, string title)
        {
            var builder = new StringBuilder();
            builder.AppendLine(title);
            builder.AppendLine("----------------");

            var rank = 1;
            foreach (var pair in kills.OrderByDescending(item => item.Value).ThenBy(item => item.Key))
            {
                builder.AppendLine($"#{rank} {pair.Key}: {pair.Value} frags");
                rank++;
            }

            return builder.ToString();
        }
    }
}
