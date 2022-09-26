using System.Collections;
using UnityEngine;

namespace Match3
{
    public class ChipAnimatorEventReceiver : MonoBehaviour
    {
        [SerializeField]
        private StandardChip _chip;

        private IEnumerator SwapEnded()
        {
            _chip.SwapEnded();
            yield return null;
            transform.parent.position = transform.position;
            transform.localPosition = Vector3.zero;
        }

        private void FallAnimationEnded() => _chip.FallingEnded();
        private void FadeAnimationEnded() => _chip.FadeAnimationEnded();
        private void InteractionReady() => _chip.ChipReady();
    }
}