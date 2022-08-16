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
        [SerializeField]
        private LevelManager _manager;
        private int _layer;
        private LinkedList<CellComponent> _poolingList;
        public ChipComponent Chip;
        public Dictionary<DirectionType, CellComponent> Neighbours;
        public event Action<ChipComponent> PoolingEvent;
        public event Action<CellComponent,Vector2> PointerDownEvent;
        public event Action<CellComponent> PointerUpEvent;
        public bool IsMatched { get; private set; }


        private void Start()
        {
            _layer = LayerMask.GetMask($"Level");
            _poolingList = new LinkedList<CellComponent>();
            Neighbours = FindNeighbours();
            StartCoroutine(GenerateChipRoutine());
        }

        private void Pooling(CellComponent cell = null, bool isCheckingMatches = true)
        {
            if (isCheckingMatches)
            {
                foreach (CellComponent cellComponent in _poolingList)
                {
                    cellComponent.Chip.gameObject.SetActive(false);
                    cellComponent.Chip = null;
                    PoolingEvent?.Invoke(cellComponent.Chip);
                }
            }
            else
            {
                if (cell == null) return;
                cell.Chip.gameObject.SetActive(false);
                cell.Chip = null;
                PoolingEvent?.Invoke(cell.Chip);
            }
        }

        private void PoolAdding(params CellComponent[] cells)
        {
            foreach (CellComponent cellComponent in cells)
            {
                if (_poolingList.Contains(cellComponent)) continue;
                _poolingList.AddLast(cellComponent);
            }
        }

        private bool CompareChips(CellComponent comparativeCell)
        {
            return comparativeCell != null && comparativeCell.Chip != null && comparativeCell.Chip.Type == Chip.Type;
        }

        public void CheckMatches()
        {
            IsMatched = false;
            _poolingList.Clear();
            CellComponent top = Neighbours.ContainsKey(DirectionType.Top)
                ? Neighbours[DirectionType.Top]
                : null;
            CellComponent topTop = top != null && top.Neighbours.ContainsKey(DirectionType.Top)
                ? top.Neighbours[DirectionType.Top]
                : null;
            CellComponent bot = Neighbours.ContainsKey(DirectionType.Bot)
                ? Neighbours[DirectionType.Bot]
                : null;
            CellComponent botBot = bot != null && bot.Neighbours.ContainsKey(DirectionType.Bot)
                ? bot.Neighbours[DirectionType.Bot]
                : null;
            CellComponent left = Neighbours.ContainsKey(DirectionType.Left)
                ? Neighbours[DirectionType.Left]
                : null;
            CellComponent leftLeft = left != null && left.Neighbours.ContainsKey(DirectionType.Left)
                ? left.Neighbours[DirectionType.Left]
                : null;
            CellComponent right = Neighbours.ContainsKey(DirectionType.Right)
                ? Neighbours[DirectionType.Right]
                : null;
            CellComponent rightRight = right != null && right.Neighbours.ContainsKey(DirectionType.Right)
                ? right.Neighbours[DirectionType.Right]
                : null;

            #region Horizontal
            if (CompareChips(left) && CompareChips(right))
            {
                //00_00
                if (CompareChips(leftLeft) && CompareChips(rightRight)) PoolAdding(leftLeft, rightRight);
                //00_0
                if (CompareChips(leftLeft)) PoolAdding(leftLeft);
                //0_00
                if (CompareChips(rightRight)) PoolAdding(rightRight);
                //0_0
                PoolAdding(this, left, right);
            }
            //00_
            if (CompareChips(left) && CompareChips(leftLeft)) PoolAdding(this, left, leftLeft);
            //_00
            if (CompareChips(right) && CompareChips(rightRight)) PoolAdding(this, right, rightRight);
            #endregion

            #region Vertical
            if (CompareChips(top) && CompareChips(bot))
            {
                //00_00
                if (CompareChips(topTop) && CompareChips(botBot)) PoolAdding(topTop,botBot);
                //00_0
                if (CompareChips(topTop)) PoolAdding(topTop);
                //0_00
                if (CompareChips(botBot)) PoolAdding(botBot);
                //0_0
                PoolAdding(this, top, bot);
            }
            //00_
            if (CompareChips(top) && CompareChips(topTop)) PoolAdding(this, top, topTop);
            //_00
            if (CompareChips(bot) && CompareChips(botBot)) PoolAdding(this, bot, botBot);
            #endregion

            IsMatched = _poolingList.Count != 0;

            StartCoroutine(DiscardIsMatchedRoutine());
        }

        private IEnumerator DiscardIsMatchedRoutine()
        {
            yield return null;
            Pooling();
            IsMatched = false;
        }

        private Dictionary<DirectionType, CellComponent> FindNeighbours()
        {
            var neighbours = new Dictionary<DirectionType, CellComponent>();

            RaycastHit2D topRay = Physics2D.Raycast(transform.position, transform.up, 1f, _layer);
            RaycastHit2D botRay = Physics2D.Raycast(transform.position, -transform.up, 1f, _layer);
            RaycastHit2D leftRay = Physics2D.Raycast(transform.position, -transform.right, 1f, _layer);
            RaycastHit2D rightRay = Physics2D.Raycast(transform.position, transform.right, 1f, _layer);

            if (topRay.collider != null) neighbours.Add(DirectionType.Top, topRay.collider.GetComponent<CellComponent>());
            if (botRay.collider != null) neighbours.Add(DirectionType.Bot, botRay.collider.GetComponent<CellComponent>());
            if (leftRay.collider != null) neighbours.Add(DirectionType.Left, leftRay.collider.GetComponent<CellComponent>());
            if (rightRay.collider != null) neighbours.Add(DirectionType.Right, rightRay.collider.GetComponent<CellComponent>());

            return neighbours;
        }

        private IEnumerator GenerateChipRoutine()
        {
            Chip = Instantiate(_chipPrefabs[UnityEngine.Random.Range(0, _chipPrefabs.Length-2)], transform);

            yield return null;

            var vertical = Neighbours.Where(z => z.Key == DirectionType.Bot || z.Key == DirectionType.Top).ToArray();
            var horizontal = Neighbours.Where(z => z.Key == DirectionType.Left || z.Key == DirectionType.Right).ToArray();

            while (vertical.All(z => z.Value.Chip.Type == Chip.Type) || horizontal.All(z => z.Value.Chip.Type == Chip.Type))
            {
                Chip.transform.parent = _manager.transform;
                Pooling(this, false);

                var allowedChips = _chipPrefabs.Where(z => !horizontal
                                                               .Union(vertical)
                                                               .Distinct()
                                                               .Select(x => x.Value.Chip.Type)
                                                               .Contains(z.Type)).ToArray();

                Chip = Instantiate(allowedChips[UnityEngine.Random.Range(0, allowedChips.Length-2)], transform);
            }
        }

        public void OnPointerDown(PointerEventData eventData) => PointerDownEvent?.Invoke(this, eventData.position);
        public void OnPointerUp(PointerEventData eventData) => PointerUpEvent?.Invoke(this);
    }
}