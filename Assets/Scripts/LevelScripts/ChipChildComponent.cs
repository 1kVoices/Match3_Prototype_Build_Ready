using System;
using System.Collections;
using UnityEngine;

namespace Match3
{
    public class ChipChildComponent : MonoBehaviour
    {
        [SerializeField]
        private ChipComponent _parent;
        [SerializeField]
        private SpriteRenderer _renderer;
        public bool IsPrimaryChip;
        public event Action AnimationEndEvent;

        public void OnAnimationStart()
        {
            _parent.IsInteractable = false;
            if (IsPrimaryChip) _renderer.sortingOrder = 10;
        }

        private IEnumerator AnimationEndRoutine()
        {
            yield return null;
            transform.parent.position = transform.position;
            transform.localPosition = Vector3.zero;
            _renderer.sortingOrder = 0;
            _parent.IsInteractable = true;
            AnimationEndEvent?.Invoke();
        }
    }
}