using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Match3
{
    public class Cell : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] //todo
        public StandardChip CurrentChip; //{ get; private set; }
        public StandardChip PreviousChip { get; private set; }
        public StandardChip TemporaryChip { get; private set; }
        private SpecialChip _currentSpecial;
        private int _cellsLayer;
        private int _spawnsLayer;
        public Cell Top { get; private set; }
        public Cell TopTop { get; private set; }
        public Cell Bot { get; private set; }
        public Cell BotBot { get; private set; }
        public Cell Left { get; private set; }
        public Cell LeftLeft { get; private set; }
        public Cell Right { get; private set; }
        public Cell RightRight { get; private set; }
        private List<Cell> _poolingNeighbours;
        private bool IsMatch { get; set; }
        private Cell _pulledBy;
        private CellHighlighter _highlighter;
        public SpawnPoint SpawnPoint { get; private set; }
        public event Action<Cell,Vector2> PointerDownEvent;
        public event Action<Cell> PointerUpEvent;

        private void Start()
        {
            _poolingNeighbours = new List<Cell>();
            _highlighter = GetComponentInChildren<CellHighlighter>();
            _cellsLayer = LayerMask.GetMask("Level");
            _spawnsLayer = LayerMask.GetMask("Spawn");
            FindNeighbours();
            StartCoroutine(GenerateChipRoutine());
        }

        public void HighlightCell()
        {
            _highlighter.Activate();
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
            // #region Horizontal
            // if (this.CompareChips(Left) && this.CompareChips(Right))
            // {
            //     //00_00
            //     if (this.CompareChips(LeftLeft) && this.CompareChips(RightRight)) Pulling(LeftLeft, RightRight);
            //     //00_0
            //     if (this.CompareChips(LeftLeft)) Pulling(LeftLeft);
            //     //0_00
            //     if (this.CompareChips(RightRight)) Pulling(RightRight);
            //     //0_0
            //     Pulling(Left, Right);
            // }
            // //00_
            // if (this.CompareChips(Left) && this.CompareChips(LeftLeft)) Pulling(Left, LeftLeft);
            // //_00
            // if (this.CompareChips(Right) && this.CompareChips(RightRight)) Pulling(Right, RightRight);
            // #endregion
            //
            // #region Vertical
            // if (this.CompareChips(Top) && this.CompareChips(Bot)) //top is left
            // {
            //     //00_00
            //     if (this.CompareChips(TopTop) && this.CompareChips(BotBot)) Pulling(TopTop, BotBot);
            //     //00_0
            //     if (this.CompareChips(TopTop)) Pulling(TopTop);
            //     //0_00
            //     if (this.CompareChips(BotBot)) Pulling(BotBot);
            //     //0_0
            //     Pulling(Top, Bot);
            // }
            // //00_
            // if (this.CompareChips(Top) && this.CompareChips(TopTop)) Pulling(Top, TopTop);
            // //_00
            // if (this.CompareChips(Bot) && this.CompareChips(BotBot)) Pulling(Bot, BotBot);
            // #endregion
            Extensions.FindMatches(_poolingNeighbours, this);

            Pulling(_poolingNeighbours.ToArray());
            IsMatch = _poolingNeighbours.Count > 1;
        }

        public void ChipMoved()
        {
            if (CurrentChip.IsNull()) return;
            switch (IsMatch)
            {
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

        private void Pulling(Cell[] cells)
        {
            if (!_poolingNeighbours.Contains(this))
                _poolingNeighbours.Add(this);

            foreach (Cell cell in cells)
            {
                if (cell.IsNull() || cell.CurrentChip.IsNull()) continue;

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
            if (CurrentChip.IsNull()) return;

            CurrentChip.FadeOut(_pulledBy.NotNull()
                ? _pulledBy.CurrentChip.NotNull()
                    ? _pulledBy.CurrentChip.GetComponent<SpecialChip>()
                    : _pulledBy.PreviousChip.NotNull()
                        ? _pulledBy.PreviousChip.GetComponent<SpecialChip>()
                        : null
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

            if (topRay.collider.NotNull()) Top = topRay.collider.GetComponent<Cell>();
            if (botRay.collider.NotNull()) Bot = botRay.collider.GetComponent<Cell>();
            if (leftRay.collider.NotNull()) Left = leftRay.collider.GetComponent<Cell>();
            if (rightRay.collider.NotNull()) Right = rightRay.collider.GetComponent<Cell>();
            if (spawnRay.collider.NotNull()) SpawnPoint = spawnRay.collider.GetComponent<SpawnPoint>();

            StartCoroutine(FindExtraNeighbours());
        }

        private IEnumerator FindExtraNeighbours()
        {
            yield return null;
            TopTop = Top.NotNull()
                ? Top.Top
                : null;
            BotBot = Bot.NotNull()
                ? Bot.Bot
                : null;
            LeftLeft = Left.NotNull()
                ? Left.Left
                : null;
            RightRight = Right.NotNull()
                ? Right.Right
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

            if ((!Extensions.CompareChips(this, Top) || !Extensions.CompareChips(this, Bot)) &&
                (!Extensions.CompareChips(this, Left) || !Extensions.CompareChips(this, Right)))
                yield break;

            Pool.Singleton.Pull(CurrentChip);

            Cell[] neighbours = { Top, Bot, Left, Right };

            var allowedChips = LevelManager.Singleton.ChipPrefabs
                .Where(z => !neighbours.Where(x => x.NotNull())
                .Select(c => c.CurrentChip.Type)
                .Contains(z.Type)).ToArray();

            ChipInstance(allowedChips);
        }

        public void OnPointerDown(PointerEventData eventData) => PointerDownEvent?.Invoke(this, eventData.position);
        public void OnPointerUp(PointerEventData eventData) => PointerUpEvent?.Invoke(this);
        public void SetCurrentChip(StandardChip newChip) => CurrentChip = newChip;
        private void SetPreviousChip(StandardChip newChip) => PreviousChip = newChip;
        public void SetTemporaryChip(StandardChip newChip) => TemporaryChip = newChip;
        public void SetPulledByCell(Cell cell) => _pulledBy = cell;

        private void ChipInstance(IReadOnlyList<StandardChip> array)
        {
            SetCurrentChip(Instantiate(array[UnityEngine.Random.Range(0, array.Count-LevelManager.Singleton.REMOVE)], transform));
            CurrentChip.SetCurrentCell(this);
        }
    }
}