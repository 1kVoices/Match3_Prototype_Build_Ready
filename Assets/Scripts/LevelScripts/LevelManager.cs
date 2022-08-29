using System.Collections;
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
        private Controls _controls;
        private Vector2 _startDragMousePos;
        private ChipComponent _primaryChip;
        private ChipComponent _secondaryChip;
        private CellComponent _primaryCell;
        private CellComponent _secondaryCell;
        private bool _isReading;

        private void Start()
        {
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
            yield return new WaitForSeconds(0.3f);
            foreach (CellComponent cell in _cells)
            {
                cell.CurrentChip.ShowUp();
                yield return new WaitForSeconds(0.01f);
            }
        }

        public static void GetNewChip(CellComponent callerCell)
        {
            if (callerCell.CurrentChip.NotNull()) return;
            CellComponent topNeighbour = callerCell.GetNeighbour(DirectionType.Top);

            if (topNeighbour.IsNull())
            {
                callerCell.SpawnPoint.GenerateChip(callerCell);
                return;
            }

            while (callerCell.NotNull())
            {
                if (topNeighbour.IsNull())
                {
                    callerCell.SpawnPoint.GenerateChip(callerCell);
                    callerCell = callerCell.GetNeighbour(DirectionType.Top);
                    topNeighbour = callerCell.NotNull()
                        ? callerCell.GetNeighbour(DirectionType.Top)
                        : null;
                }
                else if (topNeighbour.NotNull() && topNeighbour.CurrentChip.NotNull() && topNeighbour.CurrentChip.ReservedBy.IsNull())
                {
                    topNeighbour.CurrentChip.Transfer(callerCell);
                    callerCell = callerCell.GetNeighbour(DirectionType.Top);
                    topNeighbour = callerCell.NotNull()
                        ? callerCell.GetNeighbour(DirectionType.Top)
                        : null;
                }
                else if (topNeighbour.NotNull() && topNeighbour.CurrentChip.IsNull() || topNeighbour.CurrentChip.ReservedBy.NotNull())
                {
                    topNeighbour = topNeighbour.GetNeighbour(DirectionType.Top);
                }
            }
        }

        private void OnCellPointerUpEvent(CellComponent cell) => _isReading = false;
        private void OnCellPointerDownEvent(CellComponent cell, Vector2 cellPos)
        {
            _primaryCell = cell;
            _primaryChip = cell.CurrentChip;
            _startDragMousePos = cellPos;
            _isReading = true;
        }

        private void Update()
        {
            ReadPlayerInput();
        }

        private void ReadPlayerInput()
        {
            if (!_isReading) return;
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

            _secondaryChip = _secondaryCell.CurrentChip;

            if (_primaryCell.IsNull() || _secondaryCell.IsNull() || _secondaryCell.CurrentChip.IsNull() || _primaryCell.CurrentChip.IsNull()) return;

            if (!_primaryChip.IsInteractable || !_secondaryChip.IsInteractable) return;
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