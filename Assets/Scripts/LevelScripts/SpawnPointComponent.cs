using System.Collections;
using UnityEngine;

namespace Match3
{
    public class SpawnPointComponent : MonoBehaviour
    {
        [SerializeField]
        private ChipComponent[] _chipPrefabs;

        public void GenerateChip(CellComponent callerCell)
        {
            ChipComponent CurrentChip = Instantiate(_chipPrefabs[UnityEngine.Random.Range(0, _chipPrefabs.Length)], transform);
            CurrentChip.FastShowUp();
            CurrentChip.ReservedBy = callerCell;
            CurrentChip.Transfer(callerCell);

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