using UnityEngine;

namespace Match3
{
    public class CellHighlighter : MonoBehaviour
    {
        private Cell _parent;
        private Animator _animator;
        private static readonly int StartTrigger = Animator.StringToHash("start");
        private static readonly int StopTrigger = Animator.StringToHash("stop");

        private void Start()
        {
            _parent = GetComponentInParent<Cell>();
            _animator = GetComponent<Animator>();
        }

        public void Activate()
        {
            _animator.SetTrigger(StartTrigger);
            _parent.SetHighlightState(true);
        }

        public void Deactivate()
        {
            _animator.SetTrigger(StopTrigger);
            _parent.SetHighlightState(false);
        }
    }
}