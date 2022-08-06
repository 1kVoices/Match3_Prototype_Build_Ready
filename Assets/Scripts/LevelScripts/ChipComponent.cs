using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Match3
{
    public class ChipComponent : MonoBehaviour
    {
        [SerializeField]
        private ChipType _type;
        [SerializeField] //todo убрать
        private Transform _child;
        private Animator _animator;

        private void Start()
        {
            _child = transform.GetChild(0);
            _animator = GetComponentInChildren<Animator>();
        }
        public void Animate(string trigger)
        {
            _animator.SetTrigger(trigger);
        }

        public async void OnAnimationEnd()
        {
            _animator.enabled = false;
            await Task.Yield();
            transform.position = _child.transform.position;
            _child.transform.localPosition = Vector3.zero;
            _animator.enabled = true;
        }
    }
}