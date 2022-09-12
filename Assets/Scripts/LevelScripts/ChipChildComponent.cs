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

        private void SpecialAction()
        {
            if (_parent.GetType().IsSubclassOf(typeof(SpecialChipComponent)) == false) return;
            _parent.GetComponent<SpecialChipComponent>().Action();
        }

        private void OnAnimationStart()
        {
            if (IsPrimaryChip) _renderer.sortingOrder = 10;
            _parent.SetInteractionState(false);
            LevelManager.Singleton.SetInputState(false);
        }

        private IEnumerator AnimationEndRoutine()
        {
            AnimationEnded();
            yield return null;
            transform.parent.position = transform.position;
            transform.localPosition = Vector3.zero;
            _renderer.sortingOrder = 0;
            _parent.AnimationEnded();
        }

        private void FadeAnimationEnd()
        {
            AnimationEnded();
            _parent.CurrentCell.SetPulledByCell(null);
            _animator.enabled = false;
            Pool.Singleton.Pull(_parent);
        }

        private void AnimationEnded() => _parent.SetAnimationState(false);
        private void InteractionReady()
        {
            _parent.SetInteractionState(true);
            LevelManager.Singleton.SetInputState(true);
        }
    }
}