using UnityEngine;

namespace Match3
{
    public class CellHighlighter : MonoBehaviour
    {
        private Cell _parent;
        private Animator _animator;
        private SpriteRenderer _renderer;
        private static readonly int StartTrigger = Animator.StringToHash("start");
        private static readonly int StopTrigger = Animator.StringToHash("stop");
        private Color _red;
        private Color _blue;

        private void Start()
        {
            _parent = GetComponentInParent<Cell>();
            _animator = GetComponent<Animator>();
            _renderer = GetComponent<SpriteRenderer>();
            _red = new Color(255, 0, 0, 255);
            _blue = new Color(0, 255, 255, 0);
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

        public void Redness(bool isRed)
        {
            if (isRed)
            {
                _animator.enabled = false;
                _renderer.color = _red;
            }
            else
            {
                _renderer.color = _blue;
                _animator.enabled = true;
            }

        }
    }
}