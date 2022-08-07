using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Match3
{
    public class ChipComponent : MonoBehaviour
    {
        public ChipType _type;
        [SerializeField]
        private Animator _animator;
        public ChipChildComponent _child;

        public void Animate(string trigger)
        {
            _animator.SetTrigger(trigger);
        }
    }
}