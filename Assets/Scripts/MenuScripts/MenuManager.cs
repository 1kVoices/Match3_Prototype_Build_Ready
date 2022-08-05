using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Match3
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField]
        private PlayerProgressComponent _player;
        [SerializeField]
        private Animator[] _pedroAnimators;
        [SerializeField]
        private Animator[] _levelAnimators;
        [SerializeField]
        private Animator _blackScreenAnimator;

        private static readonly int ShowUp = Animator.StringToHash("showUp");
        private static readonly int Coloring = Animator.StringToHash("coloring");
        private static readonly int Highlight = Animator.StringToHash("highlight");
        private static readonly int Darkening = Animator.StringToHash("darkening");

        private void Start()
        {
            MenuEvents.BlackScreenBleached += OnBlackScreenBleached;
            MenuEvents.PedroAskedHelp += OnPedroAskedHelp;
            MenuEvents.BlackScreenDarken +=  OnBlackScreenDarken;
        }
        private void OnBlackScreenDarken()
        {
            SceneManager.LoadScene(1);
        }
        public void LoadLevel(int i) => GameEvents.Singleton.OnLoadLevel(i);
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

            for (int i = 0; i <= _player.PedroQuestProgress; i++)
            {
                _levelAnimators[i].SetTrigger(Coloring);
            }
        }
        private void OnDestroy()
        {
            SaveData.SavePlayerData(_player);
            MenuEvents.BlackScreenBleached -= OnBlackScreenBleached;
            MenuEvents.PedroAskedHelp -= OnPedroAskedHelp;
            MenuEvents.BlackScreenDarken -=  OnBlackScreenDarken;
        }
    }
}