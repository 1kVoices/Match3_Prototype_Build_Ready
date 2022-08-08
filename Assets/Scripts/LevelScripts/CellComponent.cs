using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


namespace Match3
{
    public class CellComponent : MonoBehaviour
    {
        [SerializeField]
        private ChipComponent[] _chipPrefabs;
        public ChipComponent Chip;
        private Dictionary<DirectionType, CellComponent> _neighbours;

        private int _layer;
        public event Action<ChipComponent> PoolingEvent;

        private void Start()
        {
            _layer = LayerMask.GetMask($"Level");
            _neighbours = new Dictionary<DirectionType, CellComponent>();
            FindNeighbours();
            GenerateChip();
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

        private async void GenerateChip()
        {
            // var allowedChips = _chipPrefabs.Where(z => !_neighbours
            //                                             .Where(x => x.Value.Chip != null)
            //                                             .Select(c => c.Value.Chip.Type)
            //                                             .Distinct()
            //                                             .Contains(z.Type)).ToArray();
            //
            // ChipComponent randomChip = UnityEngine.Random.value >= 0.5f
            //     ? _chipPrefabs[UnityEngine.Random.Range(0, _chipPrefabs.Length)]
            //     : allowedChips[UnityEngine.Random.Range(0, allowedChips.Length)];

            Chip = Instantiate(_chipPrefabs[UnityEngine.Random.Range(0, _chipPrefabs.Length)], transform);

            await Task.Yield();

            var vertical = _neighbours.Where(z => z.Key == DirectionType.Bot || z.Key == DirectionType.Top).ToArray();
            var horizontal = _neighbours.Where(z => z.Key == DirectionType.Left || z.Key == DirectionType.Right).ToArray();

            while (vertical.All(z => z.Value.Chip.Type == Chip.Type) || horizontal.All(z => z.Value.Chip.Type == Chip.Type))
            {
                print($"{name } + ");
                Chip.gameObject.SetActive(false);
                PoolingEvent?.Invoke(Chip);

                var newAllowedChips = _chipPrefabs.Where(z => !horizontal
                                                               .Union(vertical)
                                                               .Distinct()
                                                               .Select(x => x.Value.Chip.Type)
                                                               .Contains(z.Type)).ToArray();

                Chip = Instantiate(newAllowedChips[UnityEngine.Random.Range(0, newAllowedChips.Length)], transform);
            }
        }
    }
}