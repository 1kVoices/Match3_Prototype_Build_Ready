using System;
using System.Collections;
using UnityEngine;

namespace Match3
{
    public class ChipComponent : MonoBehaviour
    {
        public ChipType Type;
        [SerializeField]
        private Animator _animator;
        public ChipChildComponent Child;
        private static readonly int Show = Animator.StringToHash("showUp");
        private static readonly int Fade = Animator.StringToHash("fadeOut");
        private static readonly int FastShow = Animator.StringToHash("fastShowUp");

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

        public void ShowUp()
        {
            _animator.SetTrigger(Show);
            transform.localScale = Vector3.one;
        }

        public void FastShowUp()
        {
            _animator.enabled = true;
            _animator.SetTrigger(FastShow);
            transform.localScale = Vector3.one;
        }

        public void FadeOut()
        {
            _animator.SetTrigger(Fade);
        }
    }
}