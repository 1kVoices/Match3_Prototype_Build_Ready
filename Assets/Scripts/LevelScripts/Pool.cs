using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Match3
{
    public class Pool : MonoBehaviour
    {
        public static Pool Singleton;
        public LinkedList<ChipComponent> ChipPool;

        private void Start()
        {
            if (!Singleton) Singleton = this;
            else Destroy(gameObject);

            ChipPool = new LinkedList<ChipComponent>();
        }

        public void Pooling(IEnumerable<CellComponent> cells, bool isInitPool = false)
        {
            if (isInitPool) foreach (CellComponent cell in cells) Pull(cell.Chip);
            else
            {
                foreach (CellComponent cell in cells.OrderBy(z => z.transform.position.y))
                {
                    cell.Chip.FadeOut();
                    cell.Kidnapping(cell.GetNeighbour(DirectionType.Top));
                }
            }
        }

        public void Pull(ChipComponent chip)
        {
            chip.transform.parent = transform;
            ChipPool.AddLast(chip);
        }
    }
}