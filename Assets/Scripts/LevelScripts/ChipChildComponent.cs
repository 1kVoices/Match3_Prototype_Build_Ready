using System;
using System.Collections;
using UnityEngine;

namespace Match3
{
    public class ChipChildComponent : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _renderer;
        [SerializeField]
        private ChipComponent _parent;
        [SerializeField]
        private Animator _animator;
        public bool IsPrimaryChip { get; set; }
        public event Action AnimationEndEvent;
        public event Action<ChipComponent> FadeEndEvent;

        private void OnAnimationStart()
        {
            if (IsPrimaryChip) _renderer.sortingOrder = 10;
        }

        private IEnumerator AnimationEndRoutine()
        {
            yield return null;
            transform.parent.position = transform.position;
            transform.localPosition = Vector3.zero;
            _renderer.sortingOrder = 0;
            AnimationEndEvent?.Invoke();
        }

        private void FadeAnimationEnd()
        {
            _animator.enabled = false;
            FadeEndEvent?.Invoke(_parent);
        }
    }
}