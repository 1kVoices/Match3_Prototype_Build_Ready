using Match3.Data;

namespace Match3
{
    public class HammerTree : Tree, IData
    {
        private GameData _data;

        private void Start()
        {
            for (int i = 0; i < _data.HammerAmountLevel; i++)
            {
                LeftBranch.Leaves[i].Upgrade();
            }

            for (int i = 0; i < _data.HammerHitAmount; i++)
            {
                // MidBranch.Leaves[i].Upgrade();
            }

            for (int i = 0; i < _data.HammerCdLevel; i++)
            {
                // RightBranch.Leaves[i].Upgrade();
            }
        }

        public override void Upgrade(Branch branch, int level)
        {
            if (branch == LeftBranch)
            {
                switch (level)
                {
                    case 0:
                        _data.HammerAmount = 2;
                        _data.HammerAmountLevel = 1;
                        break;
                    case 1:
                        _data.HammerAmount = 3;
                        _data.HammerAmountLevel = 2;
                        break;
                    case 2:
                        _data.HammerAmount = 4;
                        _data.HammerAmountLevel = 3;
                        break;
                }
            }
            else if (branch == MidBranch)
            {
                switch (level)
                {
                    case 0:
                        _data.HammerHitAmount = 2;
                        _data.HammerAmountLevel = 1;
                        break;
                    case 1:
                        _data.HammerHitAmount = 3;
                        _data.HammerAmountLevel = 2;
                        break;
                    case 2:
                        _data.HammerHitAmount = 4;
                        _data.HammerAmountLevel = 3;
                        break;
                }
            }
            else if (branch == RightBranch)
            {
                switch (level)
                {
                    case 0:
                        _data.HammerCd = 10;
                        _data.HammerCdLevel = 1;
                        break;
                    case 1:
                        _data.HammerCd = 5;
                        _data.HammerCdLevel = 2;
                        break;
                    case 2:
                        _data.HammerCd = 3;
                        _data.HammerCdLevel = 3;
                        break;
                }
            }
        }

        public void LoadData(GameData data)
        {
            _data = data;
        }

        public void SaveData(ref GameData data)
        {
            data.HammerAmountLevel = _data.HammerAmountLevel;
            data.HammerHitAmountLevel = _data.HammerHitAmountLevel;
            data.HammerCdLevel = _data.HammerCdLevel;

            data.HammerAmount = _data.HammerAmount;
            data.HammerHitAmount = _data.HammerHitAmount;
            data.HammerCd = _data.HammerCd;
        }
    }
}