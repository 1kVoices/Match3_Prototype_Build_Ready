using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

namespace Match3
{
    public class LevelManager : MonoBehaviour
    {
        private int _currentLevel;

        [SerializeField]
        private CellComponent[] _cells;
        [SerializeField]
        private float _dragSens;
        private LinkedList<ChipComponent> _chipPool;
        private Controls _controls;
        private Vector2 _startDragMousePos;
        private ChipComponent _primaryChip;
        private ChipComponent _secondaryChip;
        private CellComponent _primaryCell;
        private CellComponent _secondaryCell;
        private DirectionType _playerDirection;
        private bool _isReading;
        private bool _isSwappedBack;
        private bool _isAnimating;

        private IEnumerator Start()
        {
            _controls = new Controls();
            _controls.Enable();
            _chipPool = new LinkedList<ChipComponent>();

            foreach (CellComponent cell in _cells)
            {
                cell.PoolingEvent += OnPoolingEvent;
                cell.PointerDownEvent += OnCellPointerDownEvent;
                cell.PointerUpEvent += OnCellPointerUpEvent;

                yield return null;
                cell.Chip.Child.AnimationEndEvent += OnChipChildAnimationEndEvent;
            }
        }

        private void OnChipChildAnimationEndEvent()
        {
            _primaryCell.Chip = _secondaryChip;
            _secondaryChip.transform.parent = _primaryCell.transform;
            _secondaryCell.Chip = _primaryChip;
            _primaryChip.transform.parent = _secondaryCell.transform;
            if (_primaryCell.IsMatch() || _secondaryCell.IsMatch())
            {
                _isAnimating = false;//todo
            }
            else
            {
                if (_isSwappedBack) return;
                StartCoroutine(SwapBackRoutine());
            }
        }

        private void OnPoolingEvent(ChipComponent chip) =>_chipPool.AddLast(chip);
        private void OnCellPointerUpEvent(CellComponent cell)
        {
            if(_isAnimating) return;
            _isReading = false;
        }
        private void OnCellPointerDownEvent(CellComponent cell, Vector2 cellPos)
        {
            if(_isAnimating) return;
            _primaryCell = cell;
            _primaryChip = cell.Chip;
            _isReading = true;
            _startDragMousePos = cellPos;
            _isSwappedBack = false;
        }

        private void Update()
        {
            ReadPlayerInput();
        }

        private void ReadPlayerInput()
        {
            if (!_isReading || _isAnimating) return;
            Vector2 newMousePos = _controls.MainMap.Mouse.ReadValue<Vector2>();

            if (newMousePos.x - _startDragMousePos.x > _dragSens)
            {
                _isReading = false;
                _playerDirection = DirectionType.Right;
                SwapChips(DirectionType.Right);
            }
            else if (newMousePos.x - _startDragMousePos.x < -_dragSens)
            {
                _isReading = false;
                _playerDirection = DirectionType.Left;
                SwapChips(DirectionType.Left);
            }
            else if (newMousePos.y - _startDragMousePos.y > _dragSens)
            {
                _isReading = false;
                _playerDirection = DirectionType.Top;
                SwapChips(DirectionType.Top);
            }
            else if (newMousePos.y - _startDragMousePos.y < -_dragSens)
            {
                _isReading = false;
                _playerDirection = DirectionType.Bot;
                SwapChips(DirectionType.Bot);
            }
        }

        private IEnumerator SwapBackRoutine()
        {
            yield return null;
            SwapChips(OppositeDirection(_playerDirection), true);
            _isSwappedBack = true;

            yield return new WaitForSeconds(0.2f);
            _isAnimating = false;
        }

        private void SwapChips(DirectionType direction, bool isSwapBack = false)
        {
            _isAnimating = true;
            if (!isSwapBack)
            {
                if (!_primaryCell.Neighbours.ContainsKey(direction)) return;
                if (!_primaryCell.Chip.IsInteractable) return;

                _secondaryCell = _primaryCell.Neighbours[direction];
                _secondaryChip = _secondaryCell.Chip;
            }
            _primaryChip.Move(direction, true);
            _secondaryChip.Move(OppositeDirection(direction), false);
        }

        private static DirectionType OppositeDirection(DirectionType direction)
        {
            switch (direction)
            {
                case DirectionType.Top:
                    return DirectionType.Bot;
                case DirectionType.Bot:
                    return DirectionType.Top;
                case DirectionType.Left:
                    return DirectionType.Right;
                case DirectionType.Right:
                    return DirectionType.Left;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        private void OnDestroy()
        {
            _controls.Dispose();
            foreach (CellComponent cell in _cells)
            {
                cell.PoolingEvent -= OnPoolingEvent;
                cell.PointerDownEvent -= OnCellPointerDownEvent;
                cell.PointerUpEvent -= OnCellPointerUpEvent;
                cell.Chip.Child.AnimationEndEvent -= OnChipChildAnimationEndEvent;
            }
        }
    }
}