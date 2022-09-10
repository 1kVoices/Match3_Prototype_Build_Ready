using UnityEngine;

namespace Match3
{
    public class SpawnPointComponent : MonoBehaviour
    {
        public void GenerateChip(CellComponent callerCell)
        {
            ChipComponent[] prefabs = LevelManager.Singleton.ChipPrefabs;
            ChipComponent newChip = Instantiate(prefabs[UnityEngine.Random.Range(0, prefabs.Length-LevelManager.Singleton.REMOVE)], transform);
            newChip.FastShowUp();
            newChip.Transfer(callerCell);

            // LinkedList<ChipComponent> pool = Pool.Singleton.ChipPool;
            //
            // if (pool.Count == 0) return;
            // ChipComponent chip = pool.ElementAt(UnityEngine.Random.Range(0, pool.Count));
            // pool.Remove(chip);
            // chip.transform.parent = transform;
            // chip.transform.position = transform.position;
            // chip.FastShowUp();
            // chip.ReservedBy = callerCell;
            // chip.Transfer(callerCell);
        }

    }
}