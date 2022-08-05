using System;
using UnityEngine;

namespace Match3
{
    public sealed class MenuEvents : MonoBehaviour
    {
        public static MenuEvents Singleton;

        public event Action BlackScreenBleached;

        public event Action PedroAskedHelp;
        private void Start()
        {
            if (!Singleton)
            {
                Singleton = this;
                DontDestroyOnLoad(gameObject);
            }
            else Destroy(gameObject);
        }

        public void OnBlackScreenBleached() => BlackScreenBleached?.Invoke();
        public void OnPedroAskedHelp() => PedroAskedHelp?.Invoke();
    }
}