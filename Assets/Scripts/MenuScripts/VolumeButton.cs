using UnityEngine;

namespace Match3
{
    public class VolumeButton : MonoBehaviour
    {
        [SerializeField]
        private Animator _sliderAnimator;
        public bool IsOpen { get; set; }= true;

        public void OpenVolume_UnityEvent()
        {
            IsOpen = !IsOpen;
            _sliderAnimator.SetTrigger(IsOpen ? Extensions.Hide : Extensions.Show);
        }

        public void CloseVolume_UnityEvent()
        {
            _sliderAnimator.SetTrigger(Extensions.Hide);
        }
    }
}