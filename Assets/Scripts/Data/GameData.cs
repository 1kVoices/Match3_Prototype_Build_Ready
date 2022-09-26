namespace Match3.Data
{
    [System.Serializable]
    public class GameData
    {
        public int CurrentLevel;
        public bool WasFirstStart;
        public int Money;
        public int PlayerLevel;
        public int UpgradePoints;
        public int M18RadiusLevel;
        public int HammerAmountLevel;
        public int HammerAmount;
        public int HammerHitAmountLevel;
        public int HammerHitAmount;
        public int HammerCdLevel;
        public int HammerCd;
        public int SledgeHammerAmountLevel;
        public int SledgeHammerHitAmountLevel;
        public int ExpAmountLevel;
        public int SpawnChance;

        public GameData()
        {
            CurrentLevel = 0;
            WasFirstStart = false;
            Money = 0;
            PlayerLevel = 0;
            UpgradePoints = 0;
            M18RadiusLevel = 0;
            HammerAmountLevel = 0;
            HammerAmount = 1;
            HammerHitAmountLevel = 0;
            HammerHitAmount = 1;
            HammerCdLevel = 0;
            HammerCd = 15;
            SledgeHammerAmountLevel = 0;
            SledgeHammerHitAmountLevel = 0;
            ExpAmountLevel = 0;
            SpawnChance = 0;
        }
    }
}