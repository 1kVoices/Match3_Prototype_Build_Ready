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
        private Level _level;
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
        private float _dragSens;
        private Controls _controls;
        private Vector2 _startDragMousePos;
        private StandardChip _primaryChip;
        private StandardChip _secondaryChip;
        private Cell _primaryCell;
        private Cell _secondaryCell;
        private bool _isReading;
        private bool _allowInput;
        private bool _toolActive;
        private bool _isLoop;
        private Coroutine _destroyingRoutine;
        private LinkedList<Cell> _fadingCells;
        private Dictionary<Cell, SpecialChip> _cellsToSpawnSpecial;
        public static LevelManager Singleton;
        private LinkedList<Cell> _allCells;
        public StandardChip[] ChipPrefabs => _chipPrefabs;
        public float ChipsFallTime => _chipsFallTime;
        public int REMOVE;
        public event Action<Cell> OnPlayerClick;

        private void Start()
        {
            Singleton = this;
            _fadingCells = new LinkedList<Cell>();
            _allCells = new LinkedList<Cell>();
            _cellsToSpawnSpecial = new Dictionary<Cell, SpecialChip>();
            _controls = new Controls();
            _controls.Enable();
            foreach (Line line in _level.LevelLayout)
            {
                foreach (Cell cell in line.LineCells)
                {
                    cell.PointerDownEvent += OnCellPointerDownEvent;
                    cell.PointerUpEvent += OnCellPointerUpEvent;
                }
            }
            StartCoroutine(ChipsShowUp());
        }

        private IEnumerator ChipsShowUp() //todo
        {
            yield return new WaitForSeconds(0.3f);
            foreach (Line line in _level.LevelLayout)
            {
                foreach (Cell cell in line.LineCells)
                {
                    cell.CurrentChip.ShowUp();
                    yield return null;
                }
            }
            yield return new WaitUntil(() => _level.LevelLayout.All(z => z.LineCells.All(x => x.CurrentChip.IsInteractable)));
            SetInputState(true);
        }

        private void GetNewChip()
        {
            foreach (Line line in _level.LevelLayout)
            {
                foreach (Cell cell in line.LineCells)
                {
                    if (cell.HasChip()) continue;
                    Cell topNeighbour = cell.GetNeighbour(DirectionType.Top);
                    if (topNeighbour.IsNull())
                        cell.SpawnPoint.GenerateChip(cell);

                    while (topNeighbour.NotNull())
                    {
                        if (topNeighbour.HasChip())
                        {
                            topNeighbour.TransferChip(cell);
                            break;
                        }
                        if (!topNeighbour.HasChip())
                            topNeighbour = topNeighbour.GetNeighbour(DirectionType.Top);

                        if (topNeighbour.IsNull())
                            cell.SpawnPoint.GenerateChip(cell);
                    }
                }
            }
        }

        public void DestroyChips(Cell sender, params Cell[] cells)
        {
            SetInputState(false);
            SpecialChip specialChip = null;
            if (sender.NotNull())
                specialChip = sender.CurrentChip.GetComponent<SpecialChip>();
            if (specialChip.IsNull() && sender.NotNull())
            {
                if (cells.Length == 4)
                {
                    _cellsToSpawnSpecial.Add(sender, cells.PosYIdentical()
                        ? _specialBlasterV
                        : _specialBlasterH);
                }
                else if (cells.Length >= 5)
                {
                    if (cells.PosXIdentical() || cells.PosYIdentical())
                        _cellsToSpawnSpecial.Add(sender, _specialSun);
                    else
                        _cellsToSpawnSpecial.Add(sender, _specialM18);
                }
            }

            foreach (Cell cell in cells.Where(z => z.NotNull()))
            {
                cell.SetPulledByCell(sender);
                _fadingCells.AddLast(cell);
                if (cell.CurrentChip.Type != ChipType.None) continue;

                SpecialChip special = cell.CurrentChip as SpecialChip;
                special.Action();
            }

            if (_destroyingRoutine.IsNull())
                _destroyingRoutine = StartCoroutine(WaitForDestroy());
        }

        private IEnumerator WaitForDestroy()
        {
            yield return new WaitForSeconds(0.05f);

            foreach (Cell cell in _fadingCells.Where(z => z.CurrentChip.NotNull()))
                cell.ChipFade();

            foreach (var pair in _cellsToSpawnSpecial)
                StartCoroutine(pair.Key.SetSpecialChip(pair.Value));

            yield return new WaitWhile(() => _fadingCells
                .Where(z => z.PreviousChip.NotNull())
                .Any(x => x.PreviousChip.IsAnimating));

            _cellsToSpawnSpecial.Clear();
            _fadingCells.Clear();

            GetNewChip();
            yield return new WaitUntil(() => AllCells().All(z => z.CurrentChip.NotNull()));
            SetInputState(true);
            _destroyingRoutine = null;
        }

        /// <summary>
        /// В прототипе есть редкие и крайне редкие события, инпут пользователя в которых
        /// мог привести к ошибкам, и в связи с этим он блокируется.
        /// </summary>
        public void SetInputState(bool state) => _allowInput = state;
        public void SetToolState(bool state) => _toolActive = state;
        private void OnCellPointerUpEvent(Cell cell) => _isReading = false;
        private void OnCellPointerDownEvent(Cell cell, Vector2 cellPos)
        {
            if (!_allowInput) return;
            _primaryCell = cell;
            OnPlayerClick?.Invoke(_primaryCell);
            _primaryChip = cell.CurrentChip;
            _startDragMousePos = cellPos;
            _isReading = true;
        }

        private void Update() => ReadPlayerInput();
        private void ReadPlayerInput()
        {
            if (!_isReading || _toolActive) return;
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

        public IEnumerable<Cell> AllCells()
        {
            if (_allCells.Count > 0) return _allCells;
            foreach (Line line in _level.LevelLayout)
            {
                foreach (Cell cell in line.LineCells)
                {
                    _allCells.AddLast(cell);
                }
            }
            return _allCells;
        }

        private void OnDestroy()
        {
            _controls.Dispose();
            foreach (Line line in _level.LevelLayout)
            {
                foreach (Cell cell in line.LineCells)
                {
                    cell.PointerDownEvent -= OnCellPointerDownEvent;
                    cell.PointerUpEvent -= OnCellPointerUpEvent;
                }
            }
        }
    }
}