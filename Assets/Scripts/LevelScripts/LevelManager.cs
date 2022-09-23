using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Match3.Data;
using UnityEngine;

namespace Match3
{
    public class LevelManager : MonoBehaviour, IData
    {
        private GameData _data;
        [SerializeField]
        private Level[] _levels;
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
        [SerializeField]
        private float _dragSens;
        [SerializeField]
        private float _hintDelay;
        private Level _level;
        private GameTime _gameTime;
        private PlayerObjective _playerObjective;
        private MatchHelper _matchHelper;
        private Controls _controls;
        private Vector2 _startDragMousePos;
        private StandardChip _primaryChip;
        private StandardChip _secondaryChip;
        private Cell _primaryCell;
        private Cell _secondaryCell;
        private bool _isReading;
        private bool _allowInput;
        private bool _toolActive;
        private bool _hintActive;
        private float _hintTime;
        private Coroutine _destroyingRoutine;
        private LinkedList<Cell> _fadingCells;
        private Dictionary<Cell, SpecialChip> _cellsToSpawnSpecial;
        private WaitForSeconds _destroyDelay;
        public static LevelManager Singleton;
        public LinkedList<Cell> AllCells { get; private set; }
        public int ChipsCount { get; private set; }
        public StandardChip[] ChipPrefabs => _chipPrefabs;
        public float ChipsFallTime => _chipsFallTime;
        public event Action<Cell> OnPlayerClick;
        public event Action<SpecialChipType> OnSpecialActivateEvent;
        public event Action OnSpawnSpecial;
        public event Action<ChipType> OnDestroyChip;
        public event Action OnPlayerInput;

        private IEnumerator Start()
        {
            Singleton = this;
            _level = _levels[_data.CurrentLevel];
            AllCells = new LinkedList<Cell>();
            _fadingCells = new LinkedList<Cell>();
            _cellsToSpawnSpecial = new Dictionary<Cell, SpecialChip>();
            _playerObjective = FindObjectOfType<PlayerObjective>();
            _gameTime = FindObjectOfType<GameTime>();
            _gameTime.OutOfTimeEvent += GameTimeIsUp;
            _hintTime = _hintDelay;
            ChipsCount = _level.RemoveChips;
            _destroyDelay = new WaitForSeconds(0.05f);
            _hintActive = true;
            _matchHelper = new MatchHelper();
            _controls = new Controls();
            _controls.Enable();

            foreach (Line line in _level.LevelLayout)
            {
                foreach (Cell cell in line.LineCells)
                {
                    cell.PointerDownEvent += OnCellPointerDownEvent;
                    cell.PointerUpEvent += OnCellPointerUpEvent;
                    cell.PointerClickEvent += OnCellPointerClickEvent;
                    AllCells.AddLast(cell);
                }
            }
            StartCoroutine(ChipsShowUp());
            yield return null;
            _gameTime.SetTimer(_level.LevelTime * 60);
        }

        private IEnumerator ChipsShowUp() //todo
        {
            yield return new WaitForSeconds(0.3f);

            _playerObjective.SetCurrentObjective(_level.TargetType, _level.ChipsToDestroy);

            foreach (Line line in _level.LevelLayout)
            {
                foreach (Cell cell in line.LineCells)
                {
                    cell.CurrentChip.ShowUp();
                    yield return null;
                }
            }
            yield return new WaitUntil(() => _level.LevelLayout.All(line => line.LineCells.All(x => x.CurrentChip.IsInteractable)));
            CheckPossibleMatches();
            SetInputState(true);

            _hintActive = false;
            _gameTime.StartTimer();
        }

        /// <summary>
        /// Метод получения новой фишки.
        /// Идет по списку всех клеток на поле и если у клетки нет фишки,
        /// то метод отправляет фишку соседа сверху этой клетке
        /// </summary>
        private void GetNewChip()
        {
            foreach (Line line in _level.LevelLayout)
            {
                foreach (Cell cell in line.LineCells)
                {
                    if (cell.HasChip()) continue;
                    Cell topNeighbour = cell.Top;

                    while (topNeighbour.NotNull())
                    {
                        if (topNeighbour.HasChip())
                        {
                            topNeighbour.TransferChip(cell);
                            break;
                        }
                        if (!topNeighbour.HasChip())
                            topNeighbour = topNeighbour.Top;
                    }

                    if (topNeighbour.IsNull())
                        cell.SpawnPoint.GenerateChip(cell);
                }
            }
        }
        /// <summary>
        /// Метод уничтожения фишек
        /// Если sender не имеет супер фишки, то он добавляется в словарь и позже получит суперфишку.
        /// Если в cells есть супер фишки - они активируются
        /// </summary>
        public void DestroyChips(Cell sender, params Cell[] cells)
        {
            CheckHint();
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

                if (cells.Length >= 4 && cells.All(cell => !cell.CurrentChip.IsTransferred))
                    OnSpawnSpecial?.Invoke();

                if (cells.All(cell => !cell.CurrentChip.IsTransferred))
                {
                    OnDestroyChip?.Invoke(cells.First().CurrentChip.Type);
                    if (cells.Length >= 4)
                        OnSpawnSpecial?.Invoke();
                }
            }

            var cleared = cells.Where(cell => cell.NotNull()).ToArray();
            cells = cleared;

