using System.Threading.Tasks;
using UnityEngine;

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

        private static readonly int ShowUp = Animator.StringToHash("showUp");
        private static readonly int Coloring = Animator.StringToHash("coloring");

        private void Start()
        {
            MenuEvents.Singleton.BlackScreenBleached += OnBlackScreenBleached;
            MenuEvents.Singleton.PedroAskedHelp += OnPedroAskedHelp;
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
            MenuEvents.Singleton.BlackScreenBleached -= OnBlackScreenBleached;
            MenuEvents.Singleton.PedroAskedHelp -= OnPedroAskedHelp;
        }
    }
}