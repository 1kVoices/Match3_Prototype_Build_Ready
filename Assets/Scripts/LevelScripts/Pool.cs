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
        public LinkedList<CellComponent> PoolingList;

        private void Start()
        {
            if (!Singleton) Singleton = this;
            else Destroy(gameObject);

            ChipPool = new LinkedList<ChipComponent>();
            PoolingList = new LinkedList<CellComponent>();
        }

        public void Pull(ChipComponent chip)
        {
            chip.transform.parent = transform;
            ChipPool.AddLast(chip);
        }

        public IEnumerator Pooling()
        {
            if (PoolingList.Count == 0) yield break;

            foreach (var list in PoolingList.Split())
            {
                if (list.Count == 4)
                {
                    if (list.Any(z => Math.Abs(z.transform.position.y - list.First().transform.position.y) > 0)) //vertical check
                    {
                        print($"{list.First(z => z.IsMatch)} vertical4");
                    }
                    else
                    {
                        print($"{list.First(z => z.IsMatch)} horizontal4");
                    }
                }
                else if (list.Count >= 5)
                {
                    var listYPoses = list.Select(z => z.transform.position.y).Distinct().Count();

                    if (list.PosXIdentical() || list.PosYIdentical())
                    {
                        print($"{list.First(z => z.IsMatch)} match5");
                    }
                    else if(listYPoses == 3)
                    {
                        if (list.Count() == 7) print($"{list.First(z => z.IsMatch)} match5(2)");
                        else print($"{list.First(z => z.IsMatch)} T type");
                    }
                    else if (listYPoses == 4)
                    {
                        print($"{list.First(z => z.IsMatch)} T type");
                    }
                    else if (listYPoses == 5)
                    {
                        print($"{list.First(z => z.IsMatch)} match5(2)");
                    }
                }

                foreach (CellComponent cellComponent in list.OrderBy(z => z.transform.position.x).ThenBy(z => z.transform.position.y))
                {
                    yield return new WaitForSeconds(0.05f);
                    StartCoroutine(cellComponent.ChipFadingRoutine());
                }
            }

            PoolingList.Clear();
        }
    }
}