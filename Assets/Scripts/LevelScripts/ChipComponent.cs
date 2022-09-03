using System;
using System.Collections;
using UnityEngine;

namespace Match3
{
    public class ChipComponent : MonoBehaviour
    {
        public ChipType Type;
        [SerializeField]
        private bool _isSpecial;
        public bool IsSpecial => _isSpecial;
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private ChipChildComponent _child;
        private static readonly int Show = Animator.StringToHash("showUp");
        private static readonly int Fade = Animator.StringToHash("fadeOut");
        private static readonly int FastShow = Animator.StringToHash("fastShowUp");
        private static readonly int Fall = Animator.StringToHash("fall");
        private static readonly int EndFall = Animator.StringToHash("endFall");
        private CellComponent _targetCell;
        public CellComponent CurrentCell;
        private DirectionType _direction;
        private bool _sentBack;
        public bool IsInteractable { get; private set; }
        public bool IsTransferred { get; private set; }
        public bool IsAnimating { get; private set; }

        public void Move(DirectionType direction, bool isPrimaryChip, CellComponent targetCell)
        {
            SetAnimationState(true);
            _targetCell = targetCell;
            transform.parent = _targetCell.transform;
            _direction = direction;
            _child.IsPrimaryChip = isPrimaryChip;
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

            if (!_sentBack) _targetCell.CheckMatches(this);
        }

        public IEnumerator MoveBackRoutine()
        {
            _sentBack = true;
            Move(_direction.OppositeDirection(), _child.IsPrimaryChip, CurrentCell);
            yield return new WaitWhile(() => IsAnimating);
            SetInteractionState(true);
        }

        public void AnimationEnded()
        {
            if (_sentBack)
                _sentBack = false;

            else if(_targetCell.NotNull()) CurrentCell = _targetCell;
        }

        public void Transfer(CellComponent cell)
        {
            if (CurrentCell.NotNull())
                CurrentCell.SetCurrentChip(null);
            CurrentCell = cell;
            StartCoroutine(ChipTransferRoutine(cell, 0.5f));
        }

        private IEnumerator ChipTransferRoutine(CellComponent targetCell, float transferTime)
        {
            IsTransferred = true;
            SetAnimationState(true);
            _animator.SetTrigger(Fall);
            float time = 0f;
            Vector3 start = transform.position;
            Vector3 target = targetCell.transform.position;
            while (time < 1f)
            {
                transform.position = Vector3.Lerp(start, target, time * time);
                time += Time.deltaTime / transferTime;
                yield return null;
            }
            transform.position = target;
            targetCell.CheckMatches(this);
            _animator.SetTrigger(EndFall);
            transform.parent = targetCell.transform;
        }

        public void SetInteractionState(bool state) => IsInteractable = state;
        public void SetAnimationState(bool state) => IsAnimating = state;
        public bool GetMatchState() => CurrentCell.IsMatch;
        public void EndTransfer() => IsTransferred = false;

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
            transform.parent = null;
            SetAnimationState(true);
            _animator.SetTrigger(Fade);
        }
    }
}