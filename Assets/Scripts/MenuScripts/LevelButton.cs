using UnityEngine;

namespace Match3
{
    public class LevelButton : MonoBehaviour
    {
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private Animator _rewardAnimator;
        private bool _isInteractable;
        public bool IsShowedUp { get; private set; }
        public bool IsRewardShowed { get; private set; }

        private void Colorize() => _animator.SetTrigger(Extensions.Coloring);

        public void ShowUp() => _animator.SetTrigger(Extensions.Show);

        public void ShowedUp()
        {
            IsShowedUp = true;
            if(_isInteractable)
                Colorize();
        }

        public void ShowReward() => _rewardAnimator.SetTrigger(Extensions.Show);

        public void HideReward() => _rewardAnimator.SetTrigger(Extensions.Hide);

        public void SetInteractionState(bool state) => _isInteractable = state;
        public void SetRewardState(bool state) => IsRewardShowed = state;
    }
}