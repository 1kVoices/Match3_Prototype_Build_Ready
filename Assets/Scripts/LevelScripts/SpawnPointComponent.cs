using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Match3
{
    public class SpawnPointComponent : MonoBehaviour
    {
        public void GenerateChip(CellComponent callerCell)
        {
            print($"Generator{callerCell}");
            LinkedList<ChipComponent> pool = Pool.Singleton.ChipPool;

            if (pool.Count == 0) return;
            ChipComponent chip = pool.ElementAt(UnityEngine.Random.Range(0, pool.Count));
            pool.Remove(chip);
            chip.transform.parent = transform; //todo
            chip.transform.position = transform.position;
            chip.FastShowUp();
            // chip.Transfer(callerCell, false);
        }
    }
}