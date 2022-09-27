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
        public int HammerAmountLevel;
        public int HammerAmount;
        public int HammerHitAmountLevel;
        public int HammerHitAmount;
        public int HammerCdLevel;
        public int HammerCd;
        public int SledgeHammerAmountLevel;
        public int SledgeHammerAmount;
        public int SledgeHammerHitAmountLevel;
        public int SledgeHammerHitAmount;
        public int SledgeHammerCdLevel;
        public int SledgeHammerCd;
        public int SpawnChanceLevel;
        public int ExtraChance;
        public int ExpAmountLevel;
        public int ExtraExp;
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
            HammerAmount = 1;
            HammerHitAmountLevel = 0;
            HammerHitAmount = 1;
            HammerCdLevel = 0;
            HammerCd = 15;
            SledgeHammerAmountLevel = 0;
            SledgeHammerAmount = 1;
            SledgeHammerHitAmountLevel = 0;
            SledgeHammerHitAmount = 1;
            SledgeHammerCdLevel = 0;
            SledgeHammerCd = 15;
            SpawnChanceLevel = 0;
            ExtraChance = 0;
            ExpAmountLevel = 0;
            ExtraExp = 0;
            M18RadiusLevel = 0;
        }
    }
}