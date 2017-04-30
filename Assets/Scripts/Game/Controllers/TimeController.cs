using UniRx;
using UnityEngine;
using Zenject;

namespace pstudio.GoM.Game.Controllers
{
    public class TimeController : IInitializable, ITickable
    {
        private readonly WorldSimulator _simulator;

        private float _timer;

        public ReactiveProperty<float> GenerationsPerSecond { get; private set; }

        public ReactiveProperty<bool> Paused { get; private set; }

        public TimeController(WorldSimulator worldSimulator)
        {
            _simulator = worldSimulator;

            GenerationsPerSecond = new ReactiveProperty<float>();
            Paused = new ReactiveProperty<bool>();
        }

        public void SetSpeed(float speed)
        {
            GenerationsPerSecond.Value = Mathf.Clamp(speed, 0.5f, 8f);
        }

        public void IncreaseSpeed()
        {
            SetSpeed(GenerationsPerSecond.Value*2f);
        }

        public void DecreaseSpeed()
        {
            SetSpeed(GenerationsPerSecond.Value/2f);
        }

        public void TogglePause()
        {
            Paused.Value = !Paused.Value;
        }

        public void Initialize()
        {
            GenerationsPerSecond.Value = 1f;
            _timer = 0f;
            Paused.Value = false;
        }

        public void Tick()
        {
            if (Paused.Value) return;

            _timer += Time.deltaTime;

            var target = 2f/GenerationsPerSecond.Value;
            while (_timer > target)
            {
                _simulator.DoGeneration();
                _timer -= target;
            }
        }
    }

}
