using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Singleton;
        private int _currentLevel;
        [SerializeField]
        private CellComponent[] _cells;
        [SerializeField]
        private float _dragSens;
        private Controls _controls;
        private Vector2 _startDragMousePos;
        private ChipComponent _primaryChip;
        private ChipComponent _secondaryChip;
        private CellComponent _primaryCell;
        private CellComponent _secondaryCell;
        private bool _isReading;
        private bool _isAnimating;

        private void Start()
        {
            if (!Singleton) Singleton = this;
            else Destroy(gameObject);

            _controls = new Controls();
            _controls.Enable();
            foreach (CellComponent cell in _cells)
            {
                cell.PointerDownEvent += OnCellPointerDownEvent;
                cell.PointerUpEvent += OnCellPointerUpEvent;
            }

            StartCoroutine(ChipsShowUp());
        }

        private IEnumerator ChipsShowUp() //todo
        {
            SetAnimationState(true);
            yield return new WaitForSeconds(0.3f);
            foreach (CellComponent cell in _cells)
            {
                cell.Chip.ShowUp();
                yield return new WaitForSeconds(0.01f);
            }
            SetAnimationState(false);
        }

        public void SetAnimationState(bool state) => _isAnimating = state;

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
        }

        private void Update()
        {
            ReadPlayerInput();
        }

        private void ReadPlayerInput()
        {
            if (!_isReading || _isAnimating) return;
            Vector2 newMousePos = _controls.MainMap.Mouse.ReadValue<Vector2>();

            if (newMousePos.y - _startDragMousePos.y > _dragSens)
            {
                _isReading = false;
                SwapChips(DirectionType.Top);
            }
            else if (newMousePos.y - _startDragMousePos.y < -_dragSens)
            {
                _isReading = false;
                SwapChips(DirectionType.Bot);
            }
            else if (newMousePos.x - _startDragMousePos.x < -_dragSens)
            {
                _isReading = false;
                SwapChips(DirectionType.Left);
            }
            else if (newMousePos.x - _startDragMousePos.x > _dragSens)
            {
                _isReading = false;
                SwapChips(DirectionType.Right);
            }
        }

        private void SwapChips(DirectionType direction)
        {
            _secondaryCell = _primaryCell.GetNeighbour(direction);
            if (_secondaryCell.IsNull()) return;

            _secondaryChip = _secondaryCell.Chip;

            if (_primaryCell.IsNull() || _secondaryCell.IsNull() || _secondaryCell.Chip.IsNull()) return;

            SetAnimationState(true);
            _primaryChip.Move(direction, true, _secondaryCell);
            _secondaryChip.Move(direction.OppositeDirection(), false, _primaryCell);
        }

        private void OnDestroy()
        {
            _controls.Dispose();
            foreach (CellComponent cell in _cells)
            {
                cell.PointerDownEvent -= OnCellPointerDownEvent;
                cell.PointerUpEvent -= OnCellPointerUpEvent;
            }
        }
    }
}