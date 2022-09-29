namespace Match3
{
    [System.Serializable]
    public class GameData
    {
        public int CurrentLevel;
        public bool WasFirstStart;
        public int Money;
        public int PlayerLevel;
        public int UpgradePoints;
        public int HammerAmount;
        public int HammerHitAmount;
        public int HammerCd;
        public int SledgeHammerAmount;
        public int SledgeHammerHitAmount;
        public int SledgeHammerCd;
        public int ExtraExp;
        public int ExtraChance;
        public int HammerAmountLevel;
        public int HammerHitAmountLevel;
        public int HammerCdLevel;
        public int SledgeHammerAmountLevel;
        public int SledgeHammerHitAmountLevel;
        public int SledgeHammerCdLevel;
        public int SpawnChanceLevel;
        public int ExpAmountLevel;
        public int M18RadiusLevel;

        public GameData()
        {
            CurrentLevel = 0;
            WasFirstStart = false;
            Money = 0;
            PlayerLevel = 0;
            UpgradePoints = 0;
            M18RadiusLevel = 0;
            HammerAmountLevel = 0;
            HammerHitAmountLevel = 0;
            HammerCdLevel = 0;
            SledgeHammerAmountLevel = 0;
            SledgeHammerHitAmountLevel = 0;
            SledgeHammerCdLevel = 0;
            HammerAmount = 1;
            HammerHitAmount = 1;
            HammerCd = 15;
            SledgeHammerAmount = 1;
            SledgeHammerHitAmount = 1;
            SledgeHammerCd = 15;
            SpawnChanceLevel = 0;
            ExtraChance = 0;
            ExpAmountLevel = 0;
            ExtraExp = 0;
            M18RadiusLevel = 0;
        }
    }
}