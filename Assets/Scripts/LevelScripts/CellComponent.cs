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
        private void Start()
        {
            _layer = LayerMask.GetMask($"Level");
            _neighbours = new Dictionary<DirectionType, CellComponent>();
            FindNeighbours();
            GenerateChip();
        }
        private void Update()
        {
            Debug.DrawRay(transform.position, transform.up * 1f, Color.cyan);
            Debug.DrawRay(transform.position, -transform.up * 1f, Color.cyan);
            Debug.DrawRay(transform.position, -transform.right * 1f, Color.cyan);
            Debug.DrawRay(transform.position, transform.right * 1f, Color.cyan);
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
            await Task.Yield();
            var allowedChips = _chipPrefabs.Where(z => !_neighbours
                                               .Where(x => x.Value.Chip != null)
                                               .Select(c => c.Value.Chip._type)
                                               .Distinct()
                                               .Contains(z._type)).ToArray();
            Chip = Instantiate(allowedChips[Random.Range(0, allowedChips.Length)], transform);
        }
    }
}