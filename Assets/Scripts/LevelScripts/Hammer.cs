using System.Collections.Generic;

namespace Match3
{
    public class Hammer : Tool
    {
        protected override void PlayerClicked(Cell cell)
        {
            if (!IsInput) return;
            base.PlayerClicked(cell);
            LevelManager.Singleton.DestroyChips(null, cell);
            UpdateState();
            UpdateState();
        }
    }
}