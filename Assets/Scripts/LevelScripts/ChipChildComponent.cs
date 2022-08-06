using System;
using UnityEngine;

namespace Match3
{
    public class ChipChildComponent : MonoBehaviour
    {
        private ChipComponent _parent;

        private void Start()
        {
            _parent = GetComponentInParent<ChipComponent>();

        }
        public void OnAnimationEnd()
        {
            _parent.OnAnimationEnd();
        }
    }
}