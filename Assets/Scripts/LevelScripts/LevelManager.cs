using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Match3
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField, Range(0.1f, 0.7f)]
        private float _chipsFallTime;
        [SerializeField]
        private StandardChip[] _chipPrefabs;
        [SerializeField]
        private SpecialChip _specialM18;
        [SerializeField]
        private SpecialChip _specialBlasterH;
        [SerializeField]
        private SpecialChip _specialBlasterV;
        [SerializeField]
        private SpecialChip _specialSun;
        private int _currentLevel;
        [SerializeField]
        private Cell[] _cells;
        [SerializeField]
        private float _dragSens;
        private Controls _controls;
        private Vector2 _startDragMousePos;
        private StandardChip _primaryChip;
        private StandardChip _secondaryChip;
        private Cell _primaryCell;
        private Cell _secondaryCell;
        private bool _isReading;
        private bool _allowInput;
        private List<Cell> _waitingCells;
        private List<Cell> _extraWaitingCells;
        public static LevelManager Singleton;
        public IEnumerable<Cell> AllCells => _cells;
        public StandardChip[] ChipPrefabs => _chipPrefabs;
        public float ChipsFallTime => _chipsFallTime;
        public int REMOVE;

        private void Start()
        {
            Singleton = this;
            _waitingCells = new List<Cell>();
            _extraWaitingCells = new List<Cell>();
            _controls = new Controls();
            _controls.Enable();
            foreach (Cell cell in _cells)
            {
                cell.PointerDownEvent += OnCellPointerDownEvent;
                cell.PointerUpEvent += OnCellPointerUpEvent;
            }

            StartCoroutine(ChipsShowUp());
        }

        private IEnumerator ChipsShowUp() //todo
        {
            yield return new WaitForSeconds(0.3f);
            foreach (Cell cell in _cells)
            {
                cell.CurrentChip.ShowUp();
                yield return new WaitForSeconds(0.01f);
            }
        }

        public void GetNewChip()
        {
            var ordered = _waitingCells.OrderBy(z => z.transform.position.y).Distinct().ToList();
            _waitingCells = ordered;
            while (_waitingCells.Count > 0)
            {
                Cell callerCell = _waitingCells.RemoveFirst();
                if (callerCell.HasChip()) continue;

                Cell topNeighbour = callerCell.GetNeighbour(DirectionType.Top);
                if (topNeighbour.IsNull())
                    callerCell.SpawnPoint.GenerateChip(callerCell);

                while (topNeighbour.NotNull())
                {
                    if (topNeighbour.HasChip())
                    {
                        if (topNeighbour.CurrentChip.IsAnimating || !topNeighbour.CurrentChip.IsInteractable)
                        {
                            _extraWaitingCells.Enqueue(callerCell);
                            break;
                        }
                        if (topNeighbour.CurrentChip.ReservedBy.IsNull())
                        {
                            topNeighbour.TransferChip(callerCell);
                            _waitingCells.Enqueue(topNeighbour);
                            break;
                        }
                    }
                    if (topNeighbour.HasChip() && topNeighbour.CurrentChip.ReservedBy.NotNull() || !topNeighbour.HasChip())
                        topNeighbour = topNeighbour.GetNeighbour(DirectionType.Top);

                    if (topNeighbour.IsNull())
                        callerCell.SpawnPoint.GenerateChip(callerCell);
                }
            }
            _waitingCells.AddRange(_extraWaitingCells);
            _extraWaitingCells.Clear();
        }

        public void DestroyChips(List<Cell> list, Cell sender)
        {
            SpecialChip specialChip = sender.CurrentChip.GetComponent<SpecialChip>();
            if (specialChip.IsNull())
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
                    else
                        SetSpecialChip(sender, MatchType.T_type);
                }
            }

            foreach (Cell cell in list.OrderBy(z => z.transform.position.y))
            {
                if (!cell.HasChip()) continue;

                if (cell.CurrentChip.Type == ChipType.None && cell.CurrentChip != sender.CurrentChip)
                {
                    SpecialChip special = cell.CurrentChip as SpecialChip;
                    special.Action();
                    continue;
                }

                cell.ChipFade(specialChip);
                _waitingCells.Add(cell);
            }
        }

        private void SetSpecialChip(Cell cell, MatchType type)
        {
            switch (type)
            {
                case MatchType.Horizontal4:
                    StartCoroutine(cell.SetSpecialChip(_specialBlasterH));
                    break;
                case MatchType.Vertical4:
                    StartCoroutine(cell.SetSpecialChip(_specialBlasterV));
                    break;
                case MatchType.T_type:
                    StartCoroutine(cell.SetSpecialChip(_specialM18));
                    break;
                case MatchType.Match5:
                    StartCoroutine(cell.SetSpecialChip(_specialSun));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public void SetInputState(bool state) => _allowInput = state;
        private void OnCellPointerUpEvent(Cell cell) => _isReading = false;
        private void OnCellPointerDownEvent(Cell cell, Vector2 cellPos)
        {
            // if (!_allowInput) return;
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

            if (_primaryCell.IsNull() || _secondaryCell.IsNull() || !_primaryCell.HasChip() || !_secondaryCell.HasChip())
                return;

            if (!_primaryChip.IsInteractable || !_secondaryChip.IsInteractable) return;

            _primaryChip.Move(direction, true, _secondaryCell);
            _secondaryChip.Move(direction.OppositeDirection(), false, _primaryCell);
        }

        private void OnDestroy()
        {
            _controls.Dispose();
            foreach (Cell cell in _cells)
            {
                cell.PointerDownEvent -= OnCellPointerDownEvent;
                cell.PointerUpEvent -= OnCellPointerUpEvent;
            }
        }
    }
}