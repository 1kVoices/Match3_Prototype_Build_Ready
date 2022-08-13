using System;
using UnityEngine;

namespace Match3
{
    public class ChipComponent : MonoBehaviour
    {
        public ChipType Type;
        [SerializeField]
        private Animator _animator;
        public ChipChildComponent Child;
        public bool IsInteractable { get; set; } = true;

        public void Move(DirectionType direction, bool isPrimaryChip)
        {
            Child.IsPrimaryChip = isPrimaryChip;
            string trigger = isPrimaryChip
                ? "Primary"
                : "Secondary";
            switch (direction)
            {
                case DirectionType.Top:
                    _animator.SetTrigger(string.Concat("top", trigger));
                    break;
                case DirectionType.Bot:
                    _animator.SetTrigger(string.Concat("bot", trigger));
                    break;
                case DirectionType.Left:
                    _animator.SetTrigger(string.Concat("left", trigger));
                    break;
                case DirectionType.Right:
                    _animator.SetTrigger(string.Concat("right", trigger));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }
    }
}