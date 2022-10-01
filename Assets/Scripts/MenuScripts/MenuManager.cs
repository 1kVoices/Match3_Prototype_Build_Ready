using UnityEngine;
using UnityEngine.SceneManagement;

namespace Match3
{
    public class MenuManager : MonoBehaviour, IData
    {
        [SerializeField]
        private Animator[] _pedroAnimators;
        private DifficultySwitcher _switcher;
        private BlackScreen _blackScreen;
        private PedroCloud _pedro;
        private PauseButton _pause;
        private GameData _data;
        private static readonly int Highlight = Animator.StringToHash("highlight");

        private TypeChip _type;

        private void Start()
        {
            _pedro = FindObjectOfType<PedroCloud>();
            _pedro.AskedHelp += OnPedroAskedHelp;
            _blackScreen = FindObjectOfType<BlackScreen>();
            _blackScreen.OnScreenDarken += OnBlackScreenDarken;
            _blackScreen.OnScreenWhitening += OnBlackScreenBleached;
            _switcher = FindObjectOfType<DifficultySwitcher>();
            _pause = FindObjectOfType<PauseButton>();
            _pause.PauseEvent += OnPauseEvent;
            _blackScreen.Whitening();
            MoneyManager.Singleton.gameObject.SetActive(false);
        }

        private void OnPauseEvent()
        {
            Time.timeScale = 0f;
        }

        private static void OnBlackScreenDarken() => SceneManager.LoadScene(1);

        public void LoadLevel(int i)
        {
            _data.CurrentLevel = i;
            DataManager.Singleton.SaveGame();
        }

        public void AnimateLevelCircle(Animator levelAnimator)
        {
            _blackScreen.Darken();
            levelAnimator.SetTrigger(Highlight);
        }

        private void OnBlackScreenBleached()
        {
            foreach (Animator anim in _pedroAnimators)
                anim.SetTrigger(Extensions.Show);
        }

        private void OnPedroAskedHelp()
        {
            _switcher.ActivateSwitchers();
            MoneyManager.Singleton.gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            _pedro.AskedHelp -= OnPedroAskedHelp;
            _blackScreen.OnScreenDarken -= OnBlackScreenDarken;
            _blackScreen.OnScreenWhitening -= OnBlackScreenBleached;
            _pause.PauseEvent -= OnPauseEvent;
        }

        public void LoadData(GameData data) => _data = data;
        public void SaveData(ref GameData data) => data.CurrentLevel = _data.CurrentLevel;
    }
}