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
        private GameData _data;
        private bool _isPaused;
        private static readonly int Highlight = Animator.StringToHash("highlight");
        public MoneyManager MoneyManager { get; private set; }
        public static MenuManager Singleton;

        private void Start()
        {
            Singleton = this;
            _pedro = FindObjectOfType<PedroCloud>();
            MoneyManager = FindObjectOfType<MoneyManager>();
            _pedro.AskedHelp += OnPedroAskedHelp;
            _blackScreen = FindObjectOfType<BlackScreen>();
            _blackScreen.OnScreenDarken += OnBlackScreenDarken;
            _blackScreen.OnScreenWhitening += OnBlackScreenBleached;
            _switcher = FindObjectOfType<DifficultySwitcher>();
            _blackScreen.Whitening();
            MoneyManager.gameObject.SetActive(false);
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
            MoneyManager.gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            _pedro.AskedHelp -= OnPedroAskedHelp;
            _blackScreen.OnScreenDarken -= OnBlackScreenDarken;
            _blackScreen.OnScreenWhitening -= OnBlackScreenBleached;
        }

        public void LoadData(GameData data) => _data = data;
        public void SaveData(ref GameData data) => data.CurrentLevel = _data.CurrentLevel;
    }
}