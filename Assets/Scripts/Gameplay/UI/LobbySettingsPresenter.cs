using BigBoom.Core;
using UnityEngine;
using UnityEngine.UI;

namespace BigBoom.Gameplay.UI
{
    public class LobbySettingsPresenter : MonoBehaviour
    {
        [SerializeField] private Slider botCountSlider;
        [SerializeField] private Slider roundsSlider;
        [SerializeField] private Text botValueText;
        [SerializeField] private Text roundsValueText;
        [SerializeField] private GameBootstrap gameBootstrap;

        private void Start()
        {
            RefreshTexts();
        }

        public void OnBotSliderChanged(float value)
        {
            botValueText.text = Mathf.RoundToInt(value).ToString();
        }

        public void OnRoundsSliderChanged(float value)
        {
            roundsValueText.text = Mathf.RoundToInt(value).ToString();
        }

        public void StartMatchPressed()
        {
            var rounds = Mathf.RoundToInt(roundsSlider.value);
            var bots = Mathf.RoundToInt(botCountSlider.value);
            gameBootstrap.ApplyLobbyConfiguration(rounds, bots);
            gameBootstrap.StartSession();
        }

        private void RefreshTexts()
        {
            OnBotSliderChanged(botCountSlider.value);
            OnRoundsSliderChanged(roundsSlider.value);
        }
    }
}
