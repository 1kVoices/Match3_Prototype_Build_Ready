using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public void Pooling(LinkedList<CellComponent> poolingList)
        {
            if (poolingList.Count == 0) return;

            foreach (CellComponent cellComponent in poolingList)
            {
                if (poolingList.Count >= 4)
                    print(poolingList.First(z => z.IsMatch) + " " + poolingList.Count);

                StartCoroutine(cellComponent.ChipFadingRoutine());
            }
            // var splittedList = poolingList.Split();

            // foreach (var list in splittedList)
            // {
            //     if (list.Count == 4)
            //     {
            //         if (list.Any(z => Math.Abs(z.transform.position.y - list.First().transform.position.y) > 0)) //vertical check
            //         {
            //             LevelManager.Singleton.SetSpecialChip(list.First(z => z.IsMatch), MatchType.Vertical4);
            //         }
            //         else
            //         {
            //             LevelManager.Singleton.SetSpecialChip(list.First(z => z.IsMatch), MatchType.Horizontal4);
            //         }
            //     }
            //     else if (list.Count >= 5)
            //     {
            //         if (list.PosXIdentical() || list.PosYIdentical())
            //         {
            //             LevelManager.Singleton.SetSpecialChip(list.First(z => z.IsMatch), MatchType.Match5);
            //         }
            //         else if (!list.PosXIdentical())
            //         {
            //             LevelManager.Singleton.SetSpecialChip(list.First(z => z.IsMatch), MatchType.T_type);
            //         }
            //     }
            //
            //     foreach (CellComponent cellComponent in list.OrderBy(z => z.transform.position.y))
            //     {
            //         StartCoroutine(cellComponent.ChipFadingRoutine());
            //     }
            // }

            poolingList.Clear();
        }
    }
}