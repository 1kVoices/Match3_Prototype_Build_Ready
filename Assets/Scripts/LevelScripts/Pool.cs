using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
    public class Pool : MonoBehaviour
    {
        public static Pool Singleton;
        public LinkedList<StandardChip> ChipPool;
        // public LinkedList<CellComponent> PoolingList;

        private void Start()
        {
            if (!Singleton) Singleton = this;
            else Destroy(gameObject);

            ChipPool = new LinkedList<StandardChip>();
            // PoolingList = new LinkedList<CellComponent>();
        }

        public void Pull(StandardChip standardChip)
        {
            standardChip.transform.parent = transform;
            ChipPool.AddLast(standardChip);
        }
    }
}