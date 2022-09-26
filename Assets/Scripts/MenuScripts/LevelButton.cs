using UnityEngine;

namespace Match3
{
    public class LevelButton : MonoBehaviour
    {
        [SerializeField]
        private Animator _animator;
        private static readonly int Coloring = Animator.StringToHash("coloring");
        private static readonly int Show = Animator.StringToHash("showUp");
        private bool _isInteractable;
        public bool IsShowedUp { get; private set; }

        private void Colorize()
        {
            _animator.SetTrigger(Coloring);
        }

        public void ShowUp()
        {
            _animator.SetTrigger(Show);
        }

        public void ShowedUp()
        {
            IsShowedUp = true;
            if(_isInteractable)
                Colorize();
        }

        public void SetInteractionState(bool state) => _isInteractable = state;
    }
}