            foreach (Cell cell in cells)
            {
                cell.SetPulledByCell(sender);
                _fadingCells.AddLast(cell);

                if (cell.CurrentChip.IsNull()) continue;
                if (cell.CurrentChip.Type != ChipType.None) continue;

                SpecialChip special = cell.CurrentChip as SpecialChip;
                special.Action();
            }

            if (_destroyingRoutine.IsNull())
                _destroyingRoutine = StartCoroutine(WaitForDestroy());
        }

        /// <summary>
        /// Задача этой корутины запустится один раз и неважно кто именно ее запустит
        /// Поэтому в самом начале есть оджидание наполнения списка _fadingCells
        /// Т.к метод DestroyChips уничтожит на карте все, что может уничтожится
        /// </summary>
        private IEnumerator WaitForDestroy()
        {
            yield return _destroyDelay;
            while (_fadingCells.Count > 0)
            {
                _hintTime = _hintDelay;
                foreach (Cell cell in _fadingCells.Where(cell => cell.CurrentChip.NotNull()))
                {
                    if (cell.CurrentChip.Type == _level.TargetType)
                        _playerObjective.UpdateCount();

                    cell.ChipFade();
                }

                foreach (var pair in _cellsToSpawnSpecial)
                    StartCoroutine(pair.Key.SetSpecialChip(pair.Value));

                yield return new WaitWhile(() => _fadingCells
                    .Where(cell => cell.PreviousChip.NotNull())
                    .Any(cell => cell.PreviousChip.IsAnimating));

                _cellsToSpawnSpecial.Clear();
                _fadingCells.Clear();

                GetNewChip();

                yield return new WaitUntil(() => AllCells.All(cell => cell.CurrentChip.NotNull() && !cell.CurrentChip.IsAnimating));
            }

            if (_playerObjective.CurrentCount <= 0)
            {
                print("You win");
                _data.CurrentLevel++;
            }
            _destroyingRoutine = null;
            CheckPossibleMatches();
            SetInputState(true);
        }

        private void CheckPossibleMatches()
        {
            if (_matchHelper.IsMatchPossible()) return;

            DestroyChips(null, AllCells.ToArray());
            print("No match possible");
            UnityEditor.EditorApplication.isPaused = true;
        }

        private void SuggestMatch(List<Cell> cells)
        {
            if (_hintActive) return;
            _hintActive = true;

            foreach (Cell cell in cells)
                cell.Highlight(true);
        }

        /// <summary>
        /// Если подсказка активна - метод ее выключает
        /// </summary>
        private void CheckHint()
        {
            if (!_hintActive) return;
            var highlightingCells = AllCells.Where(cell => cell.IsHighlighting);

            foreach (Cell cell in highlightingCells)
                cell.Highlight(false);

            _hintActive = false;
            _hintTime = _hintDelay;
        }

        private void HintDelay()
        {
            if (_hintActive) return;
            _hintTime -= Time.deltaTime;

            if (_hintTime <= 0)
                SuggestMatch(_matchHelper.PossibleMatches);
        }

        private void GameTimeIsUp()
        {
            CheckHint();
            _hintActive = true;
            SetInputState(false);
            Debug.Log("You lost");
        }

        /// <summary>
        /// В прототипе есть редкие и крайне редкие события, инпут пользователя в которых
        /// может привести к ошибкам, и в связи с этим он блокируется
        /// </summary>
        public bool GetInputState() => _allowInput;
        public void SetInputState(bool state) => _allowInput = state;
        public void SetToolState(bool state) => _toolActive = state;
        public void OnSpecialActivate(SpecialChipType obj) => OnSpecialActivateEvent?.Invoke(obj);
        private void OnCellPointerUpEvent(Cell cell) => _isReading = false;
        private void OnCellPointerClickEvent(Cell cell) => OnPlayerClick?.Invoke(cell);

        private void OnCellPointerDownEvent(Cell cell, Vector2 cellPos)
        {
            if (!_allowInput) return;

            _primaryCell = cell;
            _primaryChip = cell.CurrentChip;
            _startDragMousePos = cellPos;
            _isReading = true;
        }

        private void Update()
        {
            ReadPlayerInput();
            HintDelay();
        }

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
            OnPlayerInput?.Invoke();
            _secondaryCell = GetNeighbour(_primaryCell, direction);
            if (_secondaryCell.IsNull()) return;

            _secondaryChip = _secondaryCell.CurrentChip;

            if (_primaryCell.IsNull() || _secondaryCell.IsNull()) return;
            if (!_primaryCell.HasChip() || !_secondaryCell.HasChip()) return;

            if (!_primaryChip.IsInteractable || !_secondaryChip.IsInteractable) return;
            if (_primaryChip.IsAnimating || _secondaryChip.IsAnimating) return;

            _primaryChip.Move(direction, true, _secondaryCell);
            _secondaryChip.Move(direction.OppositeDirection(), false, _primaryCell);
            CheckHint();
        }

        private static Cell GetNeighbour(Cell cell, DirectionType direction)
        {
            switch (direction)
            {
                case DirectionType.Top:
                    return cell.Top;
                case DirectionType.Bot:
                    return cell.Bot;
                case DirectionType.Left:
                    return cell.Left;
                case DirectionType.Right:
                    return cell.Right;
                default: return null;
            }
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
                    cell.PointerClickEvent -= OnCellPointerClickEvent;
                }
            }
        }

        public void LoadData(GameData data)
        {
            _data = data;
        }

        public void SaveData(ref GameData data)
        {
            data.CurrentLevel = _data.CurrentLevel;
        }
    }
}