using UnityEngine;

namespace Match3
{
    public class PauseButton : MonoBehaviour
    {
        [SerializeField]
        private Animator _buttonsAnimator;
        [SerializeField]
        private VolumeButton _slider;
        private bool _isOpen = true;

        public void OpenMenu_UnityEvent()
        {
            _isOpen = !_isOpen;
            _buttonsAnimator.SetTrigger(_isOpen ? Extensions.Hide : Extensions.Show);
            if (_slider.IsOpen) return;
            _slider.CloseVolume_UnityEvent();
            _slider.IsOpen = true;
        }

        public void ChangeVolume_UnityEvent(float f) => SoundManager.Singleton.ChangeVolume(f);

        public void Exit_UnityEvent()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE_WIN
            Application.Quit();
#endif
        }
    }
}