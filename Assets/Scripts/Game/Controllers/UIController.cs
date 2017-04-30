using pstudio.GoM.Game.Views.UI;
using UniRx;
using Zenject;

namespace pstudio.GoM.Game.Controllers
{
    public class UIController : IInitializable
    {
        private readonly WorldSimulator _simulator;
        private readonly TimeController _time;
        private readonly StatsUI _statsUI;

        public UIController(WorldSimulator worldSimulator, TimeController timeController, StatsUI statsUI)
        {
            _simulator = worldSimulator;
            _time = timeController;
            _statsUI = statsUI;
        }

        public void Initialize()
        {
            _simulator.Generations.SubscribeToText(_statsUI.GenerationsText);
            _time.GenerationsPerSecond.SubscribeToText(_statsUI.SpeedText);
            _time.Paused.Subscribe(b => _statsUI.PauseScreen.SetActive(b));
            _simulator.GameOver.Subscribe(b =>
            {
                _statsUI.GameOverScreen.SetActive(b);
                _statsUI.GameOverText.text = _simulator.Generations.Value.ToString();
            });
        }
    }

}
