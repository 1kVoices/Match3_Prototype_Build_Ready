using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Match3
{
    public class ChipChildComponent : MonoBehaviour
    {
        [SerializeField]
        private ChipComponent _parent;

        public event Action<ChipComponent> OnAnimationEndEvent;

        public async void OnAnimationEnd()
        {
            await Task.Yield();
            transform.parent.position = transform.position;
            transform.localPosition = Vector3.zero;
            OnAnimationEndEvent?.Invoke(_parent);
        }
    }
}