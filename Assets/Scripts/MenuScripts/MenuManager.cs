﻿using UnityEngine;
using UnityEngine.SceneManagement;

namespace Match3
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField]
        private Animator[] _pedroAnimators;
        [SerializeField]
        private Animator _blackScreenAnimator;
        private DifficultySwitcher _switcher;

        private static readonly int ShowUp = Animator.StringToHash("showUp");
        private static readonly int Coloring = Animator.StringToHash("coloring");
        private static readonly int Highlight = Animator.StringToHash("highlight");
        private static readonly int Darkening = Animator.StringToHash("darkening");

        private void Start()
        {
            MenuEvents.BlackScreenBleached += OnBlackScreenBleached;
            MenuEvents.PedroAskedHelp += OnPedroAskedHelp;
            MenuEvents.BlackScreenDarken += OnBlackScreenDarken;
            _switcher = FindObjectOfType<DifficultySwitcher>();
        }

        private static void OnBlackScreenDarken()
        {
            SceneManager.LoadScene(1);
        }

        public void LoadLevel(int i)
        {
            GameEvents.Singleton.OnLoadLevel(i);
        }

        public void AnimateLevelCircle(Animator levelAnimator)
        {
            _blackScreenAnimator.SetTrigger(Darkening);
            levelAnimator.SetTrigger(Highlight);
        }

        private void OnBlackScreenBleached()
        {
            foreach (Animator anim in _pedroAnimators)
                anim.SetTrigger(ShowUp);
        }

        private void OnPedroAskedHelp()
        {
            _switcher.ActivateSwitchers();
        }

        private void OnDestroy()
        {
            MenuEvents.BlackScreenBleached -= OnBlackScreenBleached;
            MenuEvents.PedroAskedHelp -= OnPedroAskedHelp;
            MenuEvents.BlackScreenDarken -= OnBlackScreenDarken;
        }
    }
}