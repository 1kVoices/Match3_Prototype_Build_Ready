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
        public ChipComponent Chip;
        public Dictionary<DirectionType, CellComponent> Neighbours;
        private int _layer;
        public event Action<ChipComponent> PoolingEvent;
        public event Action<CellComponent,Vector2> PointerDownEvent;
        public event Action<CellComponent> PointerUpEvent;
        public bool IsMatched { get; private set; }

        private void Start()
        {
            _layer = LayerMask.GetMask($"Level");
            Neighbours = FindNeighbours();
            StartCoroutine(GenerateChipRoutine());
        }

        private void Pooling(params CellComponent[] cells)
        {
            foreach (CellComponent cell in cells)
            {
                cell.Chip.gameObject.SetActive(false);
                cell.Chip = null;
                PoolingEvent?.Invoke(cell.Chip);
            }
        }

        private bool CompareChips(CellComponent comparativeCell)
        {
            return Chip != null && comparativeCell != null && comparativeCell.Chip != null && comparativeCell.Chip.Type == Chip.Type;
        }

        public void CheckMatches()
        {
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
                if (CompareChips(leftLeft) && CompareChips(rightRight)) Pooling(leftLeft, rightRight);
                //00_0
                else if (CompareChips(leftLeft))
                {
                    Pooling(leftLeft);
                }
                //0_00
                else if (CompareChips(rightRight))
                {
                    Pooling(rightRight);
                }

                //0_0
                Pooling(this, left, right);

                IsMatched = true;
            }
            //00_
            else if (CompareChips(left) && CompareChips(leftLeft))
            {
                Pooling(this, left, leftLeft);
                IsMatched = true;
            }
            //_00
            else if (CompareChips(right) && CompareChips(rightRight))
            {
                Pooling(this, right, rightRight);
                IsMatched = true;
            }
            #endregion
            #region Vertical
            else if (CompareChips(top) && CompareChips(bot))
            {
                //00_00
                if (CompareChips(topTop) && CompareChips(botBot))
                {
                    Pooling(topTop,botBot);
                }
                //00_0
                else if (CompareChips(topTop))
                {
                    Pooling(topTop);
                }
                //0_00
                else if (CompareChips(botBot))
                {
                    Pooling(botBot);
                }
                //0_0
                Pooling(this, top, bot);

                IsMatched = true;
            }
            //00_
            else if (CompareChips(top) && CompareChips(topTop))
            {
                Pooling(this, top, topTop);
                IsMatched = true;
            }
            //_00
            else if (CompareChips(bot) && CompareChips(botBot))
            {
                Pooling(this, bot, botBot);
                IsMatched = true;
            }
            #endregion
            else IsMatched = Neighbours.Any(z => z.Value.IsMatched);

            StartCoroutine(DiscardIsMatchedRoutine());
        }

        private IEnumerator DiscardIsMatchedRoutine()
        {
            yield return null;
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
            Chip = Instantiate(_chipPrefabs[UnityEngine.Random.Range(0, _chipPrefabs.Length)], transform);

            yield return null;

            var vertical = Neighbours.Where(z => z.Key == DirectionType.Bot || z.Key == DirectionType.Top).ToArray();
            var horizontal = Neighbours.Where(z => z.Key == DirectionType.Left || z.Key == DirectionType.Right).ToArray();

            while (vertical.All(z => z.Value.Chip.Type == Chip.Type) || horizontal.All(z => z.Value.Chip.Type == Chip.Type))
            {
                Chip.transform.parent = _manager.transform;
                Pooling(this);

                var allowedChips = _chipPrefabs.Where(z => !horizontal
                                                               .Union(vertical)
                                                               .Distinct()
                                                               .Select(x => x.Value.Chip.Type)
                                                               .Contains(z.Type)).ToArray();

                Chip = Instantiate(allowedChips[UnityEngine.Random.Range(0, allowedChips.Length)], transform);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // if (Chip.IsInteractable)
                PointerDownEvent?.Invoke(this, eventData.position);
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            // if (Chip.IsInteractable)
                PointerUpEvent?.Invoke(this);
        }
    }
}