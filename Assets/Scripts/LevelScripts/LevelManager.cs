using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private LinkedList<CellComponent> _affectedCells;
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
        private bool _isChecked;
        public int COUNT;

        private void Start()
        {
            _controls = new Controls();
            _controls.Enable();
            _chipPool = new LinkedList<ChipComponent>();
            _affectedCells = new LinkedList<CellComponent>();
            _isChecked = false;
            foreach (CellComponent cell in _cells)
            {
                cell.PoolingCellEvent += OnPoolingCellEvent;
                cell.PointerDownEvent += OnCellPointerDownEvent;
                cell.PointerUpEvent += OnCellPointerUpEvent;
                cell.Chip.Child.AnimationEndEvent += OnChipAnimationEnd;
                cell.Chip.Child.FadeEndEvent += OnPoolingChipEvent;
            }

            StartCoroutine(ChipsShowUp());
        }

        private IEnumerator ChipsShowUp() //todo
        {
            _isAnimating = true;
            yield return new WaitForSeconds(0.3f);
            foreach (CellComponent cell in _cells)
            {
                cell.Chip.ShowUp();
                yield return new WaitForSeconds(0.01f);
            }
            _isAnimating = false;
        }

        private void OnChipAnimationEnd()
        {
            if (_isChecked) return;
            _isChecked = true;
            _isAnimating = false;
            _primaryCell.CheckMatch();
            _secondaryCell.CheckMatch();

            if (_primaryCell.IsMatch || _secondaryCell.IsMatch)
            {
                foreach (CellComponent cellComponent in _affectedCells.OrderBy(z => z.transform.position.y))
                {
                    cellComponent.Kidnapping(_chipPool);
                }
            }
            else
            {
                if (_isSwappedBack) return;
                _isAnimating = true;
                StartCoroutine(SwapBackRoutine());
            }
        }

        private void SwapChipTransforms(bool isDirectSwap)
        {
            _primaryCell.Chip = isDirectSwap
                ? _secondaryChip
                : _primaryChip;
            _secondaryCell.Chip = isDirectSwap
                ? _primaryChip
                : _secondaryChip;
            _primaryChip.transform.parent = isDirectSwap
                ? _secondaryCell.transform
                : _primaryCell.transform;
            _secondaryChip.transform.parent = isDirectSwap
                ? _primaryCell.transform
                : _secondaryCell.transform;
        }

        private void OnPoolingCellEvent(CellComponent cell)
        {
            _affectedCells.AddLast(cell);
        }

        private void FixedUpdate()
        {
            print(_chipPool.Count);
        }

        private void OnPoolingChipEvent(ChipComponent chip)
        {
            _chipPool.AddLast(chip);
        }

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
            _startDragMousePos = cellPos;
            _isReading = true;
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
            SwapChipTransforms(false);
            _isSwappedBack = true;
        }

        private void SwapChips(DirectionType direction, bool isSwapBack = false)
        {
            if (!isSwapBack)
            {
                if (!_primaryCell.Neighbours.ContainsKey(direction)) return;

                _secondaryCell = _primaryCell.Neighbours[direction];
                _secondaryChip = _secondaryCell.Chip;
            }
            if(_primaryCell == null || _secondaryCell == null || _secondaryCell.Chip == null) return;
            _isAnimating = true;
            _primaryChip.Move(direction, true);
            _secondaryChip.Move(OppositeDirection(direction), false);
            SwapChipTransforms(true);
            _isChecked = false;
            _affectedCells.Clear();
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
                cell.PoolingCellEvent -= OnPoolingCellEvent;
                cell.PointerDownEvent -= OnCellPointerDownEvent;
                cell.PointerUpEvent -= OnCellPointerUpEvent;
                if(cell.Chip != null)
                    cell.Chip.Child.AnimationEndEvent -= OnChipAnimationEnd;
            }
        }
    }
}