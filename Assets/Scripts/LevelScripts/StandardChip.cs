using System;
using System.Collections;
using UnityEngine;

namespace Match3
{
    public class StandardChip : MonoBehaviour
    {
        [SerializeField]
        private ChipType _type;
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private SpriteRenderer _renderer;
        protected Animator ChipAnimator => _animator;
        protected Cell CurrentCell { get; private set; }
        public Cell PreviousCell { get; private set; }
        private DirectionType _direction;
        private bool _isSentBack;
        private bool IsPrimaryChip { get; set; }
        public ChipType Type => _type;
        public Cell ReservedBy { get; private set; }
        public bool IsInteractable { get; private set; }
        public bool IsAnimating { get; private set; }

        public void Move(DirectionType direction, bool isPrimaryChip, Cell targetCell)
        {
            SetAnimationState(true);
            SetInteractionState(false);
            LevelManager.Singleton.SetInputState(false);

            IsPrimaryChip = isPrimaryChip;
            _renderer.sortingOrder = IsPrimaryChip ? 10 : 0;
            SetPreviousCell(CurrentCell);
            SetCurrentCell(targetCell);
            transform.parent = CurrentCell.transform;
            _direction = direction;
            string priority = isPrimaryChip
                ? "Primary"
                : "Secondary";
            switch (_direction)
            {
                case DirectionType.Top:
                    _animator.SetTrigger(string.Concat("top", priority));
                    break;
                case DirectionType.Bot:
                    _animator.SetTrigger(string.Concat("bot", priority));
                    break;
                case DirectionType.Left:
                    _animator.SetTrigger(string.Concat("left", priority));
                    break;
                case DirectionType.Right:
                    _animator.SetTrigger(string.Concat("right", priority));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            if (!_isSentBack) CurrentCell.CheckMatches(this, false);
        }

        public void SendBack()
        {
            _isSentBack = true;
            Move(_direction.OppositeDirection(), IsPrimaryChip, PreviousCell);
            SetPreviousCell(null);
        }

        public void FallingEnded()
        {
            ChipReady();
            SetAnimationState(false);
            CurrentCell.ChipMoved();
        }

        public void SwapEnded()
        {
            SetAnimationState(false);
            _renderer.sortingOrder = 0;

            if (_isSentBack)
            {
                _isSentBack = false;
                ChipReady();
                return;
            }
            CurrentCell.ChipMoved();
        }

        public void ChipReady()
        {
            SetInteractionState(true);
            LevelManager.Singleton.SetInputState(true);
        }

        public void FadeAnimationEnded()
        {
            CurrentCell.SetPulledByCell(null);
            _animator.enabled = false;
            Pool.Singleton.Pull(this);
            LevelManager.Singleton.GetNewChip();
        }

        public IEnumerator Transfer(Cell targetCell, float transferTime)
        {
            ReservedBy = targetCell;
            CurrentCell = targetCell;
            SetAnimationState(true);
            _animator.SetTrigger(Extensions.Fall);

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
            CurrentCell.CheckMatches(this, true);
            _animator.SetTrigger(Extensions.EndFall);
            transform.parent = targetCell.transform;
            ReservedBy = null;
        }

        public void SetCurrentCell(Cell cell) => CurrentCell = cell;
        public void SetPreviousCell(Cell cell) => PreviousCell = cell;
        public void SetInteractionState(bool state) => IsInteractable = state;
        protected void SetAnimationState(bool state) => IsAnimating = state;

        public void ShowUp()
        {
            _animator.SetTrigger(Extensions.Show);
            transform.localScale = Vector3.one;
        }

        public void FastShowUp()
        {
            _animator.enabled = true;
            _animator.SetTrigger(Extensions.FastShow);
            transform.localScale = Vector3.one;
        }

        public virtual void FadeOut(SpecialChip specialChip)
        {
            transform.parent = null;
            SetAnimationState(true);

            if (specialChip.IsNull())
            {
                _animator.SetTrigger(Extensions.Fade);
                return;
            }

            switch (specialChip.SpecialType)
            {
                case SpecialChipType.SpecialSun:
                    _animator.SetTrigger(Extensions.SunFade);
                    break;
                case SpecialChipType.SpecialM18:
                    _animator.SetTrigger(Extensions.FastFade);
                    break;
                case SpecialChipType.SpecialBlasterH:
                    _animator.SetTrigger(Extensions.FastFade);
                    break;
                case SpecialChipType.SpecialBlasterV:
                    _animator.SetTrigger(Extensions.FastFade);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}