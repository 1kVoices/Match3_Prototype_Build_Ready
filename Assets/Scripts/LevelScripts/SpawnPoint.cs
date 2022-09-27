using UnityEngine;

namespace Match3
{
    public class SpawnPoint : MonoBehaviour
    {
        public void GenerateChip(Cell callerCell)
        {
            StandardChip newChip = Instantiate(LevelManager.Singleton.RandomChip(), transform);
            newChip.FastShowUp();
            StartCoroutine(newChip.Transfer(callerCell, LevelManager.Singleton.ChipsFallTime));

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