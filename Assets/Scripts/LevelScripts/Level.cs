using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
    public class Level : MonoBehaviour
    {
        [SerializeField]
        private Line[] _levelLayout;
        public IEnumerable<Line> LevelLayout => _levelLayout;
    }
}