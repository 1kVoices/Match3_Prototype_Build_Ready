using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField]
        private ChipComponent[] _chipPrefabs;
        [SerializeField]
        private ChipComponent _specialM18;
        [SerializeField]
        private ChipComponent _specialBlasterH;
        [SerializeField]
        private ChipComponent _specialBlasterV;
        [SerializeField]
        private ChipComponent _specialSun;
        public ChipComponent[] ChipPrefabs => _chipPrefabs;
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

        private void Start()
        {
            Singleton = this;
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
            if (callerCell.IsWaitingCell) return;
            CellComponent topNeighbour = callerCell.GetNeighbour(DirectionType.Top);

            if (topNeighbour.IsNull())
            {
                callerCell.SpawnPoint.GenerateChip(callerCell);
                callerCell.SetWaitingState(true);
                return;
            }

            while (callerCell.NotNull())
            {
                if (topNeighbour.IsNull())
                {
                    callerCell.SpawnPoint.GenerateChip(callerCell);
                    callerCell.SetWaitingState(true);
                    callerCell = callerCell.GetNeighbour(DirectionType.Top);

                    topNeighbour = callerCell.NotNull()
                        ? callerCell.GetNeighbour(DirectionType.Top)
                        : null;
                }
                else if (topNeighbour.NotNull() && topNeighbour.CurrentChip.NotNull() && !topNeighbour.IsWaitingCell)
                {
                    callerCell.SetWaitingState(true);
                    topNeighbour.CurrentChip.Transfer(callerCell);
                    callerCell = callerCell.GetNeighbour(DirectionType.Top);

                    topNeighbour = callerCell.NotNull()
                        ? callerCell.GetNeighbour(DirectionType.Top)
                        : null;
                }
                else if (topNeighbour.NotNull() && topNeighbour.CurrentChip.IsNull() || topNeighbour.IsWaitingCell)
                    topNeighbour = topNeighbour.GetNeighbour(DirectionType.Top);

            }
        }

        public void SetSpecialChip(CellComponent cell, MatchType type)
        {
            return;
            switch (type)
            {
                case MatchType.Horizontal4:
                    cell.SetSpecialChip(Instantiate(_specialBlasterH, cell.transform));
                    break;
                case MatchType.Vertical4:
                    cell.SetSpecialChip(Instantiate(_specialBlasterV, cell.transform));
                    break;
                case MatchType.T_type:
                    cell.SetSpecialChip(Instantiate(_specialM18, cell.transform));
                    break;
                case MatchType.Match5:
                    cell.SetSpecialChip(Instantiate(_specialSun, cell.transform));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
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