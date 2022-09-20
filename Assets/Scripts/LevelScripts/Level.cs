using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
    public class Level : MonoBehaviour
    {
        [SerializeField]
        private ChipType _targetType;
        [SerializeField, Range(10,100)]
        private int _chipsToDestroy;
        [SerializeField,Range(0,4)]
        private int _chipsCount;
        [SerializeField, Range(5, 15), Tooltip("Minutes")]
        private float _levelTime;
        [SerializeField]
        private Line[] _levelLayout;

        public int ChipsToDestroy => _chipsToDestroy;
        public int ChipsCount => _chipsCount;
        public ChipType TargetType => _targetType;
        public float LevelTime => _levelTime;

        public IEnumerable<Line> LevelLayout => _levelLayout;
    }
}