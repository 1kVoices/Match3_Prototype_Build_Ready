using System.Threading.Tasks;
using Match3.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Match3
{
    public class MenuManager : MonoBehaviour, IData
    {
        [SerializeField]
        private Animator[] _pedroAnimators;
        [SerializeField]
        private Animator[] _levelAnimators;
        [SerializeField]
        private Animator _blackScreenAnimator;
        private GameData _data;

        private static readonly int ShowUp = Animator.StringToHash("showUp");
        private static readonly int Coloring = Animator.StringToHash("coloring");
        private static readonly int Highlight = Animator.StringToHash("highlight");
        private static readonly int Darkening = Animator.StringToHash("darkening");

        private void Start()
        {
            MenuEvents.BlackScreenBleached += OnBlackScreenBleached;
            MenuEvents.PedroAskedHelp += OnPedroAskedHelp;
            MenuEvents.BlackScreenDarken += OnBlackScreenDarken;
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
            {
                anim.SetTrigger(ShowUp);
            }
        }

        private async void OnPedroAskedHelp()
        {
            foreach (Animator anim in _levelAnimators)
            {
                anim.SetTrigger(ShowUp);
                await Task.Delay(100);
            }

            for (int i = 0; i <= _data.CurrentLevel; i++)
            {
                _levelAnimators[i].SetTrigger(Coloring);
            }
        }

        private void OnDestroy()
        {
            MenuEvents.BlackScreenBleached -= OnBlackScreenBleached;
            MenuEvents.PedroAskedHelp -= OnPedroAskedHelp;
            MenuEvents.BlackScreenDarken -= OnBlackScreenDarken;
        }

        public void LoadData(GameData data)
        {
            _data = data;
        }

        public void SaveData(ref GameData data) {}
    }
}