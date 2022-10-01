using System;
using UnityEngine;

namespace Match3
{
    public class PauseButton : MonoBehaviour
    {
        public event Action PauseEvent;

        private void OpenMenu()
        {
            PauseEvent?.Invoke();
        }
    }
}