using System;
using System.Collections;
using UnityEngine;

namespace Match3
{
    public class LevelManager : MonoBehaviour
    {
        private int _currentLevel;

        [SerializeField]
        private ChipComponent[] _chip;
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(1f);
            foreach (var chip in _chip)
            {
                chip.Animate("top");
            }

            yield return new WaitForSeconds(1f);
            foreach (var chip in _chip)
            {
                chip.Animate("left");
            }

            yield return new WaitForSeconds(1f);
            foreach (var chip in _chip)
            {
                chip.Animate("bot");
            }

            yield return new WaitForSeconds(1f);
            foreach (var chip in _chip)
            {
                chip.Animate("right");
            }
        }
    }
}