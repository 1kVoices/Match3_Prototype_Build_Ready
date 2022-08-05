using System;
using System.Collections;
using UnityEngine;

namespace Match3
{
    public class LevelManager : MonoBehaviour
    {
        private int _currentLevel;

        [SerializeField]
        private ChipComponent _chip;
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(1f);
            _chip.Animate("top");

            yield return new WaitForSeconds(1f);
            _chip.Animate("left");
        }
    }
}