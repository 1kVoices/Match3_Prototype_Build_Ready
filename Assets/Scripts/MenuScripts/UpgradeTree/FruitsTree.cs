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
                        Data.ExtraChance = 5;
                        Data.SpawnChanceLevel = 1;
                        break;
                    case 1:
                        Data.ExtraChance = 10;
                        Data.SpawnChanceLevel = 2;
                        break;
                    case 2:
                        Data.ExtraChance = 15;
                        Data.SpawnChanceLevel = 3;
                        break;
                }
            }
            else if (branch == MidBranch)
            {
                switch (level)
                {
                    case 0:
                        Data.ExtraExp = 5;
                        Data.ExpAmountLevel = 1;
                        break;
                    case 1:
                        Data.ExtraExp = 10;
                        Data.ExpAmountLevel = 2;
                        break;
                    case 2:
                        Data.ExtraExp = 15;
                        Data.ExpAmountLevel = 3;
                        break;
                }
            }
            else if (branch == RightBranch)
            {
                Data.M18RadiusLevel = level switch
                {
                    0 => 1,
                    1 => 2,
                    2 => 3,
                    _ => Data.M18RadiusLevel
                };
            }
        }

        public void LoadData(GameData data) => Data = data;

        public void SaveData(ref GameData data)
        {
            data.SpawnChanceLevel = Data.SpawnChanceLevel;
            data.ExpAmountLevel = Data.ExpAmountLevel;
            data.M18RadiusLevel = Data.M18RadiusLevel;
        }
    }
}