using System;
using UnityEngine;
using Zenject;

namespace pstudio.GoM.Game.Controllers
{
    public class InputController : ITickable
    {
        private readonly WorldSimulator _simulator;
        private readonly TimeController _timeController;
        private readonly Camera _camera;

        public InputController(WorldSimulator worldSimulator, TimeController timeController, Camera camera)
        {
            _simulator = worldSimulator;
            _timeController = timeController;
            _camera = camera;
        }

        public void Tick()
        {
            // DEBUG feature - generate new world
            if (Input.GetKeyDown(KeyCode.R))
                _simulator.GenerateNewWorld();

            // Toggle pause
            if (Input.GetKeyDown(KeyCode.Space))
                _timeController.TogglePause();

            // Speed up time
            if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Plus))
                _timeController.IncreaseSpeed();

            // Slow down time
            if (Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.Minus))
                _timeController.DecreaseSpeed();

            // Move map
            const float camSpeed = 9f;
            // Left
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                var position = _camera.transform.position;
                position.x = Math.Max(position.x - camSpeed*Time.deltaTime, -12f);
                _camera.transform.position = position;
            }

            // Right
            if (Input.GetKey(KeyCode.RightArrow))
            {
                var position = _camera.transform.position;
                position.x = Math.Min(position.x + camSpeed*Time.deltaTime, 12f);
                _camera.transform.position = position;
            }

            // Up
            if (Input.GetKey(KeyCode.UpArrow))
            {
                var position = _camera.transform.position;
                position.y = Math.Min(position.y + camSpeed*Time.deltaTime, 9f);
                _camera.transform.position = position;
            }

            // Down
            if (Input.GetKey(KeyCode.DownArrow))
            {
                var position = _camera.transform.position;
                position.y = Math.Max(position.y - camSpeed*Time.deltaTime, -9f);
                _camera.transform.position = position;
            }

            // Zoom Level 1
            if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1))
                LeanTween.moveZ(_camera.gameObject, -20f, 0.3f);

            // Zoom Level 2
            if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2))
                LeanTween.moveZ(_camera.gameObject, -15f, 0.3f);

            // Zoom Level 3
            if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3))
                LeanTween.moveZ(_camera.gameObject, -10f, 0.3f);

            // Zoom Level 4
            if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4))
                LeanTween.moveZ(_camera.gameObject, -5f, 0.3f);
        }
    }
}
