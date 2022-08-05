using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Match3
{
    public class ChipComponent : MonoBehaviour
    {
        [SerializeField]
        private ChipType _type;
        [SerializeField]
        private Animator _animator;

        private void Start()
        {
            _animator = GetComponentInChildren<Animator>();
        }
        public void Animate(string trigger)
        {
            _animator.SetTrigger(trigger);
        }

        protected async void OnAnimationEnd()
        {
            _animator.enabled = false;
            await Task.Yield();
            transform.parent.position = transform.position;
            transform.localPosition = Vector3.zero;
            _animator.enabled = true;
        }
    }
}