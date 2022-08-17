using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Match3
{
    public class SpawnPointComponent : MonoBehaviour
    {
        private LinkedList<ChipComponent> _chipPool;
        private ChipComponent _chip;

        private void Start()
        {
            _chipPool = new LinkedList<ChipComponent>();
        }

        public void GenerateChip(CellComponent callerCell, LinkedList<ChipComponent> list)
        {
            _chipPool = list;

            if (_chipPool.Count != 0)
            {
                _chip = _chipPool.ElementAt(UnityEngine.Random.Range(0, _chipPool.Count));
                list.Remove(_chip);
                _chip.transform.parent = transform; //todo
                _chip.transform.position = transform.position;
                _chip.FastShowUp();
                StartCoroutine(ChipTransferRoutine(_chip, callerCell, 1f));
                _chip = null;
            }
            else
            {

            }
        }

        private IEnumerator ChipTransferRoutine(ChipComponent chip, CellComponent cell, float transferTime)
        {
            float time = 0f;
            Vector3 start = chip.transform.position;
            Vector3 target = cell.transform.position;
            cell.Chip = chip;

            while (time < 1f)
            {
                chip.transform.position = Vector3.Lerp(start, target, time * time);
                time += Time.deltaTime / transferTime;
                yield return null;
            }
            chip.transform.position = target;
            cell.CheckMatch();
        }
    }
}