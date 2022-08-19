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
        private static readonly int Show = Animator.StringToHash("showUp");
        private static readonly int Fade = Animator.StringToHash("fadeOut");
        private static readonly int FastShow = Animator.StringToHash("fastShowUp");
        private static readonly int Fall = Animator.StringToHash("fall");
        private static readonly int EndFall = Animator.StringToHash("endFall");
        private CellComponent _targetCell;
        private CellComponent _currentCell;
        private DirectionType _direction;
        private bool _sentBack;
        private bool _sentBySpawner;
        public bool IsAnimating { get; private set; }
        public ChipChildComponent Child;

        public void Move(DirectionType direction, bool isPrimaryChip, CellComponent targetCell)
        {
            IsAnimating = true;
            _targetCell = targetCell;
            _direction = direction;
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

            if (!_sentBack) _targetCell.CheckMatches(this);
        }

        public void MoveBack()
        {
            _sentBack = true;
            Move(_direction.OppositeDirection(), Child.IsPrimaryChip, _currentCell);
        }

        public void AnimationEnded()
        {
            IsAnimating = false;
            if (_sentBack)
            {
                _sentBack = false;
                LevelManager.Singleton.SetAnimationState(false);
            }
            else _currentCell = _targetCell;
        }

        // public void Transfer(CellComponent cell, bool isSentByCell)
        // {
        //     StartCoroutine(ChipTransferRoutine(cell, isSentByCell, 1f));
        // }

        // private IEnumerator ChipTransferRoutine(CellComponent targetCell, bool isSentByCell, float transferTime)
        // {
        //     float time = 0f;
        //     Vector3 start = transform.position;
        //     Vector3 target = targetCell.transform.position;
        //     if(!isSentByCell) SentBySpawner(targetCell);
        //     while (time < 1f)
        //     {
        //         transform.position = Vector3.Lerp(start, target, time * time);
        //         time += Time.deltaTime / transferTime;
        //         yield return null;
        //     }
        //     transform.position = target;
        //     EndFalling(targetCell);
        //     targetCell.CheckMatches(this);
        // }
        //
        // private void SentBySpawner(CellComponent targetCell)
        // {
        //     _animator.SetTrigger(Fall);
        //     _sentBySpawner = true;
        //     _targetCell = targetCell;
        //     _currentCell = null;
        // }
        //
        // private void EndFalling(CellComponent cell)
        // {
        //     _animator.SetTrigger(EndFall);
        //     _currentCell = cell;
        //     transform.parent = cell.transform;
        // }

        public void SetCurrentCell(CellComponent cell) => _currentCell = cell;
        public bool GetMatchState() => _currentCell.IsMatch;
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
            Child.SetCurrentCell(_currentCell);
        }
    }
}