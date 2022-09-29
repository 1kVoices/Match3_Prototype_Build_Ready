using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Match3
{
    public class BlackScreen : MonoBehaviour
    {
        public event Action OnScreenDarken;

        private void ScreenDarken()
        {
            Scene scene = SceneManager.GetActiveScene();
            if (scene.buildIndex == 1)
                OnScreenDarken?.Invoke();
            else
                MenuEvents.OnBlackScreenDarken();
        }

        private void ScreenBleached()
        {
            Scene scene = SceneManager.GetActiveScene();
            if (scene.buildIndex == 0)
                MenuEvents.OnBlackScreenBleached();
        }
    }
}