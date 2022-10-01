using UnityEngine;

namespace Match3
{
    public class CanvasManager : MonoBehaviour, IData
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
        public bool IsTutorialShown { get; private set; }

        private void Start()
        {
            _blackScreen.SetTrigger(Extensions.Show);
            if (!IsTutorialShown)
                _tutorialScreen.SetTrigger(Extensions.Show);
        }

        public void CloseTutorial()
        {
            _tutorialScreen.SetTrigger(Extensions.Hide);
            IsTutorialShown = true;
            StartCoroutine(LevelManager.Singleton.ChipsShowUp());
        }

        public void CloseLostScreen()
        {
            _lostScreen.SetTrigger(Extensions.Hide);
            _blackScreen.SetTrigger(Extensions.Hide);
            DataManager.Singleton.SaveGame();
        }

        public void CloseWinScreen()
        {
            _winScreen.SetTrigger(Extensions.Hide);
            _blackScreen.SetTrigger(Extensions.Hide);
            DataManager.Singleton.SaveGame();
        }

        public void ShowLostScreen() => _lostScreen.SetTrigger(Extensions.Show);
        public void ShowWinScreen() => _winScreen.SetTrigger(Extensions.Show);
        public void ShowNoMoveScreen() => _noMoveScreen.SetTrigger(Extensions.Show);
        public void HideNoMoveScreen() => _noMoveScreen.SetTrigger(Extensions.Hide);

        public void LoadData(GameData data)
        {
            IsTutorialShown = data.TutorialShown;
        }

        public void SaveData(ref GameData data)
        {
            data.TutorialShown = IsTutorialShown;
        }
    }
}