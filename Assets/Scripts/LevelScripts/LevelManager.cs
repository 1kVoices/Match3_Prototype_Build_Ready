using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Match3
{
    public class LevelManager : MonoBehaviour
    {
        private int _currentLevel;

        [SerializeField]
        private CellComponent[] _cells;
        private LinkedList<ChipComponent> _chipPool;
        //todo убрать
        public int Count;

        private void Start()
        {
            _chipPool = new LinkedList<ChipComponent>();

            foreach (var cell in _cells)
            {
                cell.PoolingEvent += OnPoolingEvent;
            }
        }
        private void OnPoolingEvent(ChipComponent chip)
        {
            _chipPool.AddLast(chip);
            Count = _chipPool.Count();
        }
    }
}