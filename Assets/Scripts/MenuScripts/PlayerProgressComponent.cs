using UnityEngine;

namespace Match3
{
    public class PlayerProgressComponent : MonoBehaviour
    {
        public int PedroQuestProgress { get; set; }
        public bool IsVeryFirstStart { get; set; }

        private void Start()
        {
            PlayerData player = SaveData.LoadData();

            if (player.IsNull()) return;

            PedroQuestProgress = player.GameProgress;
            IsVeryFirstStart = player.IsVeryFirstStart;
        }
    }
}