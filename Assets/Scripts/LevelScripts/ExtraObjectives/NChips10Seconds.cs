using UnityEngine;

namespace Match3
{
    public class NChips10Seconds : ExtraObjective
    {
        private float _timer;
        private bool _timerReady;

        protected override void Init()
        {
            LevelManager.Singleton.OnDefaultDestroy += Destroyed;
            LevelManager.Singleton.OnPlayerInput += PlayerInput;
            _timerReady = false;
            _timer = 10;
            UpdateCount();
            UpdateText();
        }

        private void Destroyed() => ConditionMet();
        private void PlayerInput() => _timerReady = true;
        private void Update()
        {
            if (IsCompleted || !_timerReady) return;

            _timer -= Time.deltaTime;
            if (_timer <= 0) TimeIsUp();

            UpdateText();
        }

        private void TimeIsUp()
        {
            CurrentCount = 0;
            _timer = 10;
            _timerReady = false;
        }

        private void UpdateText() => _questText.text = $"Destroy {TargetCount - CurrentCount} chips within sec: {_timer:n0} ";

        protected override void Completed()
        {
            LevelManager.Singleton.OnPlayerInput -= PlayerInput;
            LevelManager.Singleton.OnDefaultDestroy -= Destroyed;
        }
    }
}