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

        private void Start()
        {
            _layer = LayerMask.GetMask($"Level");
            Neighbours = FindNeighbours();
            StartCoroutine(GenerateChipRoutine());
        }

        public bool IsMatch()
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

            if (left != null && left.Chip.Type == Chip.Type && right != null && right.Chip.Type == Chip.Type)
            {
                print($"{name} {Chip.name}");

                Chip.gameObject.SetActive(false);
                left.Chip.gameObject.SetActive(false);
                right.Chip.gameObject.SetActive(false);

                return true;
            }
            if (top != null && top.Chip.Type == Chip.Type && bot != null && bot.Chip.Type == Chip.Type)
            {
                print($"{name} {Chip.name}");

                Chip.gameObject.SetActive(false);
                top.Chip.gameObject.SetActive(false);
                bot.Chip.gameObject.SetActive(false);

                return true;
            }
            if (left != null && left.Chip.Type == Chip.Type && leftLeft != null && leftLeft.Chip.Type == Chip.Type)
            {
                print($"{name} {Chip.name}");

                Chip.gameObject.SetActive(false);
                left.Chip.gameObject.SetActive(false);
                leftLeft.Chip.gameObject.SetActive(false);

                return true;
            }
            if (right != null && right.Chip.Type == Chip.Type && rightRight != null && rightRight.Chip.Type == Chip.Type)
            {
                print($"{name} {Chip.name}");

                Chip.gameObject.SetActive(false);
                right.Chip.gameObject.SetActive(false);
                rightRight.Chip.gameObject.SetActive(false);

                return true;
            }
            if (top != null && top.Chip.Type == Chip.Type && topTop != null && topTop.Chip.Type == Chip.Type)
            {
                print($"{name} {Chip.name}");

                Chip.gameObject.SetActive(false);
                top.Chip.gameObject.SetActive(false);
                topTop.Chip.gameObject.SetActive(false);

                return true;
            }
            if (bot != null && bot.Chip.Type == Chip.Type && botBot != null && botBot.Chip.Type == Chip.Type)
            {
                print($"{name} {Chip.name}");

                Chip.gameObject.SetActive(false);
                bot.Chip.gameObject.SetActive(false);
                botBot.Chip.gameObject.SetActive(false);

                return true;
            }

            return false;
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
                Chip.gameObject.SetActive(false);
                Chip.transform.parent = _manager.transform;
                PoolingEvent?.Invoke(Chip);

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