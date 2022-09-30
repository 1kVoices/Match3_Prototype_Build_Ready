using System;
using UnityEngine;

namespace Match3
{
    public class BlackScreen : MonoBehaviour
    {
        [SerializeField]
        private Animator _animator;
        public event Action OnScreenDarken;
        public event Action OnScreenWhitening;

        private void ScreenDarken() => OnScreenDarken?.Invoke();
        private void ScreenBleached() => OnScreenWhitening?.Invoke();
        public void Whitening() => _animator.SetTrigger(Extensions.Show);
        public void Darken() => _animator.SetTrigger(Extensions.Hide);
    }
}