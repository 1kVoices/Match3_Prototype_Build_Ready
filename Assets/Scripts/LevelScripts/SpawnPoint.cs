using System.Linq;
using UnityEngine;

namespace Match3
{
    public class SpawnPoint : MonoBehaviour
    {
        public void GenerateChip(Cell callerCell)
        {
            StandardChip newChip = LevelManager.Singleton.RandomChip();
            StandardChip pooledChip = Pool.Singleton.ChipPool.FirstOrDefault(chip => chip.Type == newChip.Type);

            if (pooledChip is null)
                newChip = Instantiate(newChip);
            else
            {
                newChip = pooledChip;
                Pool.Singleton.ChipPool.Remove(pooledChip);
            }
            newChip.transform.parent = transform;
            newChip.transform.position = transform.position;
            newChip.FastShowUp();
            StartCoroutine(newChip.Transfer(callerCell, LevelManager.Singleton.ChipsFallTime));
        }
    }
}