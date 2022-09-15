using System.Collections;
using UnityEngine;

namespace Match3
{
    public class AnimatorEventReceiver : MonoBehaviour
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
        private void ExecutionStarted() => _chip.GetComponent<SpecialChip>().ExecutionStart();
        private void ExecutionEnded() => _chip.GetComponent<SpecialChip>().Executed();
        private void InteractionReady() => _chip.ChipReady();
    }
}