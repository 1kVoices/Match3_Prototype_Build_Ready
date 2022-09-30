namespace Match3
{
    public class HammerTree : Tree, IData
    {
        private void Start()
        {
            for (int i = 0; i < Data.HammerAmountLevel; i++)
                LeftBranch.Leaves[i].Upgrade();

            for (int i = 0; i < Data.HammerHitAmountLevel; i++)
                MidBranch.Leaves[i].Upgrade();

            for (int i = 0; i < Data.HammerCdLevel; i++)
                RightBranch.Leaves[i].Upgrade();
        }

        public override void Upgrade(Branch branch, int level)
        {
            if (branch == LeftBranch)
            {
                switch (level)
                {
                    case 0:
                        Data.HammerAmount = 2;
                        Data.HammerAmountLevel = 1;
                        break;
                    case 1:
                        Data.HammerAmount = 3;
                        Data.HammerAmountLevel = 2;
                        break;
                    case 2:
                        Data.HammerAmount = 4;
                        Data.HammerAmountLevel = 3;
                        break;
                }
            }
            else if (branch == MidBranch)
            {
                switch (level)
                {
                    case 0:
                        Data.HammerHitAmount = 2;
                        Data.HammerHitAmountLevel = 1;
                        break;
                    case 1:
                        Data.HammerHitAmount = 3;
                        Data.HammerHitAmountLevel = 2;
                        break;
                    case 2:
                        Data.HammerHitAmount = 4;
                        Data.HammerHitAmountLevel = 3;
                        break;
                }
            }
            else if (branch == RightBranch)
            {
                switch (level)
                {
                    case 0:
                        Data.HammerCd = 10;
                        Data.HammerCdLevel = 1;
                        break;
                    case 1:
                        Data.HammerCd = 5;
                        Data.HammerCdLevel = 2;
                        break;
                    case 2:
                        Data.HammerCd = 3;
                        Data.HammerCdLevel = 3;
                        break;
                }
            }
        }

        public void LoadData(GameData data) => Data = data;

        public void SaveData(ref GameData data)
        {
            data.HammerAmountLevel = Data.HammerAmountLevel;
            data.HammerHitAmountLevel = Data.HammerHitAmountLevel;
            data.HammerCdLevel = Data.HammerCdLevel;

            data.HammerAmount = Data.HammerAmount;
            data.HammerHitAmount = Data.HammerHitAmount;
            data.HammerCd = Data.HammerCd;
        }
    }
}