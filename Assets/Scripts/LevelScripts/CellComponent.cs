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
        public ChipComponent Chip;
        private Dictionary<DirectionType, CellComponent> _neighbours;
        private int _layer;
        public event Action<ChipComponent> PoolingEvent;
        public event Action<CellComponent,Vector2> PointerDownEvent;
        public event Action<CellComponent> PointerUpEvent;

        private void Start()
        {
            _layer = LayerMask.GetMask($"Level");
            _neighbours = new Dictionary<DirectionType, CellComponent>();
            FindNeighbours();
            StartCoroutine(GenerateChipRoutine());
        }

        private void FindNeighbours()
        {
            RaycastHit2D topRay = Physics2D.Raycast(transform.position, transform.up, 1f, _layer);
            RaycastHit2D botRay = Physics2D.Raycast(transform.position, -transform.up, 1f, _layer);
            RaycastHit2D leftRay = Physics2D.Raycast(transform.position, -transform.right, 1f, _layer);
            RaycastHit2D rightRay = Physics2D.Raycast(transform.position, transform.right, 1f, _layer);

            if (topRay.collider != null) _neighbours.Add(DirectionType.Top, topRay.collider.GetComponent<CellComponent>());
            if (botRay.collider != null) _neighbours.Add(DirectionType.Bot, botRay.collider.GetComponent<CellComponent>());
            if (leftRay.collider != null) _neighbours.Add(DirectionType.Left, leftRay.collider.GetComponent<CellComponent>());
            if (rightRay.collider != null) _neighbours.Add(DirectionType.Right, rightRay.collider.GetComponent<CellComponent>());
        }

        private IEnumerator GenerateChipRoutine()
        {
            Chip = Instantiate(_chipPrefabs[UnityEngine.Random.Range(0, _chipPrefabs.Length)], transform);

            yield return null;

            var vertical = _neighbours.Where(z => z.Key == DirectionType.Bot || z.Key == DirectionType.Top).ToArray();
            var horizontal = _neighbours.Where(z => z.Key == DirectionType.Left || z.Key == DirectionType.Right).ToArray();

            while (vertical.All(z => z.Value.Chip.Type == Chip.Type) || horizontal.All(z => z.Value.Chip.Type == Chip.Type))
            {
                Chip.gameObject.SetActive(false);
                PoolingEvent?.Invoke(Chip);

                var allowedChips = _chipPrefabs.Where(z => !horizontal
                                                               .Union(vertical)
                                                               .Distinct()
                                                               .Select(x => x.Value.Chip.Type)
                                                               .Contains(z.Type)).ToArray();

                Chip = Instantiate(allowedChips[UnityEngine.Random.Range(0, allowedChips.Length)], transform);
            }
        }

        public void OnPointerDown(PointerEventData eventData) => PointerDownEvent?.Invoke(this,eventData.position);
        public void OnPointerUp(PointerEventData eventData) => PointerUpEvent?.Invoke(this);
    }
}