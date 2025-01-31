﻿namespace Match3
{
    [System.Serializable]
    public class GameData
    {
        public int CurrentLevel;
        public bool WasFirstStart;
        public bool TutorialShown;
        public int Money;
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
        public SerializableDictionary<int, bool> LevelsCompleted;

        public GameData()
        {
            LevelsCompleted = new SerializableDictionary<int, bool>
            {
                {0, false},
                {1, false},
                {2, false},
                {3, false},
                {4, false},
                {5, false},
                {6, false},
                {7, false},
                {8, false},
                {9, false},
                {10, false},
                {11, false},
            };
            CurrentLevel = 0;
            WasFirstStart = false;
            TutorialShown = false;
            Money = 0;
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