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
        public ChipComponent[] _chipPrefabs;
        public ChipComponent Chip;
        private ChipComponent _previousChip;
        private Dictionary<DirectionType, CellComponent> Neighbours;
        private LinkedList<CellComponent> _poolingList;
        private int _cellsLayer;
        private int _spawnsLayer;
        private SpawnPointComponent _spawnPoint;
        public event Action<CellComponent,Vector2> PointerDownEvent;
        public event Action<CellComponent> PointerUpEvent;
        public bool IsMatch { get; private set; }

        private void Start()
        {
            _poolingList = new LinkedList<CellComponent>();
            _cellsLayer = LayerMask.GetMask("Level");
            _spawnsLayer = LayerMask.GetMask("Spawn");
            Neighbours = FindNeighbours();
            StartCoroutine(GenerateChipRoutine());
        }

        private bool CompareChips(CellComponent comparativeCell)
        {
            return comparativeCell != null &&
                   comparativeCell.Chip != null &&
                   comparativeCell.Chip != Chip &&
                   comparativeCell.Chip.Type == Chip.Type;
        }

        private void Pulling(params CellComponent[] cells)
        {
            if (!_poolingList.Contains(this)) _poolingList.AddLast(this);

            foreach (CellComponent cellComponent in cells)
            {
                if (_poolingList.Contains(cellComponent)) continue;
                _poolingList.AddLast(cellComponent);
            }
        }

        public void CheckMatches(ChipComponent newChip)
        {
            _previousChip = Chip;
            Chip = newChip;
            Chip.transform.parent = transform;
            IsMatch = false;
            _poolingList.Clear();
            CellComponent top = GetNeighbour(DirectionType.Top);
            CellComponent topTop = top != null
                ? top.GetNeighbour(DirectionType.Top)
                : null;
            CellComponent bot = GetNeighbour(DirectionType.Bot);
            CellComponent botBot = bot != null
                ? bot.GetNeighbour(DirectionType.Bot)
                : null;
            CellComponent left = GetNeighbour(DirectionType.Left);
            CellComponent leftLeft = left != null
                ? left.GetNeighbour(DirectionType.Left)
                : null;
            CellComponent right = GetNeighbour(DirectionType.Right);
            CellComponent rightRight = right != null
                ? right.GetNeighbour(DirectionType.Right)
                : null;

            #region Horizontal
            if (CompareChips(left) && CompareChips(right))
            {
                //00_00
                if (CompareChips(leftLeft) && CompareChips(rightRight)) Pulling(leftLeft, rightRight);
                //00_0
                if (CompareChips(leftLeft)) Pulling(leftLeft);
                //0_00
                if (CompareChips(rightRight)) Pulling(rightRight);
                //0_0
                Pulling(left, right);
            }
            //00_
            if (CompareChips(left) && CompareChips(leftLeft)) Pulling(left, leftLeft);
            //_00
            if (CompareChips(right) && CompareChips(rightRight)) Pulling(right, rightRight);
            #endregion

            #region Vertical
            if (CompareChips(top) && CompareChips(bot)) //top is left
            {
                //00_00
                if (CompareChips(topTop) && CompareChips(botBot)) Pulling(topTop, botBot);
                //00_0
                if (CompareChips(topTop)) Pulling(topTop);
                //0_00
                if (CompareChips(botBot)) Pulling(botBot);
                //0_0
                Pulling(top, bot);
            }
            //00_
            if (CompareChips(top) && CompareChips(topTop)) Pulling(top, topTop);
            //_00
            if (CompareChips(bot) && CompareChips(botBot)) Pulling(bot, botBot);
            #endregion

            if (_poolingList.Count != 0)
            {
                IsMatch = true;
                StartCoroutine(MatchRoutine());
            }
            else
            {
                IsMatch = false;
                StartCoroutine(NoMatchRoutine());
            }
        }

        private IEnumerator MatchRoutine()
        {
            yield return new WaitWhile(() => _poolingList.Any(z => z.Chip.IsAnimating));
            Chip.SetCurrentCell(this);
            Pool.Singleton.Pooling(_poolingList);
        }

        private IEnumerator NoMatchRoutine()
        {
            yield return new WaitWhile(() => Chip.IsAnimating);
            if (_previousChip.GetMatchState() || IsMatch) yield break;
            Chip.MoveBack();
            Chip = _previousChip;
        }

        // public void Kidnapping() // :)
        // {
        //     CellComponent top = GetNeighbour(DirectionType.Top);
        //     if (top != null)
        //     {
        //         top.Chip.Transfer(this, true);
        //         top.Kidnapping();
        //     }
        //     else _spawnPoint.GenerateChip(this);
        // }

        private Dictionary<DirectionType, CellComponent> FindNeighbours()
        {
            var neighbours = new Dictionary<DirectionType, CellComponent>();

            RaycastHit2D topRay = Physics2D.Raycast(transform.position, transform.up, 1f, _cellsLayer);
            RaycastHit2D botRay = Physics2D.Raycast(transform.position, -transform.up, 1f, _cellsLayer);
            RaycastHit2D leftRay = Physics2D.Raycast(transform.position, -transform.right, 1f, _cellsLayer);
            RaycastHit2D rightRay = Physics2D.Raycast(transform.position, transform.right, 1f, _cellsLayer);
            RaycastHit2D spawnRay = Physics2D.Raycast(transform.position, transform.up, 10f, _spawnsLayer);

            if (topRay.collider != null) neighbours.Add(DirectionType.Top, topRay.collider.GetComponent<CellComponent>());
            if (botRay.collider != null) neighbours.Add(DirectionType.Bot, botRay.collider.GetComponent<CellComponent>());
            if (leftRay.collider != null) neighbours.Add(DirectionType.Left, leftRay.collider.GetComponent<CellComponent>());
            if (rightRay.collider != null) neighbours.Add(DirectionType.Right, rightRay.collider.GetComponent<CellComponent>());
            if (spawnRay.collider != null) _spawnPoint = spawnRay.collider.GetComponent<SpawnPointComponent>();

            return neighbours;
        }

        private IEnumerator GenerateChipRoutine()
        {
            ChipInstance(_chipPrefabs);
            yield return null;

            var vertical = Neighbours.Where(z => z.Key == DirectionType.Bot || z.Key == DirectionType.Top).ToArray();
            var horizontal = Neighbours.Where(z => z.Key == DirectionType.Left || z.Key == DirectionType.Right).ToArray();

            if (vertical.Any(z => !CompareChips(z.Value)) && horizontal.Any(z => !CompareChips(z.Value))) yield break;

            Pool.Singleton.Pull(Chip);

            var allowedChips = _chipPrefabs.Where(z => !horizontal.Union(vertical)
                                                                  .Distinct()
                                                                  .Select(x => x.Value.Chip.Type)
                                                                  .Contains(z.Type)).ToArray();

            ChipInstance(allowedChips);

        }

        private void ChipInstance(IReadOnlyList<ChipComponent> array)
        {
            Chip = Instantiate(array[UnityEngine.Random.Range(0, array.Count)], transform);
            Chip.SetCurrentCell(this);
        }

        public CellComponent GetNeighbour(DirectionType direction)
        {
            return Neighbours.ContainsKey(direction)
                ? Neighbours[direction]
                : null;
        }

        public void OnPointerDown(PointerEventData eventData) => PointerDownEvent?.Invoke(this, eventData.position);
        public void OnPointerUp(PointerEventData eventData) => PointerUpEvent?.Invoke(this);
    }
}