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

        private void Start()
        {
            // _chip._child.OnAnimationEndEvent += ChildOnOnAnimationEndEvent;
        }
        private static void ChildOnOnAnimationEndEvent(ChipComponent obj)
        {

        }
    }
}