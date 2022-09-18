using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Match3
{
    public class Cell : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        public StandardChip CurrentChip; //{ get; private set; }
        public StandardChip PreviousChip { get; private set; }
        private SpecialChip _currentSpecial;
        private int _cellsLayer;
        private int _spawnsLayer;
        private Cell _top;
        private Cell _topTop;
        private Cell _bot;
        private Cell _botBot;
        private Cell _left;
        private Cell _leftLeft;
        private Cell _right;
        private Cell _rightRight;
        private List<Cell> _poolingNeighbours;
        private bool IsMatch { get; set; }
        private Cell _pulledBy;
        public SpawnPoint SpawnPoint { get; private set; }
        public event Action<Cell,Vector2> PointerDownEvent;
        public event Action<Cell> PointerUpEvent;

        private void Start()
        {
            _poolingNeighbours = new List<Cell>();
            _cellsLayer = LayerMask.GetMask("Level");
            _spawnsLayer = LayerMask.GetMask("Spawn");
            FindNeighbours();
            StartCoroutine(GenerateChipRoutine());
        }

        public void CheckMatches(StandardChip newChip, bool isChipTransferred)
        {
            IsMatch = false;
            _poolingNeighbours.Clear();
            SetPreviousChip(CurrentChip);
            SetCurrentChip(newChip);
            if (newChip.Type == ChipType.None && !isChipTransferred)
            {
                _currentSpecial = CurrentChip.GetComponent<SpecialChip>();
                IsMatch = true;
                return;
            }

            #region Horizontal
            if (CompareChips(_left) && CompareChips(_right))
            {
                //00_00
                if (CompareChips(_leftLeft) && CompareChips(_rightRight)) Pulling(_leftLeft, _rightRight);
                //00_0
                if (CompareChips(_leftLeft)) Pulling(_leftLeft);
                //0_00
                if (CompareChips(_rightRight)) Pulling(_rightRight);
                //0_0
                Pulling(_left, _right);
            }
            //00_
            if (CompareChips(_left) && CompareChips(_leftLeft)) Pulling(_left, _leftLeft);
            //_00
            if (CompareChips(_right) && CompareChips(_rightRight)) Pulling(_right, _rightRight);
            #endregion

            #region Vertical
            if (CompareChips(_top) && CompareChips(_bot)) //top is left
            {
                //00_00
                if (CompareChips(_topTop) && CompareChips(_botBot)) Pulling(_topTop, _botBot);
                //00_0
                if (CompareChips(_topTop)) Pulling(_topTop);
                //0_00
                if (CompareChips(_botBot)) Pulling(_botBot);
                //0_0
                Pulling(_top, _bot);
            }
            //00_
            if (CompareChips(_top) && CompareChips(_topTop)) Pulling(_top, _topTop);
            //_00
            if (CompareChips(_bot) && CompareChips(_botBot)) Pulling(_bot, _botBot);
            #endregion

            IsMatch = _poolingNeighbours.Count > 0;
        }

        public void ChipMoved()
        {
            if (CurrentChip.IsNull()) return;
            SpecialChip special = null;
            if (PreviousChip.NotNull())
                special = PreviousChip.GetComponent<SpecialChip>();
            switch (IsMatch)
            {
                case true when _currentSpecial.NotNull() && PreviousChip.NotNull()
                                                        && special.NotNull() && special.SpecialType == SpecialChipType.Sun:
                    _currentSpecial = null;
                    break;
                case true when _currentSpecial.NotNull():
                    _currentSpecial.Action();
                    SetPreviousChip(CurrentChip);
                    SetCurrentChip(null);
                    _currentSpecial = null;
                    break;
                case true:
                    CurrentChip.SetPreviousCell(null);
                    Pooling();
                    break;
                case false when CurrentChip.PreviousCell.NotNull() && CurrentChip.PreviousCell.IsMatch == false:
                    CurrentChip.SendBack();
                    SetCurrentChip(PreviousChip);
                    SetPreviousChip(null);
                    break;
                case false:
                    CurrentChip.SetPreviousCell(null);
                    CurrentChip.SetInteractionState(true);
                    break;
            }
        }

        private void Pulling(params Cell[] cells)
        {
            if (!_poolingNeighbours.Contains(this))
                _poolingNeighbours.Add(this);

            foreach (Cell cell in cells)
            {
                if (cell.IsNull() || cell.CurrentChip.IsNull()) continue;
                if (_poolingNeighbours.Contains(cell)) continue;
                _poolingNeighbours.Add(cell);

                cell.CurrentChip.SetInteractionState(false);

                if (cell._pulledBy.IsNull())
                    cell.SetPulledByCell(this);

                else if (cell._pulledBy.NotNull())
                {
                    _poolingNeighbours.AddRange(cell._pulledBy._poolingNeighbours);

                    foreach (Cell pulledCell in _poolingNeighbours)
                        pulledCell.SetPulledByCell(this);
                }
            }
        }

        private void Pooling()
        {
            if (CurrentChip.IsNull()) return;
            if (CurrentChip.Type != ChipType.None)
            {
                if (_pulledBy.NotNull() && _pulledBy != this)
                {
                    _pulledBy._poolingNeighbours.AddRange(_poolingNeighbours);
                    _poolingNeighbours.Clear();
                    return;
                }
            }

            var cleared = _poolingNeighbours.Distinct();
            _poolingNeighbours = cleared.ToList();
            LevelManager.Singleton.DestroyChips(this, _poolingNeighbours.ToArray());
        }

        public void TransferChip(Cell callerCell)
        {
            StartCoroutine(CurrentChip.Transfer(callerCell, LevelManager.Singleton.ChipsFallTime));
            SetCurrentChip(null);
        }

        public void ChipFade()
        {
            CurrentChip.FadeOut(_pulledBy.CurrentChip.NotNull()
                ? _pulledBy.CurrentChip.GetComponent<SpecialChip>()
                : _pulledBy.PreviousChip.NotNull()
                    ? _pulledBy.PreviousChip.GetComponent<SpecialChip>()
                    : null);

            SetPreviousChip(CurrentChip);
            SetCurrentChip(null);
        }

        public IEnumerator SetSpecialChip(SpecialChip specialChip)
        {
            yield return null;
            SetCurrentChip(Instantiate(specialChip, transform));
            CurrentChip.SetCurrentCell(this);
            CurrentChip.ShowUp();
        }

        private void FindNeighbours()
        {
            RaycastHit2D topRay = Physics2D.Raycast(transform.position, transform.up, 1f, _cellsLayer);
            RaycastHit2D botRay = Physics2D.Raycast(transform.position, -transform.up, 1f, _cellsLayer);
            RaycastHit2D leftRay = Physics2D.Raycast(transform.position, -transform.right, 1f, _cellsLayer);
            RaycastHit2D rightRay = Physics2D.Raycast(transform.position, transform.right, 1f, _cellsLayer);
            RaycastHit2D spawnRay = Physics2D.Raycast(transform.position, transform.up, 10f, _spawnsLayer);

            if (topRay.collider.NotNull()) _top = topRay.collider.GetComponent<Cell>();
            if (botRay.collider.NotNull()) _bot = botRay.collider.GetComponent<Cell>();
            if (leftRay.collider.NotNull()) _left = leftRay.collider.GetComponent<Cell>();
            if (rightRay.collider.NotNull()) _right = rightRay.collider.GetComponent<Cell>();
            if (spawnRay.collider.NotNull()) SpawnPoint = spawnRay.collider.GetComponent<SpawnPoint>();

            StartCoroutine(FindExtraNeighbours());
        }

        private IEnumerator FindExtraNeighbours()
        {
            yield return null;
            _topTop = _top.NotNull()
                ? _top.GetNeighbour(DirectionType.Top)
                : null;
            _botBot = _bot.NotNull()
                ? _bot.GetNeighbour(DirectionType.Bot)
                : null;
            _leftLeft = _left.NotNull()
                ? _left.GetNeighbour(DirectionType.Left)
                : null;
            _rightRight = _right.NotNull()
                ? _right.GetNeighbour(DirectionType.Right)
                : null;
        }

        private IEnumerator GenerateChipRoutine()
        {
            if (CurrentChip.NotNull()) //todo
            {
                CurrentChip = Instantiate(CurrentChip, transform);
                CurrentChip.SetCurrentCell(this);
                yield break;
            }

            ChipInstance(LevelManager.Singleton.ChipPrefabs);
            yield return null;

            if ((!CompareChips(_top) || !CompareChips(_bot)) && (!CompareChips(_left) || !CompareChips(_right))) yield break;
            Pool.Singleton.Pull(CurrentChip);

            Cell[] neighbours = { _top, _bot, _left, _right };

            var allowedChips = LevelManager.Singleton.ChipPrefabs
                .Where(z => !neighbours.Where(x => x.NotNull())
                .Select(c => c.CurrentChip.Type)
                .Contains(z.Type)).ToArray();

            ChipInstance(allowedChips);
        }

        [CanBeNull]
        public Cell GetNeighbour(DirectionType direction)
        {
            switch (direction)
            {
                case DirectionType.Top:
                    return _top;
                case DirectionType.Bot:
                    return _bot;
                case DirectionType.Left:
                    return _left;
                case DirectionType.Right:
                    return _right;
                default: return null;
            }
        }

        public void OnPointerDown(PointerEventData eventData) => PointerDownEvent?.Invoke(this, eventData.position);
        public void OnPointerUp(PointerEventData eventData) => PointerUpEvent?.Invoke(this);
        private void SetCurrentChip(StandardChip newChip) => CurrentChip = newChip;
        private void SetPreviousChip(StandardChip newChip) => PreviousChip = newChip;
        public void SetPulledByCell(Cell cell) => _pulledBy = cell;

        private void ChipInstance(IReadOnlyList<StandardChip> array)
        {
            SetCurrentChip(Instantiate(array[UnityEngine.Random.Range(0, array.Count-LevelManager.Singleton.REMOVE)], transform));
            CurrentChip.SetCurrentCell(this);
        }

        private bool CompareChips(Cell comparativeCell)
        {
            return comparativeCell.NotNull() &&
                   comparativeCell.CurrentChip.NotNull() &&
                   comparativeCell.CurrentChip != CurrentChip &&
                   comparativeCell.CurrentChip.Type != ChipType.None &&
                   comparativeCell.CurrentChip.Type == CurrentChip.Type;
        }
    }
}