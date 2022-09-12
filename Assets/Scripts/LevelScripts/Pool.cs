using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
    public class Pool : MonoBehaviour
    {
        public static Pool Singleton;
        public LinkedList<ChipComponent> ChipPool;
        // public LinkedList<CellComponent> PoolingList;

        private void Start()
        {
            if (!Singleton) Singleton = this;
            else Destroy(gameObject);

            ChipPool = new LinkedList<ChipComponent>();
            // PoolingList = new LinkedList<CellComponent>();
        }

        public void Pull(ChipComponent chip)
        {
            chip.transform.parent = transform;
            ChipPool.AddLast(chip);
        }
    }
}