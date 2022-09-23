namespace Match3.Data
{
    [System.Serializable]
    public class GameData
    {
        public int CurrentLevel;
        public bool WasFirstStart;
        public int CurrentPopularity;
        public int PlayerLevel;
        public int UpgradePoints;
        public int M18Level;
        public int HammerLevel;
        public int SledgeHammerLevel;
        public int PopularityAmountLevel;
        public int ChipChance;

        public GameData()
        {
            CurrentLevel = 0;
            WasFirstStart = false;
            CurrentPopularity = 0;
            PlayerLevel = 0;
            UpgradePoints = 0;
            M18Level = 0;
            HammerLevel = 0;
            SledgeHammerLevel = 0;
            PopularityAmountLevel = 0;
            ChipChance = 0;
        }
    }
}