using System;
using TMPro;
using UnityEngine;

namespace Match3
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class GameTime : MonoBehaviour
    {
        private TextMeshProUGUI _text;
        private float _time;
        private TimeSpan _timeSpan;
        private bool _isActive;
        public event Action<bool> OutOfTimeEvent;

        private void Start() => _text = GetComponent<TextMeshProUGUI>();

        public void SetTimer(float time)
        {
            _time = time;
            ConvertToTimeSpan();
        }

        private void Update()
        {
            if (!_isActive || _time <= 0) return;

            _time -= Time.deltaTime;
            ConvertToTimeSpan();

            if (_time <= 0) OutOfTimeEvent?.Invoke(false);
        }

        public void StartTimer() => _isActive = true;
        public void StopTimer() => _isActive = false;

        private void ConvertToTimeSpan()
        {
            _timeSpan = TimeSpan.FromSeconds(_time);
            _text.text = _timeSpan.ToString(@"mm\:ss");
        }
    }
}