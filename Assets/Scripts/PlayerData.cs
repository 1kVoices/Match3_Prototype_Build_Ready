using System;

namespace Match3
{
    [Serializable]
    public class PlayerData
    {
        public int GameProgress;
        public bool IsVeryFirstStart;

        public PlayerData(PlayerProgressComponent player)
        {
            GameProgress = player.PedroQuestProgress;
            IsVeryFirstStart = player.IsVeryFirstStart;
        }
    }
}