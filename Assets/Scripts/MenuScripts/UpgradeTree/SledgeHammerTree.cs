namespace Match3
{
    public class SledgeHammerTree : Tree, IData
    {
        private void Start()
        {
            for (int i = 0; i < Data.SledgeHammerAmountLevel; i++)
                LeftBranch.Leaves[i].Upgrade();

            for (int i = 0; i < Data.SledgeHammerHitAmountLevel; i++)
                MidBranch.Leaves[i].Upgrade();

            for (int i = 0; i < Data.SledgeHammerCdLevel; i++)
                RightBranch.Leaves[i].Upgrade();
        }

        public override void Upgrade(Branch branch, int level)
        {
            if (branch == LeftBranch)
            {
                switch (level)
                {
                    case 0:
                        Data.SledgeHammerAmount = 2;
                        Data.SledgeHammerAmountLevel = 1;
                        break;
                    case 1:
                        Data.SledgeHammerAmount = 3;
                        Data.SledgeHammerAmountLevel = 2;
                        break;
                    case 2:
                        Data.SledgeHammerAmount = 4;
                        Data.SledgeHammerAmountLevel = 3;
                        break;
                }
            }
            else if (branch == MidBranch)
            {
                switch (level)
                {
                    case 0:
                        Data.SledgeHammerHitAmount = 2;
                        Data.SledgeHammerHitAmountLevel = 1;
                        break;
                    case 1:
                        Data.SledgeHammerHitAmount = 3;
                        Data.SledgeHammerHitAmountLevel = 2;
                        break;
                    case 2:
                        Data.SledgeHammerHitAmount = 4;
                        Data.SledgeHammerHitAmountLevel = 3;
                        break;
                }
            }
            else if (branch == RightBranch)
            {
                switch (level)
                {
                    case 0:
                        Data.SledgeHammerCd = 10;
                        Data.SledgeHammerCdLevel = 1;
                        break;
                    case 1:
                        Data.SledgeHammerCd = 5;
                        Data.SledgeHammerCdLevel = 2;
                        break;
                    case 2:
                        Data.SledgeHammerCd = 3;
                        Data.SledgeHammerCdLevel = 3;
                        break;
                }
            }
        }

        public void LoadData(GameData data)
        {
            Data = data;
        }

        public void SaveData(ref GameData data)
        {
            data.SledgeHammerAmountLevel = Data.SledgeHammerAmountLevel;
            data.SledgeHammerHitAmountLevel = Data.SledgeHammerHitAmountLevel;
            data.SledgeHammerCdLevel = Data.SledgeHammerCdLevel;

            data.SledgeHammerAmount = Data.SledgeHammerAmount;
            data.SledgeHammerHitAmount = Data.SledgeHammerHitAmount;
            data.SledgeHammerCd = Data.SledgeHammerCd;
        }
    }
}