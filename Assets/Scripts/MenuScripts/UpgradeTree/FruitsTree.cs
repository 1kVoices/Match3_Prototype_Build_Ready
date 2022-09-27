namespace Match3
{
    public class FruitsTree : Tree, IData
    {
        private void Start()
        {
            for (int i = 0; i < Data.SpawnChanceLevel; i++)
                LeftBranch.Leaves[i].Upgrade();

            for (int i = 0; i < Data.ExpAmountLevel; i++)
                MidBranch.Leaves[i].Upgrade();

            for (int i = 0; i < Data.M18RadiusLevel; i++)
                RightBranch.Leaves[i].Upgrade();
        }

        public override void Upgrade(Branch branch, int level)
        {
            if (branch == LeftBranch)
            {
                switch (level)
                {
                    case 0:
                        Data.ExtraChance = 1;
                        Data.SpawnChanceLevel = 1;
                        break;
                    case 1:
                        Data.ExtraChance = 2;
                        Data.SpawnChanceLevel = 2;
                        break;
                    case 2:
                        Data.ExtraChance = 3;
                        Data.SpawnChanceLevel = 3;
                        break;
                }
            }
            else if (branch == MidBranch)
            {
                switch (level)
                {
                    case 0:
                        Data.ExtraExp = 100;
                        Data.ExpAmountLevel = 1;
                        break;
                    case 1:
                        Data.ExtraExp = 150;
                        Data.ExpAmountLevel = 2;
                        break;
                    case 2:
                        Data.ExtraExp = 200;
                        Data.ExpAmountLevel = 3;
                        break;
                }
            }
            else if (branch == RightBranch)
            {
                switch (level)
                {
                    case 0:
                        Data.M18RadiusLevel = 1;
                        break;
                    case 1:
                        Data.M18RadiusLevel = 2;
                        break;
                    case 2:
                        Data.M18RadiusLevel = 3;
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
            data.SpawnChanceLevel = Data.SpawnChanceLevel;
            data.ExpAmountLevel = Data.ExpAmountLevel;
            data.M18RadiusLevel = Data.M18RadiusLevel;
        }
    }
}