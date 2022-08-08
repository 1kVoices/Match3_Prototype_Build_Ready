using UnityEngine;

namespace Match3
{
    public class ChipComponent : MonoBehaviour
    {
        public ChipType Type;
        [SerializeField]
        private Animator _animator;
        public ChipChildComponent _child;

        public void Move(string trigger)
        {
            _animator.SetTrigger(trigger);
        }
    }
}