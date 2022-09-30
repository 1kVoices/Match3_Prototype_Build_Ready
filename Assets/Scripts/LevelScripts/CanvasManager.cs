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

        private void Start()
        {
            _tutorialScreen.SetTrigger(Extensions.Show);
            _blackScreen.SetTrigger(Extensions.Show);
        }

        public void CloseTutorial()
        {
            _tutorialScreen.SetTrigger(Extensions.Hide);
            StartCoroutine(LevelManager.Singleton.ChipsShowUp());
        }

        public void CloseLostScreen()
        {
            _lostScreen.SetTrigger(Extensions.Hide);
            _blackScreen.SetTrigger(Extensions.Hide);
        }

        public void CloseWinScreen()
        {
            _winScreen.SetTrigger(Extensions.Hide);
            _blackScreen.SetTrigger(Extensions.Hide);
        }

        public void ShowLostScreen() => _lostScreen.SetTrigger(Extensions.Show);
        public void ShowWinScreen() => _winScreen.SetTrigger(Extensions.Show);
        public void ShowNoMoveScreen() => _noMoveScreen.SetTrigger(Extensions.Show);
        public void HideNoMoveScreen() => _noMoveScreen.SetTrigger(Extensions.Hide);
    }
}