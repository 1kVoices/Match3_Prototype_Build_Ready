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
        public bool IsMatched { get; set; }

        private void Start()
        {
            _layer = LayerMask.GetMask($"Level");
            Neighbours = FindNeighbours();
            StartCoroutine(GenerateChipRoutine());
        }

        private void Pooling(ChipComponent chip, CellComponent cell)
        {
            cell.Chip = null;
            chip.gameObject.SetActive(false);
            PoolingEvent?.Invoke(chip);
        }

        public void IsMatch()
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
            if (left != null &&
                left.Chip != null &&
                left.Chip.Type == Chip.Type &&
                right != null &&
                right.Chip != null &&
                right.Chip.Type == Chip.Type)
            {
                //00_00
                if (leftLeft != null &&
                    leftLeft.Chip != null &&
                    leftLeft.Chip.Type == Chip.Type &&
                    rightRight != null &&
                    rightRight.Chip!= null &&
                    rightRight.Chip.Type == Chip.Type)
                {
                    Pooling(leftLeft.Chip, leftLeft);
                    Pooling(rightRight.Chip, rightRight);
                }
                //00_0
                else if (leftLeft != null &&
                    leftLeft.Chip != null &&
                    leftLeft.Chip.Type == Chip.Type) Pooling(leftLeft.Chip, leftLeft);
                //0_00
                else if (rightRight != null &&
                    rightRight.Chip != null &&
                    rightRight.Chip.Type == Chip.Type) Pooling(rightRight.Chip, rightRight);
                //0_0
                Pooling(Chip, this);
                Pooling(left.Chip, left);
                Pooling(right.Chip, right);

                IsMatched = true;
            }
            //00_
            else if (left != null &&
                left.Chip != null &&
                left.Chip.Type == Chip.Type &&
                leftLeft != null &&
                leftLeft.Chip != null &&
                leftLeft.Chip.Type == Chip.Type)
            {
                Pooling(Chip, this);
                Pooling(left.Chip, left);
                Pooling(leftLeft.Chip, leftLeft);
                IsMatched = true;
            }
            //_00
            else if (right != null &&
                right.Chip != null &&
                right.Chip.Type == Chip.Type &&
                rightRight != null &&
                rightRight.Chip != null &&
                rightRight.Chip.Type == Chip.Type)
            {
                Pooling(Chip, this);
                Pooling(right.Chip, right);
                Pooling(rightRight.Chip, rightRight);
                IsMatched = true;
            }
            #endregion
            #region Vertical
            else if (top != null &&
                top.Chip != null &&
                top.Chip.Type == Chip.Type &&
                bot != null &&
                bot.Chip != null &&
                bot.Chip.Type == Chip.Type)
            {
                //00_00
                if (topTop != null &&
                    topTop.Chip != null &&
                    topTop.Chip.Type == Chip.Type &&
                    botBot != null &&
                    botBot.Chip != null &&
                    botBot.Chip.Type == Chip.Type)
                {
                    Pooling(topTop.Chip, topTop);
                    Pooling(botBot.Chip, botBot);
                }
                //00_0
                else if (topTop != null &&
                    topTop.Chip != null &&
                    topTop.Chip.Type == Chip.Type) Pooling(topTop.Chip, topTop);
                //0_00
                else if (botBot != null &&
                    botBot.Chip != null &&
                    botBot.Chip.Type == Chip.Type) Pooling(botBot.Chip, botBot);
                //0_0
                Pooling(Chip, this);
                Pooling(top.Chip, top);
                Pooling(bot.Chip, bot);

                IsMatched = true;
            }
            //00_
            else if (top != null &&
                top.Chip != null &&
                top.Chip.Type == Chip.Type &&
                topTop != null &&
                topTop.Chip != null &&
                topTop.Chip.Type == Chip.Type)
            {
                Pooling(Chip, this);
                Pooling(top.Chip, top);
                Pooling(topTop.Chip, topTop);
                IsMatched = true;
            }
            //_00
            else if (bot != null &&
                bot.Chip != null &&
                bot.Chip.Type == Chip.Type &&
                botBot != null &&
                botBot.Chip != null &&
                botBot.Chip.Type == Chip.Type)
            {
                Pooling(Chip, this);
                Pooling(bot.Chip, bot);
                Pooling(botBot.Chip, botBot);
                IsMatched = true;
            }
            #endregion
            else IsMatched =Neighbours.Any(z => z.Value.IsMatched);

            StartCoroutine(DiscardIsMatched());
        }

        private IEnumerator DiscardIsMatched()
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
                Pooling(Chip, this);

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
            if (Chip.IsInteractable) PointerDownEvent?.Invoke(this, eventData.position);
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            if (Chip.IsInteractable) PointerUpEvent?.Invoke(this);
        }
    }
}