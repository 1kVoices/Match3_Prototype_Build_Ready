using System.Collections.Generic;
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
                foreach (CellComponent cell in cells)
                {
                    cell.Chip.FadeOut();
                    // cell.Kidnapping();
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