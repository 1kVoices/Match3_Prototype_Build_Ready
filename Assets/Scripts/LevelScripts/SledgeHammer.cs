using System.Collections.Generic;

namespace Match3
{
    public class SledgeHammer : Tool
    {
        protected override void PlayerClicked(Cell cell)
        {
            if (!IsInput) return;
            base.PlayerClicked(cell);
            LevelManager.Singleton.DestroyChips(null,
                cell, cell.GetNeighbour(DirectionType.Top),
                cell.GetNeighbour(DirectionType.Bot),
                cell.GetNeighbour(DirectionType.Left),
                cell.GetNeighbour(DirectionType.Right));
            UpdateState();
        }
    }
}