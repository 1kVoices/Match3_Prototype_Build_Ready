using System;
using UnityEngine;

namespace Match3
{
    public class GameEvents : MonoBehaviour
    {
        public static GameEvents Singleton;

        public event Action<int> LoadLevel;
        public int CurrentLevel { get;  private set; }

        private void Start()
        {
            if (!Singleton)
            {
                Singleton = this;
                DontDestroyOnLoad(gameObject);
            }
            else Destroy(gameObject);
        }

        public void OnLoadLevel(int i)
        {
            CurrentLevel = i;
            LoadLevel?.Invoke(i);
        }
    }
}