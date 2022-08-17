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
        public ChipComponent Chip;//todo
        public Dictionary<DirectionType, CellComponent> Neighbours { get; private set; }
        public bool IsMatch { get; private set; }
        private int _cellsLayer;
        private int _spawnsLayer;
        private LinkedList<CellComponent> _poolingList;
        private SpawnPointComponent _spawnPoint;
        private LinkedList<ChipComponent> _actualPool;
        public event Action<CellComponent> PoolingCellEvent;
        public event Action<CellComponent,Vector2> PointerDownEvent;
        public event Action<CellComponent> PointerUpEvent;

        private void Start()
        {
            _cellsLayer = LayerMask.GetMask("Level");
            _spawnsLayer = LayerMask.GetMask("Spawn");
            _poolingList = new LinkedList<CellComponent>();
            Neighbours = FindNeighbours();
            StartCoroutine(GenerateChipRoutine());
        }

        public void Kidnapping(LinkedList<ChipComponent> pool) // :)
        {
            _actualPool = pool;
            StartCoroutine(SpawnChipRoutine());
        }

        private IEnumerator SpawnChipRoutine()
        {
            _spawnPoint.GenerateChip(this, _actualPool);
            yield return null;
        }

        private void Pooling()
        {
            foreach (CellComponent cellComponent in _poolingList)
            {
                PoolingCellEvent?.Invoke(cellComponent);
                cellComponent.Chip.FadeOut();
                cellComponent.Chip = null;
            }
        }

        private void Pulling(params CellComponent[] cells)
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

        public void CheckMatch()
        {
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
                if (CompareChips(leftLeft) && CompareChips(rightRight)) Pulling(leftLeft, rightRight);
                //00_0
                if (CompareChips(leftLeft)) Pulling(leftLeft);
                //0_00
                if (CompareChips(rightRight)) Pulling(rightRight);
                //0_0
                Pulling(this, left, right);
            }
            //00_
            if (CompareChips(left) && CompareChips(leftLeft)) Pulling(this, left, leftLeft);
            //_00
            if (CompareChips(right) && CompareChips(rightRight)) Pulling(this, right, rightRight);
            #endregion

            #region Vertical
            if (CompareChips(top) && CompareChips(bot)) //top is left
            {
                //00_00
                if (CompareChips(topTop) && CompareChips(botBot)) Pulling(topTop,botBot);
                //00_0
                if (CompareChips(topTop)) Pulling(topTop);
                //0_00
                if (CompareChips(botBot)) Pulling(botBot);
                //0_0
                Pulling(this, top, bot);
            }
            //00_
            if (CompareChips(top) && CompareChips(topTop)) Pulling(this, top, topTop);
            //_00
            if (CompareChips(bot) && CompareChips(botBot)) Pulling(this, bot, botBot);
            #endregion

            Pooling();
            IsMatch = _poolingList.Count != 0;
        }

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
            Chip = Instantiate(_chipPrefabs[UnityEngine.Random.Range(0, _chipPrefabs.Length)], transform);

            yield return null;

            var vertical = Neighbours.Where(z => z.Key == DirectionType.Bot || z.Key == DirectionType.Top).ToArray();
            var horizontal = Neighbours.Where(z => z.Key == DirectionType.Left || z.Key == DirectionType.Right).ToArray();

            if (!vertical.All(z => CompareChips(z.Value)) && !horizontal.All(z => CompareChips(z.Value))) yield break;

            Pulling(this);
            Pooling();
            var allowedChips = _chipPrefabs.Where(z => !horizontal.Union(vertical)
                                                                  .Distinct()
                                                                  .Select(x => x.Value.Chip.Type)
                                                                  .Contains(z.Type)).ToArray();

            Chip = Instantiate(allowedChips[UnityEngine.Random.Range(0, allowedChips.Length)], transform);
        }

        public void OnPointerDown(PointerEventData eventData) => PointerDownEvent?.Invoke(this, eventData.position);
        public void OnPointerUp(PointerEventData eventData) => PointerUpEvent?.Invoke(this);
    }
}