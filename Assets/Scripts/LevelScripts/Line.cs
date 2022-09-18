using UnityEngine;

namespace Match3
{
    [System.Serializable]
    public struct Line
    {
        [SerializeField]
        private Cell[] _lineCells;
        public Cell[] LineCells => _lineCells;
    }
}