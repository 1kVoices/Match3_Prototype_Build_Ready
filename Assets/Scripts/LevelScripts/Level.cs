using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
    public class Level : MonoBehaviour
    {
        [SerializeField]
        private ChipType _allowedTypes;
        [SerializeField, Range(100, 500)]
        private int _rewardExp;
        [SerializeField]
        private ChipType _targetType;
        [SerializeField, Range(10,300)]
        private int _chipsToDestroy;
        [SerializeField, Range(5, 15), Tooltip("Minutes")]
        private float _levelTime;
        [SerializeField]
        private Line[] _levelLayout;

        public int ChipsToDestroy => _chipsToDestroy;
        public ChipType TargetType => _targetType;
        public float LevelTime => _levelTime;
        public int RewardExp => _rewardExp;
        public IEnumerable<Line> LevelLayout => _levelLayout;

        public IReadOnlyCollection<ChipType> GetChipTypes()
        {
            LinkedList<ChipType> list = new LinkedList<ChipType>();
            Array values = Enum.GetValues(typeof(ChipType));

            foreach (var value in values)
            {
                if ((ChipType)value == ChipType.None) continue;
                if (_allowedTypes.HasFlag((ChipType)value))
                    list.AddLast((ChipType)value);
            }
            return list;
        }
    }
}