using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private bool _allowInput;
        public int REMOVE;

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

        public void GetNewChip(CellComponent callerCell)
        {
            CellComponent topNeighbour = callerCell.GetNeighbour(DirectionType.Top);
            if (topNeighbour.IsNull())
                callerCell.SpawnPoint.GenerateChip(callerCell);

            while (topNeighbour.NotNull())
            {
                if (topNeighbour.CurrentChip.NotNull() && topNeighbour.CurrentChip.ReservedBy.IsNull())
                {
                    StartCoroutine(topNeighbour.TransferChip(callerCell));
                    break;
                }
                if (topNeighbour.CurrentChip.NotNull() && topNeighbour.CurrentChip.ReservedBy.NotNull() || topNeighbour.CurrentChip.IsNull())
                    topNeighbour = topNeighbour.GetNeighbour(DirectionType.Top);

                if (topNeighbour.IsNull())
                    callerCell.SpawnPoint.GenerateChip(callerCell);
            }
        }

        public void DestroyChips(List<CellComponent> list, CellComponent sender)
        {
            if (sender.CurrentChip.Type != ChipType.None)
            {
                if (list.Count == 4)
                {
                    SetSpecialChip(sender,
                        list.PosYIdentical()
                            ? MatchType.Vertical4
                            : MatchType.Horizontal4);

                }
                else if (list.Count >= 5)
                {
                    if (list.PosXIdentical() || list.PosYIdentical())
                        SetSpecialChip(sender, MatchType.Match5);

                    else SetSpecialChip(sender, MatchType.T_type);
                }
            }

            foreach (CellComponent cell in list.OrderBy(z => z.transform.position.y))
                StartCoroutine(cell.ChipFadingRoutine());
        }

        private void SetSpecialChip(CellComponent cell, MatchType type)
        {
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

        public void SetInputState(bool state) => _allowInput = state;
        private void OnCellPointerUpEvent(CellComponent cell) => _isReading = false;
        private void OnCellPointerDownEvent(CellComponent cell, Vector2 cellPos)
        {
            if (!_allowInput) return;
            _primaryCell = cell;
            _primaryChip = cell.CurrentChip;
            _startDragMousePos = cellPos;
            _isReading = true;
        }

        private void Update() => ReadPlayerInput();
        private void ReadPlayerInput()
        {
            if (!_isReading) return;
            Vector2 newMousePos = _controls.MainMap.Mouse.ReadValue<Vector2>();

            if (newMousePos.y - _startDragMousePos.y > _dragSens)
                SwapChips(DirectionType.Top);

            else if (newMousePos.y - _startDragMousePos.y < -_dragSens)
                SwapChips(DirectionType.Bot);

            else if (newMousePos.x - _startDragMousePos.x < -_dragSens)
                SwapChips(DirectionType.Left);

            else if (newMousePos.x - _startDragMousePos.x > _dragSens)
                SwapChips(DirectionType.Right);

        }

        private void SwapChips(DirectionType direction)
        {
            _isReading = false;
            _secondaryCell = _primaryCell.GetNeighbour(direction);
            if (_secondaryCell.IsNull()) return;

            _secondaryChip = _secondaryCell.CurrentChip;

            if (_primaryCell.IsNull() || _secondaryCell.IsNull() ||
                _primaryCell.CurrentChip.IsNull() ||
                _secondaryCell.CurrentChip.IsNull())
                return;

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