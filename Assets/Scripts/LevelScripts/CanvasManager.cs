using UnityEngine;

namespace Match3
{
    public class CanvasManager : MonoBehaviour
    {
        [SerializeField]
        private Animator _lostScreen;
        [SerializeField]
        private Animator _winScreen;
        [SerializeField]
        private Animator _tutorialScreen;
        [SerializeField]
        private Animator _blackScreen;
        [SerializeField]
        private Animator _noMoveScreen;

        private static readonly int Hide = Animator.StringToHash("hide");
        private static readonly int ShowUp = Animator.StringToHash("showUp");

        private void Start()
        {
            _tutorialScreen.SetTrigger(ShowUp);
            _blackScreen.SetTrigger(ShowUp);
        }

        public void CloseTutorial()
        {
            _tutorialScreen.SetTrigger(Hide);
            StartCoroutine(LevelManager.Singleton.ChipsShowUp());
        }

        public void CloseLostScreen()
        {
            _lostScreen.SetTrigger(Hide);
            _blackScreen.SetTrigger(Hide);
        }

        public void CloseWinScreen()
        {
            _winScreen.SetTrigger(Hide);
            _blackScreen.SetTrigger(Hide);
        }

        public void ShowLostScreen()
        {
            _lostScreen.SetTrigger(ShowUp);
        }

        public void ShowWinScreen()
        {
            _winScreen.SetTrigger(ShowUp);
        }

        public void ShowNoMoveScreen()
        {
            _noMoveScreen.SetTrigger(ShowUp);
        }

        public void HideNoMoveScreen()
        {
            _noMoveScreen.SetTrigger(Hide);
        }
    }
}