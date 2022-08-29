using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Match3
{
    public class CellComponent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private ChipComponent[] _chipPrefabs;
        public ChipComponent CurrentChip { get; private set; }
        private ChipComponent _previousChip;
        [SerializeField] //todo
        private ChipComponent _specialChip;
        private int _cellsLayer;
        private int _spawnsLayer;
        private CellComponent _top;
        private CellComponent _topTop;
        private CellComponent _bot;
        private CellComponent _botBot;
        private CellComponent _left;
        private CellComponent _leftLeft;
        private CellComponent _right;
        private CellComponent _rightRight;
        public SpawnPointComponent SpawnPoint { get; private set; }
        private MatchType _matchType;
        public event Action<CellComponent,Vector2> PointerDownEvent;
        public event Action<CellComponent> PointerUpEvent;
        public bool IsMatch { get; private set; }

        private void Start()
        {
            _cellsLayer = LayerMask.GetMask("Level");
            _spawnsLayer = LayerMask.GetMask("Spawn");
            FindNeighbours();
            StartCoroutine(GenerateChipRoutine());
        }

        public void CheckMatches(ChipComponent newChip)
        {
            SetPreviousChip(CurrentChip);
            SetCurrentChip(newChip);
            if (newChip.IsSpecial)
            {
                IsMatch = true;
                StartCoroutine(MatchRoutine());
                return;
            }
            IsMatch = false;

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

            StartCoroutine(IsMatch
                ? MatchRoutine()
                : NoMatchRoutine());
        }

        private IEnumerator MatchRoutine()
        {
            CurrentChip.CurrentCell = this;
            yield return new WaitWhile(() => Pool.Singleton.PoolingList
                                                 .Where(z => z.CurrentChip.NotNull())
                                                 .Any(x => x.CurrentChip.IsAnimating));
            StartCoroutine(Pool.Singleton.Pooling());
        }

        private IEnumerator NoMatchRoutine()
        {
            IsMatch = false;
            if (CurrentChip.IsTransferred)
            {
                CurrentChip.EndTransfer();
                yield break;
            }

            yield return new WaitWhile(() => CurrentChip.IsAnimating);
            CurrentChip.SetInteractionState(true);

            if (_previousChip.NotNull() && _previousChip.GetMatchState() || IsMatch)
                yield break;

            StartCoroutine(CurrentChip.MoveBackRoutine());
            SetCurrentChip(_previousChip);
        }

        // private IEnumerator KidnappingRoutine(CellComponent topNeighbour)
        // {
        //     while (true)
        //     {
        //         if (topNeighbour.IsNull())
        //             _spawnPoint.GenerateChip(this);
        //
        //         else if (topNeighbour.CurrentChip.NotNull() && topNeighbour.CurrentChip.ReservedBy.NotNull() || topNeighbour.CurrentChip.IsNull())
        //         {
        //             topNeighbour = topNeighbour.GetNeighbour(DirectionType.Top);
        //             continue;
        //         }
        //         break;
        //     }
        //
        //     if (topNeighbour.IsNull()) yield break;
        //     topNeighbour.CurrentChip.ReservedBy = this;
        //     topNeighbour.CurrentChip.Transfer(this);
        //     yield return null;
        //     StartCoroutine(TopNeighbourKidnappingRoutine(topNeighbour));
        // }

        // private IEnumerator TopNeighbourKidnappingRoutine(CellComponent cell)
        // {
        //     if (cell.NotNull())
        //         yield return StartCoroutine(cell.KidnappingRoutine(cell._top));
        // }

        private void Pulling(params CellComponent[] cells)
        {
            IsMatch = true;
            if (!Pool.Singleton.PoolingList.Contains(this)) Pool.Singleton.PoolingList.AddLast(this);

            foreach (CellComponent cellComponent in cells)
            {
                if (Pool.Singleton.PoolingList.Contains(cellComponent)) continue;
                Pool.Singleton.PoolingList.AddLast(cellComponent);
                cellComponent.CurrentChip.SetInteractionState(false);
            }
        }

        public IEnumerator ChipFadingRoutine()
        {
            StartCoroutine(DisableUpperChips());
            CurrentChip.FadeOut();
            SetPreviousChip(CurrentChip);
            yield return new WaitWhile(() => _previousChip.IsAnimating);

            if (_specialChip.NotNull())
            {
                _specialChip.ShowUp();
                SetCurrentChip(_specialChip);
                yield break;
            }

            if (CurrentChip.NotNull()) yield break;
            LevelManager.GetNewChip(this);
            // StartCoroutine(KidnappingRoutine(_top));
        }

        private IEnumerator DisableUpperChips()
        {
            yield return null;
            CellComponent currentCell = this;

            while (currentCell._top.NotNull())
            {
                if (currentCell._top.CurrentChip.NotNull())
                    currentCell._top.CurrentChip.SetInteractionState(false);
                if (currentCell._top._previousChip.NotNull())
                    currentCell._top._previousChip.SetInteractionState(false);

                currentCell = currentCell._top;
            }
        }

        private bool CompareChips(CellComponent comparativeCell)
        {
            return comparativeCell.NotNull() &&
                   comparativeCell.CurrentChip.NotNull() &&
                   comparativeCell.CurrentChip != CurrentChip &&
                   comparativeCell.CurrentChip.Type == CurrentChip.Type;
        }

        public void SetCurrentChip(ChipComponent newChip) => CurrentChip = newChip;
        public void SetPreviousChip(ChipComponent newChip) => _previousChip = newChip;
        public void SetSpecialChip(ChipComponent newChip) => _specialChip = newChip;

        private void FindNeighbours()
        {
            RaycastHit2D topRay = Physics2D.Raycast(transform.position, transform.up, 1f, _cellsLayer);
            RaycastHit2D botRay = Physics2D.Raycast(transform.position, -transform.up, 1f, _cellsLayer);
            RaycastHit2D leftRay = Physics2D.Raycast(transform.position, -transform.right, 1f, _cellsLayer);
            RaycastHit2D rightRay = Physics2D.Raycast(transform.position, transform.right, 1f, _cellsLayer);
            RaycastHit2D spawnRay = Physics2D.Raycast(transform.position, transform.up, 10f, _spawnsLayer);

            if (topRay.collider.NotNull()) _top = topRay.collider.GetComponent<CellComponent>();
            if (botRay.collider.NotNull()) _bot = botRay.collider.GetComponent<CellComponent>();
            if (leftRay.collider.NotNull()) _left = leftRay.collider.GetComponent<CellComponent>();
            if (rightRay.collider.NotNull()) _right = rightRay.collider.GetComponent<CellComponent>();
            if (spawnRay.collider.NotNull()) SpawnPoint = spawnRay.collider.GetComponent<SpawnPointComponent>();

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
            if (_specialChip.NotNull())//todo
            {
                SetCurrentChip(Instantiate(_specialChip, transform));
                CurrentChip.CurrentCell = this;
                yield break;
            }
            ChipInstance(_chipPrefabs);
            yield return null;

            if ((!CompareChips(_top) || !CompareChips(_bot)) && (!CompareChips(_left) || !CompareChips(_right))) yield break;
            Pool.Singleton.Pull(CurrentChip);

            CellComponent[] neighbours = { _top, _bot, _left, _right };
            var allowedChips = _chipPrefabs.Where(z => !neighbours.Where(x => x.NotNull())
                                                                  .Select(c => c.CurrentChip.Type)
                                                                  .Contains(z.Type)).ToArray();

            ChipInstance(allowedChips);
        }

        private void ChipInstance(IReadOnlyList<ChipComponent> array)
        {
            SetCurrentChip(Instantiate(array[UnityEngine.Random.Range(0, array.Count)], transform));
            CurrentChip.CurrentCell = this;
        }

        public CellComponent GetNeighbour(DirectionType direction)
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
    }
}